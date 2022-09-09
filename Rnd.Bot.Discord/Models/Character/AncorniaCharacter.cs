﻿using Newtonsoft.Json;
using Rnd.Bot.Discord.Models.Character.Fields;
using Rnd.Bot.Discord.Models.Character.Panels;
using Rnd.Bot.Discord.Models.Glossaries;

namespace Rnd.Bot.Discord.Models.Character;

/// <summary>
/// Alias class for Character&#60;AncorniaDomainType, AncorniaSkillType&#62; generic
/// </summary>
public class AncorniaCharacter : Character<AncorniaDomainType, AncorniaSkillType>
{
    public AncorniaCharacter(ICharacter character, List<Domain<AncorniaDomainType, AncorniaSkillType>> domains) 
        : base(character, domains)
    { }

    [JsonConstructor]
    public AncorniaCharacter(string name, General general, Attributes attributes, Pointers pointers, Effects effects, Traumas traumas,
        Domains<AncorniaDomainType, AncorniaSkillType> domains, Backstory backstory)
        : base(name, general, attributes, pointers, effects, traumas, domains, backstory) 
    {}
}