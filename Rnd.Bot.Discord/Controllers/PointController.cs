﻿using System.Text;
using Discord;
using Discord.Interactions;
using Rnd.Bot.Discord.Controllers.Helpers;
using Rnd.Bot.Discord.Data;
using Rnd.Bot.Discord.Models.Character.Panels.Effect;
using Rnd.Bot.Discord.Models.Glossaries;
using Rnd.Bot.Discord.Views;
using ValueType = Rnd.Bot.Discord.Views.ValueType;

namespace Rnd.Bot.Discord.Controllers;

[Group("point", "Команды для управления состояниями и очками персонажа")]
public class PointController : InteractionModuleBase<SocketInteractionContext>
{
    //Dependency Injections
    public DataContext Db { get; set; } = null!;
    
    [AutocompleteCommand("состояние", "modify")]
    public async Task PointNameAutocomplete()
    {
        var autocomplete = new Autocomplete<string>(Context, 
            Glossary.PointerNamesReversed.Keys, 
            s => s);
        
        await autocomplete.RespondAsync();
    }

    [SlashCommand("modify", "Изменение выбранного состояния или очков на указанное значение")]
    public async Task ModifyAsync(
        [Summary("состояние", "Название состояния/очков для изменения")] [Autocomplete] string name,
        [Summary("значение", "Значение на которое изменится состояние")] int modifier = 1,
        [Summary("игрок", "Пользователь для выполнения команды")] IUser? player = null)
    {
        var depot = new CharacterDepot(Db, Context, player);

        var character = await depot.GetCharacterAsync();
        
        var type = Glossary.PointerNamesReversed[name];
        character.Pointers.PointersCurrent[type] += modifier;

        var avoidLock = true;
        
        //TODO heal
        //avoidLock = modifier < 0 && type is PointerType.Ability or PointerType.Drama;

        await depot.UpdateCharacterAsync(character, avoidLock);

        var pointer = character.Pointers.FinalPointers.First(p => p.PointerType == type);
        var modifierString = EmbedView.Build(modifier, ValueType.InlineModifier);
        
        await RespondAsync($"**{pointer.Name}** {character.Name} изменено на `{modifierString}`.\n" +
                           $"Всего `{pointer.Current}/{pointer.Max}`.");
    }
    
    [AutocompleteCommand("тип", "damage")]
    public async Task DamageTypeAutocomplete()
    {
        var autocomplete = new Autocomplete<string>(Context, 
            Glossary.DamageNamesReversed.Keys, 
            s => s);
        
        await autocomplete.RespondAsync();
    }

    [SlashCommand("damage", "Нанесение урона выбранного типа")]
    public async Task DamageAsync(
        [Summary("тип", "Тип наносимого урона")] [Autocomplete] string damageType = "Физический",
        [Summary("урон", "Наносимый урон")] int damage = 1,
        [Summary("игрок", "Пользователь для выполнения команды")] IUser? player = null)
    {
        if (damage < 1)
        {
            await RespondAsync(embed: EmbedView.Error("Параметр урон должен быть больше нуля."), ephemeral: true);
            return;
        }
        
        var depot = new CharacterDepot(Db, Context, player);

        var character = await depot.GetCharacterAsync();
        
        var type = Glossary.DamageNamesReversed[damageType];
        
        var finalArmor = Glossary.DamageArmor[type] == null ? null : character.Pointers.FinalPointers.First(p => p.PointerType == Glossary.DamageArmor[type]);
        var finalHits = Glossary.DamageHit[type] == null ? null : character.Pointers.FinalPointers.First(p => p.PointerType == Glossary.DamageHit[type]);

        var current = character.Pointers.PointersCurrent;

        string? effects = null;
        
        if (finalArmor is {Current: > 0})
        {
            current[finalArmor.PointerType]--;
            finalArmor.Current--;

            effects = "*Отрицательные эффеткы заблокированы.*";
        }
        else if (finalHits is {Current: > 0})
        {
            current[finalHits.PointerType] -= damage;
            finalHits.Current -= damage;
            
            if (finalHits.Current < 0)
            {
                current[finalHits.PointerType] -= finalHits.Current;
                finalHits.Current = 0;
            }
            
            if (finalHits.Current == 0) effects = "*Персонаж присмерти!*";
        }
        else
        {
            var deathRoll = finalHits?.PointerType switch
            {
                null => character.Domains.GetRoll(AncorniaSkillType.Fortitude),
                PointerType.Body => character.Domains.GetRoll(AncorniaSkillType.Fortitude),
                PointerType.Will => character.Domains.GetRoll(AncorniaSkillType.SelfControl),
                _ => throw new ArgumentOutOfRangeException()
            };

            deathRoll.IsNearDeath = false;
            
            deathRoll.Roll();

            var result = deathRoll.SkillResult;

            TraumaEffect trauma;
            
            if (result <= 1)
            {
                trauma = new TraumaEffect(TraumaType.Deadly, type);
            }
            else if (result <= damage + 1)
            {
                trauma = new TraumaEffect(TraumaType.Critical, type);
            }
            else if (result <= 2 * damage + 1)
            {
                trauma = new TraumaEffect(TraumaType.Heavy, type);
            }
            else
            {
                trauma = new TraumaEffect(TraumaType.Light, type);
            }
            
            var count = character.Traumas.TraumaEffects.Count(t => t.Name == trauma.Name);
            trauma.Number = count;
            
            var finalPointers = character.Pointers.FinalPointers;
        
            character.Traumas.TraumaEffects.Add(trauma);
        
            character.Pointers.UpdateCurrentPoints(finalPointers);

            effects += $"Получена *{trauma.Name.ToLower()}*";
        }
        
        if (!character.IsValid)
        {
            await RespondAsync(embed: EmbedView.Error(character.Errors), ephemeral: true);
            return;
        }
        
        //TODO сделать панель для этого дела с моделью и выводить поулченные травмы
        
        await depot.UpdateCharacterAsync(character, true);

        var sb = new StringBuilder();

        sb.AppendLine($"**{character.Name}** получает `{damage}` ({damageType}) урон.");
        if (effects != null) sb.AppendLine(effects);
        if (finalArmor != null) sb.AppendLine($"{finalArmor.Name}: `{finalArmor.Current}/{finalArmor.Max}`");
        if (finalHits != null) sb.AppendLine($"{finalHits.Name}: `{finalHits.Current}/{finalHits.Max}`");

        await RespondAsync(sb.ToString());
    }

    [AutocompleteCommand("состояние", "refresh")]
    public async Task PointNameRefreshAutocomplete()
    {
        var autocomplete = new Autocomplete<string>(Context, 
            Glossary.PointerNamesReversed.Keys, 
            s => s);
        
        await autocomplete.RespondAsync();
    }

    [SlashCommand("refresh", "Устанавливает значение по умолчанию")]
    public async Task RefreshAsync(
        [Summary("состояние", "Название состояния для изменения")] [Autocomplete] string? name = null,
        [Summary("игрок", "Пользователь для выполнения команды")] IUser? player = null)
    {
        var depot = new CharacterDepot(Db, Context, player);

        var character = await depot.GetCharacterAsync();

        string message;
        
        if (name == null)
        {
            character.Pointers.CorePointers.Where(p => p.PointerType is not PointerType.Drama).ToList()
                .ForEach(p => character.Pointers.PointersCurrent[p.PointerType] = p.Max);

            message = $"Все состояния **{character.Name}** сброшены.";
        }
        else
        {
            var pointer = character.Pointers.CorePointers.First(p => p.PointerType == Glossary.PointerNamesReversed[name]);
            character.Pointers.PointersCurrent[pointer.PointerType] = pointer.Max;
            
            message = $"**{pointer.Name}** {character.Name} сброшено.";
        }
        
        if (!character.IsValid)
        {
            await RespondAsync(embed: EmbedView.Error(character.Errors), ephemeral: true);
            return;
        }
            
        await depot.UpdateCharacterAsync(character);
            
        await RespondAsync(message);
    }

    [SlashCommand("rest", "Активирует длительный отдых, энергия тратяться на худшее состояние")]
    public async Task RestAsync(
        [Summary("игрок", "Пользователь для выполнения команды")] IUser? player = null)
    {
        var depot = new CharacterDepot(Db, Context, player);

        var character = await depot.GetCharacterAsync();
        
        var finalAbility = character.Pointers.FinalPointers.First(p => p.PointerType == PointerType.Energy);
        var finalBody = character.Pointers.FinalPointers.First(p => p.PointerType == PointerType.Body);
        var finalWill = character.Pointers.FinalPointers.First(p => p.PointerType == PointerType.Will);

        var ability = character.Pointers.CorePointers.First(p => p.PointerType == PointerType.Energy);

        var current = character.Pointers.PointersCurrent;
        
        var healPoints = finalAbility.Current;

        //TODO переделать это на формулу/функцию
        while (healPoints > 0 && (finalBody.Current < finalBody.Max || finalWill.Current < finalWill.Max))
        {
            var mostDamaged = 
                finalWill.Current >= finalBody.Current 
                    ? finalBody.Current >= finalBody.Max ? finalWill : finalBody 
                    : finalWill.Current >= finalWill.Max ? finalBody : finalWill;

            mostDamaged.Current++;
            current[mostDamaged.PointerType]++;
            healPoints--;
        }

        current[PointerType.Energy] = ability.Max;
        
        if (!character.IsValid)
        {
            await RespondAsync(embed: EmbedView.Error(character.Errors), ephemeral: true);
            return;
        }
            
        await depot.UpdateCharacterAsync(character, true);
            
        await RespondAsync($"**{character.Name}** совершает длительный отдых.", embed: EmbedView.Build(character.Pointers));
    }
}