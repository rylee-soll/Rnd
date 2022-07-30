﻿using Discord.Interactions;
using RnDBot.Controllers.Helpers;
using RnDBot.Data;
using RnDBot.Models.Glossaries;
using RnDBot.Views;
using ValueType = RnDBot.Views.ValueType;

namespace RnDBot.Controllers;

[Group("point", "Команды для управления состояниями и очками персонажа")]
public class PointController : InteractionModuleBase<SocketInteractionContext>
{
    //Dependency Injections
    public DataContext Db { get; set; } = null!;
    
    [AutocompleteCommand("состояние", "modify")]
    public async Task PointModifyAutocomplete()
    {
        var autocomplete = new Autocomplete<string>(Context, 
            Glossary.PointerNamesReversed.Keys, 
            s => s);
        
        await autocomplete.RespondAsync();
    }

    [SlashCommand("modify", "Изменение выбранного состояния или очков на указанное значение")]
    public async Task ModifyAsync(
        [Summary("состояние", "Название состояния/очков для изменения")] [Autocomplete] string name,
        [Summary("значение", "Значение на которое изменится состояние")] int modifier = 1)
    {
        var depot = new CharacterDepot(Db, Context);

        var character = await depot.GetCharacterAsync();
        
        var type = Glossary.PointerNamesReversed[name];
        var pointer = character.Pointers.CorePointers.First(p => p.PointerType == type);
        pointer.Current += modifier;
            
        if (!character.IsValid)
        {
            await RespondAsync(embed: EmbedView.Error(character.Errors), ephemeral: true);
            return;
        }
        
        await depot.UpdateCharacterAsync(character);

        var modifierString = EmbedView.Build(modifier, ValueType.InlineModifier);
        
        await RespondAsync($"**{pointer.Name}** {character.Name} изменено на `{modifierString}`.\n" +
                           $"Всего `{pointer.Current}/{pointer.Max}`.");
    }

    [AutocompleteCommand("состояние", "refresh")]
    public async Task PointRefreshAutocomplete()
    {
        var autocomplete = new Autocomplete<string>(Context, 
            Glossary.PointerNamesReversed.Keys, 
            s => s);
        
        await autocomplete.RespondAsync();
    }

    [SlashCommand("refresh", "Устанавливает знаечние по умолчанию")]
    public async Task RefreshAsync(
        [Summary("состояние", "Название состояния для изменения")] [Autocomplete] string? name = null)
    {
        var depot = new CharacterDepot(Db, Context);

        var character = await depot.GetCharacterAsync();

        var message = "";
        
        if (name == null)
        {
            character.Pointers.CorePointers.Where(p => p.PointerType is not PointerType.Drama).ToList()
                .ForEach(p => p.Current = p.Max);

            message = $"Все состояния **{character.Name}** сброшены.";
        }
        else
        {
            var pointer = character.Pointers.CorePointers.First(p => p.PointerType == Glossary.PointerNamesReversed[name]);
            pointer.Current = pointer.Max;
            
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

    [SlashCommand("rest", "Активирует длительный отдых, очки способностей тратяться на худшее состояние")]
    public async Task RestAsync()
    {
        var depot = new CharacterDepot(Db, Context);

        var character = await depot.GetCharacterAsync();

        var ability = character.Pointers.CorePointers.First(p => p.PointerType == PointerType.Ability);
        var body = character.Pointers.CorePointers.First(p => p.PointerType == PointerType.Body);
        var will = character.Pointers.CorePointers.First(p => p.PointerType == PointerType.Will);
        
        var healPoints = ability.Current;

        while (healPoints > 0 && (body.Current < body.Max || will.Current < will.Max))
        {
            var mostDamaged = body.Max - body.Current >= will.Max - will.Current ? body : will;

            mostDamaged.Current++;
            healPoints--;
        }

        ability.Current = ability.Max;
        
        if (!character.IsValid)
        {
            await RespondAsync(embed: EmbedView.Error(character.Errors), ephemeral: true);
            return;
        }
            
        await depot.UpdateCharacterAsync(character);
            
        await RespondAsync($"**{character.Name}** совершает длительный отдых.", embed: EmbedView.Build(character.Pointers));
    }
}