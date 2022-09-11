﻿using Rnd.Api.Modules.Basic.Resources;

namespace Rnd.Api.Modules.RndCore.Resources;

public class State : StrictResource
{
    public State(StateType stateType, decimal max, decimal? value = null) : base(stateType.ToString())
    {
        Min = 0;
        Max = max;
        Value = value ?? Max;
        StateType = stateType;
    }
    
    public StateType StateType { get; }
    public override string Path => nameof(State);
}