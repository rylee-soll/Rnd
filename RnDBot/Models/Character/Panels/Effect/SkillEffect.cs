﻿using Newtonsoft.Json;
using RnDBot.Models.Character.Fields;
using RnDBot.Models.Glossaries;
using RnDBot.Views;
using ValueType = RnDBot.Views.ValueType;

namespace RnDBot.Models.Character.Panels.Effect;

public class SkillEffect<TEffectSkill> : IEffect
{
    public SkillEffect(string name, TEffectSkill skillType, int modifier = 0)
    {
        Name = name;
        SkillType = skillType;
        Modifier = modifier;
    }

    public string Name { get; }
    public TEffectSkill SkillType { get; }
    public int Modifier { get; }
    
    public void ModifySkill<TSkill>(Skill<TSkill> skill) where TSkill : struct
    {
        //TODO Убрать костыль с глоссарием и сравнивать напрямую
        if (SkillType is not TSkill type) return;
        if (Glossary.GetSkillName(skill.SkillType) != Glossary.GetSkillName(type)) return;
        
        skill.Value += Modifier;
        skill.Modified = true;
    }
    
    [JsonIgnore]
    public string View => $"**{Name}** {Glossary.GetSkillName(SkillType)} " +
                          $"`{EmbedView.Build(Modifier, ValueType.InlineModifier)}`";
}