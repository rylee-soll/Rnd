﻿using Newtonsoft.Json;
using Rnd.Api.Data.Entities;
using Rnd.Api.Logic.Helpers;
using Rnd.Api.Logic.Localization;
using Rnd.Api.Logic.Models.Parameters;

namespace Rnd.Api.Logic.Models.Effects;

public abstract class ParameterEffect<T> : IParameterEffect where T : notnull
{
    protected ParameterEffect(Guid id, string parameterName, T modifier)
    {
        Id = id;
        ParameterName = parameterName;
        Modifier = modifier;
    }

    public Guid Id { get; }
    public virtual string? ParameterPath { get; protected set; }
    public string ParameterName { get; private set; }
    public T Modifier { get; set; }

    #region IParameterEffect
    
    object IParameterEffect.Modifier
    {
        get => Modifier;
        set => Modifier = (T) value;
    }

    public abstract IParameter Modify(IParameter parameter);

    #endregion
    
    #region IStorable
    
    private Guid EffectId { get; set; }

    public void Save(ParameterEffect entity)
    {
        if (entity.Id != Id) throw new InvalidOperationException(Lang.Exceptions.IStorable.DifferentIds);
        
        entity.ParameterFullname = PathHelper.Combine(ParameterPath, ParameterName);
        entity.ModifierJson = JsonConvert.SerializeObject(Modifier);
        entity.EffectId = EffectId;
    }

    public void Load(ParameterEffect entity)
    {
        if (Id != entity.Id) throw new InvalidOperationException(Lang.Exceptions.IStorable.DifferentIds);

        EffectId = entity.EffectId;
        ParameterPath = PathHelper.GetPath(entity.ParameterFullname);
        ParameterName = PathHelper.GetName(entity.ParameterFullname);
        Modifier = JsonConvert.DeserializeObject<T>(entity.ModifierJson) 
                   ?? throw new InvalidOperationException(Lang.Exceptions.JsonNullError);
    }

    #endregion
}