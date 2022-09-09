﻿using Newtonsoft.Json;
using Rnd.Bot.Discord.Models.Common;
using Rnd.Bot.Discord.Views;

namespace Rnd.Bot.Discord.Models.Character.Panels;

public class General : IPanel, IValidatable
{
    public General(ICharacter character, string? description = null,
        string? culture = null, string? age = null, 
        IEnumerable<string>? ideals = null, IEnumerable<string>? vices = null, IEnumerable<string>? traits = null)
    {
        Character = character;

        Description = description;
        Culture = new TextField<string?>("Культура", culture);
        Age = new TextField<string?>("Возраст", age);
        
        Ideals = new ListField("Идеалы", ideals);
        Vices = new ListField("Пороки", vices);
        Traits = new ListField("Черты", traits);
    }

    [JsonConstructor]
    public General(ICharacter character, string? description, TextField<string?> culture, TextField<string?> age, 
        ListField ideals, ListField vices, ListField traits)
    {
        Character = character;
        Description = description;
        Culture = culture;
        Age = age;
        Ideals = ideals;
        Vices = vices;
        Traits = traits;
    }

    [JsonIgnore]
    public ICharacter Character { get; }
    
    public string? Description { get; set; }
    public TextField<string?> Culture { get; }
    public TextField<string?> Age { get; }
    public ListField Ideals { get; }
    public ListField Vices { get; }
    public ListField Traits { get; }

    [JsonIgnore]
    public string Title => Character.GetFooter;

    [JsonIgnore]
    public List<IField> Fields
    {
        get
        {
            var result = new List<IField>();

            if (Culture.TValue != null) result.Add(Culture);
            if (Age.TValue != null) result.Add(Age);
            
            if (Ideals.Values?.Count > 0) result.Add(Ideals);
            if (Vices.Values?.Count > 0) result.Add(Vices);
            if (Traits.Values?.Count > 0) result.Add(Traits);
                
            return result;
        }
    }

    [JsonIgnore]
    public bool IsValid
    {
        get
        {
            var valid = true;
            var errors = new List<string>();

            if (Age.Value != null && !Int32.TryParse(Age.TValue, out int _))
            {
                valid = false;
                errors.Add("Возраст должен быть записан цифрой.");
            }

            Errors = errors.ToArray();
            return valid;
        }
    }

    [JsonIgnore]
    public string[]? Errors { get; private set; }
}