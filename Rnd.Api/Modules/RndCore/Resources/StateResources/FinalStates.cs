﻿using Rnd.Api.Modules.RndCore.Characters;

namespace Rnd.Api.Modules.RndCore.Resources.StateResources;

public class FinalStates : States
{
    public FinalStates(Character character) : base(character, character.Final.Attributes, character.Leveling)
    {
        Character = character;
    }
    
    public Character Character { get; }

    //TODO Modify by all method
    public override State Body => Character.Effects
        .Aggregate(new FinalState(Character, base.Body), (attribute, effect) => effect.ModifyResource(attribute));
    public override State Will => Character.Effects
        .Aggregate(new FinalState(Character, base.Body), (attribute, effect) => effect.ModifyResource(attribute));
    public override State Armor => Character.Effects
        .Aggregate(new FinalState(Character, base.Body), (attribute, effect) => effect.ModifyResource(attribute));
    public override State Barrier => Character.Effects
        .Aggregate(new FinalState(Character, base.Body), (attribute, effect) => effect.ModifyResource(attribute));
    public override State Energy => Character.Effects
        .Aggregate(new FinalState(Character, base.Body), (attribute, effect) => effect.ModifyResource(attribute));
}