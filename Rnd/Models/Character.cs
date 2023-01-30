﻿using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using FluentValidation;
using Rnd.Constants;
using Rnd.Core;

// EF Proxies
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
#pragma warning disable CS8618

namespace Rnd.Models;

public class Character : ValidatableModel<Character, Character.Form, Character.UpdateValidator, Character.ClearValidator>
{
    public virtual Member Owner { get; protected set; }
    public virtual Module Module { get; protected set; }
    public virtual List<Token> Tokens { get; protected set; } = new();
    
    [MaxLength(TextSize.Tiny)]
    public string Name { get; protected set; }
    
    [MaxLength(TextSize.Small)]
    public string? Title { get; protected set; }
    
    [MaxLength(TextSize.Medium)]
    public string? Description { get; protected set; }
    
    [MaxLength(TextSize.Tiny)]
    public string? ColorHtml { get; protected set; }
    
    public DateTimeOffset Created { get; protected set; }
    public DateTimeOffset Selected { get; protected set; }
    
    #region Navigation
    
    public Guid OwnerId { get; protected set; }
    public Guid ModuleId { get; protected set; }

    #endregion

    #region Factories

    protected Character(
        Guid ownerId,
        Guid moduleId,
        string name,
        string? title,
        string? description,
        string? colorHtml
    )
    {
        OwnerId = ownerId;
        ModuleId = moduleId;
        Name = name;
        Title = title;
        Description = description;
        ColorHtml = colorHtml;
        Created = Time.Now;
        Selected = Time.Zero;
    }

    public class Factory : ValidatingFactory<Character, Form, CreateValidator>
    {
        public override Character Create(Form form)
        {
            Guard.Against.Null(form.OwnerId);
            Guard.Against.Null(form.ModuleId);
            Guard.Against.Null(form.Name);
            
            return new Character(form.OwnerId, form.ModuleId, form.Name, form.Title, form.Description, form.ColorHtml);
        }
    }

    public static Factory New => new();

    #endregion

    #region Updaters

    public override Character Update(Form form)
    {
        return this;
    }

    public override Character Clear(Form form)
    {
        return this;
    }
    
    #endregion

    #region Validators

    public class UpdateValidator : AbstractValidator<Form>
    {
        public UpdateValidator()
        {
            
        }
    }

    public class CreateValidator : UpdateValidator
    {
        public CreateValidator()
        {
            
        }
    }

    public class ClearValidator : AbstractValidator<Form>
    {
        public ClearValidator()
        {
            
        }
    }

    #endregion

    #region Views

    public record struct Form(
        Guid OwnerId,
        Guid ModuleId,
        string Name,
        string? Title,
        string? Description,
        string? ColorHtml
    );
    
    public readonly record struct View(
        Guid _id
    );

    public View GetView()
    {
        return new View(
            Id
        );
    }

    #endregion
}