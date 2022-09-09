﻿using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Rnd.Api.Data.Entities;

[Index(nameof(Login), IsUnique = true)]
public class User
{
    public User(Guid id, string login, string passwordHash)
    {
        Id = id;
        Login = login;
        PasswordHash = passwordHash;
    }

    public Guid Id { get; set; } 
    
    [Required]
    public string Login { get; set; }
    
    [Required]
    public string PasswordHash { get; set; }
}