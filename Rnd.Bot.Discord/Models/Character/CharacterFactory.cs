﻿using Rnd.Bot.Discord.Models.Character.Fields;
using Rnd.Bot.Discord.Models.Glossaries;

namespace Rnd.Bot.Discord.Models.Character;

//Создает и предзаполняет объекты персонажей в зависимости от сеттинга
public static class CharacterFactory
{
    public static AbstractCharacter AbstractCharacter(string name)
    {
        var character = new AbstractCharacter(name);

        return character;
    }
    
    public static AncorniaCharacter AncorniaCharacter(string name)
    {
        var domains = new List<Domain<AncorniaDomainType, AncorniaSkillType>>
        {
            new(AncorniaDomainType.War, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Bruteforce),
                CreateSkill(AncorniaSkillType.Blocking),
                CreateSkill(AncorniaSkillType.Struggle),
                CreateSkill(AncorniaSkillType.Fortitude),
                CreateSkill(AncorniaSkillType.Fencing),
                CreateSkill(AncorniaSkillType.Throwing),
                CreateSkill(AncorniaSkillType.Shooting),
                CreateSkill(AncorniaSkillType.Riding),
            }),
            new(AncorniaDomainType.Mist, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Alchemy),
                CreateSkill(AncorniaSkillType.Enchantment),
                CreateSkill(AncorniaSkillType.Magic),
                CreateSkill(AncorniaSkillType.Necromancy),
                CreateSkill(AncorniaSkillType.Shamanism),
                CreateSkill(AncorniaSkillType.Priesthood),
                CreateSkill(AncorniaSkillType.Demonology),
                CreateSkill(AncorniaSkillType.Metamorphism),
            }),
            new(AncorniaDomainType.Way, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.SleightOfHand),
                CreateSkill(AncorniaSkillType.Acrobatics),
                CreateSkill(AncorniaSkillType.Stealth),
                CreateSkill(AncorniaSkillType.Reaction),
                CreateSkill(AncorniaSkillType.Tracking),
                CreateSkill(AncorniaSkillType.Navigation),
                CreateSkill(AncorniaSkillType.Streets),
                CreateSkill(AncorniaSkillType.Survival),
            }),
            new(AncorniaDomainType.Word, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Empathy),
                CreateSkill(AncorniaSkillType.Rhetoric),
                CreateSkill(AncorniaSkillType.Manipulation),
                CreateSkill(AncorniaSkillType.Networking),
                CreateSkill(AncorniaSkillType.Authority),
                CreateSkill(AncorniaSkillType.SelfControl),
            }),
            new(AncorniaDomainType.Lore, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Research),
                CreateSkill(AncorniaSkillType.Engineering),
                CreateSkill(AncorniaSkillType.Medicine),
                CreateSkill(AncorniaSkillType.Nature),
                CreateSkill(AncorniaSkillType.History),
                CreateSkill(AncorniaSkillType.Society),
                CreateSkill(AncorniaSkillType.Science),
            }),
            new(AncorniaDomainType.Work, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Metallurgy),
                CreateSkill(AncorniaSkillType.Farming),
                CreateSkill(AncorniaSkillType.Mining),
                CreateSkill(AncorniaSkillType.Construction),
                CreateSkill(AncorniaSkillType.Craft),
                CreateSkill(AncorniaSkillType.Mechanisms),
                CreateSkill(AncorniaSkillType.Chemistry),
            }),
            new(AncorniaDomainType.Art, new List<Skill<AncorniaSkillType>>
            {
                CreateSkill(AncorniaSkillType.Jewelry),
                CreateSkill(AncorniaSkillType.Culture),
                CreateSkill(AncorniaSkillType.Creation),
                CreateSkill(AncorniaSkillType.Inspiration),
                CreateSkill(AncorniaSkillType.Performance),
                CreateSkill(AncorniaSkillType.Artistry),
            }),
        };

        var character = new AncorniaCharacter(AbstractCharacter(name), domains);

        return character;
    }

    private static Skill<TSkill> CreateSkill<TSkill>(TSkill type) 
        where TSkill : struct
    {
        return new(type);
    }
}