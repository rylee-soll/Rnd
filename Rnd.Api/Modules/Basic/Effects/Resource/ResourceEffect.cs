﻿using Rnd.Api.Data;
using Rnd.Api.Helpers;
using Rnd.Api.Localization;
using Rnd.Api.Modules.Basic.Resources;

namespace Rnd.Api.Modules.Basic.Effects.Resource;

public class ResourceEffect : IResourceEffect
{
    public ResourceEffect(string resourceName)
    {
        ResourceName = resourceName;
        
        Id = Guid.NewGuid();
    }

    public Guid Id { get; }
    public Guid EffectId { get; set; }
    public string? ResourcePath { get; set; }
    public string ResourceName { get; set; }
    
    public decimal? ValueModifier { get; set; }
    public decimal? MinModifier { get; set; }
    public decimal? MaxModifier { get; set; }
    
    public TResource Modify<TResource>(TResource resource) where TResource : IResource
    {
        resource.Value += ValueModifier.GetValueOrDefault();
        resource.Min += MinModifier.GetValueOrDefault();
        resource.Max += MaxModifier.GetValueOrDefault();
        return resource;
    }
    
    #region IStorable

    public IStorable<Data.Entities.ResourceEffect> AsStorable => this;

    public Data.Entities.ResourceEffect? Save(Data.Entities.ResourceEffect? entity)
    {
        entity ??= new Data.Entities.ResourceEffect {Id = Id};
        if (AsStorable.NotSave(entity)) return null;
        
        entity.ResourceFullname = PathHelper.Combine(ResourcePath, ResourceName);
        entity.ValueModifier = ValueModifier;
        entity.MinModifier = MinModifier;
        entity.MaxModifier = MaxModifier;
        entity.EffectId = EffectId;
        
        return entity;
    }

    public void Load(Data.Entities.ResourceEffect entity)
    {
        if (AsStorable.NotLoad(entity)) return;

        ResourcePath = PathHelper.GetPath(entity.ResourceFullname);
        ResourceName = PathHelper.GetName(entity.ResourceFullname);
        ValueModifier = entity.ValueModifier;
        MinModifier = entity.MinModifier;
        MaxModifier = entity.MaxModifier;
        EffectId = entity.EffectId;
    }

    #endregion
}