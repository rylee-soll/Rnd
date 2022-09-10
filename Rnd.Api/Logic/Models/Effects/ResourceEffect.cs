﻿using Rnd.Api.Logic.Helpers;
using Rnd.Api.Logic.Localization;
using Rnd.Api.Logic.Models.Resources;

namespace Rnd.Api.Logic.Models.Effects;

public class ResourceEffect : IResourceEffect
{
    public ResourceEffect(Guid id, string resourceName)
    {
        Id = id;
        ResourceName = resourceName;
    }

    public Guid Id { get; }
    public string? ResourcePath { get; set; }
    public string ResourceName { get; set; }
    
    public decimal? ValueModifier { get; set; }
    public decimal? MinModifier { get; set; }
    public decimal? MaxModifier { get; set; }
    
    public IResource Modify(IResource resource)
    {
        resource.Value += ValueModifier.GetValueOrDefault();
        resource.Min += MinModifier.GetValueOrDefault();
        resource.Max += MaxModifier.GetValueOrDefault();
        return resource;
    }
    
    #region IStorable

    private Guid EffectId { get; set; }

    public void Save(Data.Entities.ResourceEffect entity)
    {
        if (entity.Id != Id) throw new InvalidOperationException(Lang.Exceptions.IStorable.DifferentIds);
        
        entity.ResourceFullname = PathHelper.Combine(ResourcePath, ResourceName);
        entity.ValueModifier = ValueModifier;
        entity.MinModifier = MinModifier;
        entity.MaxModifier = MaxModifier;
        entity.EffectId = EffectId;
    }

    public void Load(Data.Entities.ResourceEffect entity)
    {
        if (Id != entity.Id) throw new InvalidOperationException(Lang.Exceptions.IStorable.DifferentIds);

        ResourcePath = PathHelper.GetPath(entity.ResourceFullname);
        ResourceName = PathHelper.GetName(entity.ResourceFullname);
        ValueModifier = entity.ValueModifier;
        MinModifier = entity.MinModifier;
        MaxModifier = entity.MaxModifier;
        EffectId = entity.EffectId;
    }

    #endregion
}