﻿using Newtonsoft.Json;
using Rnd.Bot.Discord.Models.Character.Fields;
using Rnd.Bot.Discord.Models.Glossaries;
using Rnd.Bot.Discord.Models.Rolls;
using Rnd.Bot.Discord.Views;

namespace Rnd.Bot.Discord.Models.Character.Panels;

public class Domains<TDomain, TSkill> : IPanel, IValidatable
    where TDomain : struct 
    where TSkill : struct
{
    public Domains(ICharacter character, List<Domain<TDomain, TSkill>> coreDomains)
    {
        Character = character;
        CoreDomains = coreDomains;
    }

    [JsonIgnore]
    public ICharacter Character { get; }

    [JsonIgnore] 
    public int MaxSkillLevel => (int) Math.Floor((double) Character.Attributes.Power.Max / 8) + 6;
    
    public List<Domain<TDomain, TSkill>> CoreDomains { get; }

    public void SetDomainLevel(TDomain domainType, int? value)
    {
        if (value != null)
        {
            CoreDomains.First(d => Glossary.GetDomainName(d.DomainType) == Glossary.GetDomainName(domainType))
                .DomainLevel = value.GetValueOrDefault();
        }
    }
    
    [JsonIgnore]
    public IReadOnlyCollection<Skill<TSkill>> CoreSkills
    {
        get
        {
            var result = new List<Skill<TSkill>>();
            
            CoreDomains.ForEach(d => result.AddRange(d.Skills));

            return result;
        }
    }
    
    [JsonIgnore]
    public IReadOnlyCollection<Domain<TDomain, TSkill>> FinalDomains
    {
        get
        {
            var result = new List<Domain<TDomain, TSkill>>();

            foreach (var domain in CoreDomains.Select(d => 
                         new Domain<TDomain, TSkill>(
                             d.DomainType, 
                             d.Skills.Select(s => new Skill<TSkill>(s.SkillType, s.Value)).ToList(), 
                             d.DomainLevel)))
            {
                foreach (var effect in Character.Effects.FinalEffects)
                {
                    effect.ModifyDomain(domain);
                }

                foreach (var skill in domain.Skills)
                {
                    skill.Value += domain.DomainLevel;
                    
                    foreach (var effect in Character.Effects.FinalEffects)
                    {
                        effect.ModifySkill(skill);
                    }
                }
                
                result.Add(domain);
            }

            return result;
        }
    }

    [JsonIgnore]
    public IReadOnlyCollection<Skill<TSkill>> FinalSkills
    {
        get
        {
            var result = new List<Skill<TSkill>>();
            
            FinalDomains.ToList().ForEach(d => result.AddRange(d.Skills));

            return result;
        }
    }
    
    public SkillRoll<TSkill> GetRoll(TSkill skillType, AttributeType? attributeType = null, int advantages = 0, int modifier = 0)
    {
        var roll = new SkillRoll<TSkill>(
            Character.Attributes.FinalAttributes.First(a => a.AttributeType == (attributeType ?? Glossary.GetSkillCoreAttribute(skillType))),
            FinalSkills.First(s => Glossary.GetSkillName(s.SkillType) == Glossary.GetSkillName(skillType)),
            Character.Pointers.IsNearDeath,
            Character.GetFooter,
            advantages,
            modifier
        );

        return roll;
    }

    [JsonIgnore]
    public string Title => "Навыки";
    
    [JsonIgnore]
    public List<IField> Fields => FinalDomains.Select(a => (IField) a).ToList();
    
    [JsonIgnore]
    public string Footer => Character.GetFooter;
    
    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            var valid = true;
            var errors = new List<string>();

            var avg = (decimal) CoreDomains.Sum(d => d.DomainLevel) / CoreDomains.Count;

            if (avg != 4)
            {
                valid = false;
                errors.Add($"Сумма уровней всех доменов должна быть равна {4 * CoreDomains.Count}");
            }
            
            var errorDomains = CoreDomains.Where(d => d.DomainLevel is > 8 or < 0).ToList();

            if (errorDomains.Any())
            {
                valid = false;

                var domainsJoin = String.Join(", ", 
                    errorDomains.Select(d => $"{d.Name}"));
                
                errors.Add($"Домены: {domainsJoin} – должны иметь уровень базового значения от 0 до 8.");
            }

            var errorSkills = CoreSkills.Where(s => s.Value > MaxSkillLevel).ToList();

            if (errorSkills.Any())
            {
                valid = false;

                var errorFinalSkills = FinalSkills
                    .Where(s => errorSkills.Select(skill => skill.SkillType).Contains(s.SkillType));
                
                var skillsJoin = String.Join(", ", 
                    errorFinalSkills.Select(s => 
                        $"{s.Name} `{s.Value}`/" +
                        $"`{s.Value - (errorSkills.First(skill => Glossary.GetSkillName(skill.SkillType) == Glossary.GetSkillName(s.SkillType)).Value - MaxSkillLevel)}`"));
                
                errors.Add($"Навыки: {skillsJoin} – превышают максимальный уровень.");
            }
            
            var negateErrorCoreSkills = CoreSkills.Where(s => s.Value < 0).ToList();

            if (negateErrorCoreSkills.Any())
            {
                valid = false;
                
                var skillsJoin = String.Join(", ", negateErrorCoreSkills.Select(s => $"{s.Name} `{s.Value}`"));
                
                errors.Add($"Навыки: {skillsJoin} – не могуть иметь уровень меньше 0 до применения эффектов.");
            }
            else
            {
                var negateErrorSkills = FinalSkills.Where(s => s.Value < 0).ToList();

                if (negateErrorSkills.Any())
                {
                    valid = false;
                
                    var skillsJoin = String.Join(", ", negateErrorSkills.Select(s => $"{s.Name} `{s.Value}`"));
                
                    errors.Add($"Навыки: {skillsJoin} – не могуть иметь уровень меньше 0.");
                }
            }

            Errors = errors.ToArray();
            return valid;
        }
    }

    [JsonIgnore]
    public string[]? Errors { get; private set; }
}