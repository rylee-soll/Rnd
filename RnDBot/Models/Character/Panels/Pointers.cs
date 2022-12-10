﻿using Newtonsoft.Json;
using RnDBot.Models.Character.Fields;
using RnDBot.Models.Glossaries;
using RnDBot.Views;

namespace RnDBot.Models.Character.Panels;

/// <summary>
/// Point Counters
/// </summary>
public class Pointers : IPanel, IValidatable
{
    public Pointers(ICharacter character)
    {
        Character = character;
        PointersCurrent = new Dictionary<PointerType, int>
        {
            [PointerType.Body] = CorePointersMax[PointerType.Body],
            [PointerType.Will] = CorePointersMax[PointerType.Will],
            [PointerType.Armor] = CorePointersMax[PointerType.Armor],
            [PointerType.Barrier] = CorePointersMax[PointerType.Barrier],
            [PointerType.Energy] = CorePointersMax[PointerType.Energy],
            [PointerType.Drama] = 3,
            [PointerType.Mana] = 0,
            [PointerType.Vital] = 0,
        };
    }

    [JsonConstructor]
    public Pointers(ICharacter character, Dictionary<PointerType, int> pointersCurrent)
    {
        Character = character;
        PointersCurrent = pointersCurrent;
    }

    [JsonIgnore]
    public ICharacter Character { get; }
    
    public Dictionary<PointerType, int> PointersCurrent { get; }

    [JsonIgnore]
    public IReadOnlyDictionary<PointerType, int> CorePointersMax => new Dictionary<PointerType, int>
    {
        [PointerType.Body] = BodySkill + 10 + (Endurance <= 0 ? 0 : 10 + Endurance),
        [PointerType.Will] = WillSkill + 10 + (Determinism <= 0 ? 0 : 10 + Determinism),
        [PointerType.Armor] = ArmorSkill / 2 + 0,
        [PointerType.Barrier] = BarrierSkill / 2 + 0,
        [PointerType.Energy] = EnergySkill / 4 + Character.Attributes.Power.Max / 7 + 4,
        [PointerType.Drama] = 6,
        [PointerType.Mana] = (int) (MagicSkill / 2.5 * (EnergySkill / 4 + Character.Attributes.Power.Max / 7 + 4)),
        [PointerType.Vital] = (int) (NecromancySkill / 2.5 * (EnergySkill / 4 + Character.Attributes.Power.Max / 7 + 4)),
    };

    private int Endurance => Character.Attributes.CoreAttributes.First(a => a.AttributeType == AttributeType.End).Modifier;
    private int Determinism => Character.Attributes.CoreAttributes.First(a => a.AttributeType == AttributeType.Det).Modifier;
    private Character<AncorniaDomainType, AncorniaSkillType>? ConcreteCharacter => Character as Character<AncorniaDomainType, AncorniaSkillType>;
    private int BodySkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Body)?.Value ?? 0;
    private int WillSkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Will)?.Value ?? 0;
    private int ArmorSkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Armor)?.Value ?? 0;
    private int BarrierSkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Barrier)?.Value ?? 0;
    private int EnergySkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Energy)?.Value ?? 0;
    private int MagicSkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Magic)?.Value ?? 0;
    private int NecromancySkill => ConcreteCharacter?.Domains?.FinalSkills?.FirstOrDefault(s => s.SkillType == AncorniaSkillType.Necromancy)?.Value ?? 0;

    
    [JsonIgnore]
    public IReadOnlyCollection<Pointer> CorePointers => new List<Pointer>
    {
        GetCorePointer(PointerType.Armor),
        GetCorePointer(PointerType.Barrier),
        GetCorePointer(PointerType.Drama),
        GetCorePointer(PointerType.Body),
        GetCorePointer(PointerType.Will),
        GetCorePointer(PointerType.Energy),
        GetCorePointer(PointerType.Mana),
        GetCorePointer(PointerType.Vital),
    };

    private Pointer GetCorePointer(PointerType type) => new(type, CorePointersMax[type], PointersCurrent[type]);
    
    public void UpdateCurrentPoints(IReadOnlyCollection<Pointer> originalPointers, bool updateFinal = true)
    {
        foreach (var originalPointer in originalPointers)
        {
            var type = originalPointer.PointerType;
            var originalMax = originalPointer.Max;
            
            var pointer = updateFinal 
                ? FinalPointers.First(p => p.PointerType == type) 
                : CorePointers.First(p => p.PointerType == type);
            
            if (pointer.Max == originalMax) continue;

            if (pointer.Max < originalMax)
            {
                if (updateFinal)
                {
                    PointersCurrent[type] -= pointer.Max - originalMax;
                    
                    pointer = FinalPointers.First(p => p.PointerType == type); 
                }
                
                var difference = updateFinal 
                    ? CorePointersMax[type] - pointer.Max 
                    : 0;

                if (pointer.Current > pointer.Max)
                {
                    PointersCurrent[type] = pointer.Max + difference;
                }
                
                continue;
            }
            
            if (updateFinal) continue;

            PointersCurrent[type] += pointer.Max - originalMax;
        }
    }
    
    public void SetPointers(int? drama = null, int? ability = null, int? body = null, int? will = null, int? armor = null, 
        int? barrier = null, int? mana = null, int? vital = null, bool setFinal = true)
    {
        SetPointer(PointerType.Drama, drama, setFinal);
        SetPointer(PointerType.Energy, ability, setFinal);
        SetPointer(PointerType.Body, body, setFinal);
        SetPointer(PointerType.Will, will, setFinal);
        SetPointer(PointerType.Armor, armor, setFinal);
        SetPointer(PointerType.Barrier, barrier, setFinal);
        SetPointer(PointerType.Mana, mana, setFinal);
        SetPointer(PointerType.Vital, vital, setFinal);
    }

    public void SetPointer(PointerType type, int? value, bool setFinal = true)
    {
        if (value == null) return;

        var difference = 0;
        
        if (setFinal) difference = CorePointersMax[type] - FinalPointers.First(p => p.PointerType == type).Max;
            
        PointersCurrent[type] = value.GetValueOrDefault() + difference;
    }

    [JsonIgnore]
    public bool IsNearDeath => FinalPointers.Any(p => p.PointerType is PointerType.Body or PointerType.Will && p.Current == 0);
    
    [JsonIgnore]
    public IReadOnlyCollection<Pointer> FinalPointers
    {
        get
        {
            var result = new List<Pointer>();

            foreach (var pointer in CorePointers.Select(p => new Pointer(p.PointerType, p.Max, p.Current)))
            {
                switch (pointer.PointerType)
                {
                    case PointerType.Drama:
                    {
                        pointer.Current -= 3;
                        pointer.Max -= 3;
                        break;
                    }
                    case PointerType.Body:
                    {
                        var difference = 
                            Character.Attributes.CoreAttributes.First(a => a.AttributeType == AttributeType.End).Modifier 
                            - Character.Attributes.FinalAttributes.First(a => a.AttributeType == AttributeType.End).Modifier;
                        
                        pointer.Current -= difference;
                        pointer.Max -= difference;
                        
                        if (pointer.Max < 0)
                        {
                            pointer.Current -= pointer.Max;
                            pointer.Max = 0;
                        }
                        
                        break;
                    }
                    case PointerType.Will:
                    {
                        var difference = 
                            Character.Attributes.CoreAttributes.First(a => a.AttributeType == AttributeType.Det).Modifier 
                            - Character.Attributes.FinalAttributes.First(a => a.AttributeType == AttributeType.Det).Modifier;
                        
                        pointer.Current -= difference;
                        pointer.Max -= difference;
                        
                        if (pointer.Max < 0)
                        {
                            pointer.Current -= pointer.Max;
                            pointer.Max = 0;
                        }
                        
                        break;
                    }
                }

                foreach (var effect in Character.Effects.FinalEffects)
                {
                    effect.ModifyPointer(pointer);
                }
                
                result.Add(pointer);
            }

            return result;
        }
    }

    [JsonIgnore]
    public string Title => "Состояния";
    
    //TODO Выводить в описание травмы и присмерти ли персонаж
    
    [JsonIgnore]
    public List<IField> Fields => FinalPointers.Select(a => (IField) a).ToList();
    
    [JsonIgnore]
    public string Footer => Character.GetFooter;
    
    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            var valid = true;
            var errors = new List<string>();

            var errorPointers = FinalPointers.Where(p => p.Current > p.Max).ToList();

            if (errorPointers.Any())
            {
                valid = false;
                
                var attrJoin = String.Join(", ", 
                    errorPointers.Select(p => $"{p.Name} `{p.Current}/{p.Max}`"));
                
                errors.Add($"Значение счетчиков: {attrJoin} – не могут превышать максимальные.");
            }
            
            var errorNegatePointers = FinalPointers
                .Where(p => p.PointerType != PointerType.Drama && p.Current < 0).ToList();

            if (errorNegatePointers.Any())
            {
                valid = false;
                
                var attrJoin = String.Join(", ", 
                    errorNegatePointers.Select(p => $"{p.Name} `{p.Current}/{p.Max}`"));
                
                errors.Add($"Значение счетчиков: {attrJoin} – не могут быть меньше нуля.");
            }
            
            var negateDrama = FinalPointers
                .FirstOrDefault(p => p.PointerType == PointerType.Drama && p.Current < -3);

            if (negateDrama != null)
            {
                valid = false;
                
                errors.Add($"Значение очков драмы не может быть меньше -3.");
            }

            Errors = errors.ToArray();
            return valid;
        }
    }

    [JsonIgnore]
    public string[]? Errors { get; private set; }
}