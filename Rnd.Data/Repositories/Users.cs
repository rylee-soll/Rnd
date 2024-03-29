﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rnd.Models;
using Rnd.Results;

namespace Rnd.Data.Repositories;

public class Users : Repository<User>
{
    public Users(DataContext context, DbSet<User> data) : base(context, data) { }
    
    public async Task<Result<User>> GetAsync(Guid id)
    {
        return await GetAsync(u => u.Id == id);
    }
    
    public async Task<Result<User>> GetAsync(ulong id)
    {
        return await GetAsync(u => u.DiscordId == id);
    }
    
    public async Task<Result<User>> LoginAsync(string login, string password)
    {
        return await GetAsync(u => u.PasswordHash == Hash.GenerateStringHash(password)
                                   && (u.Login == login || u.Email == login));
    }
    
    private async Task<Result<User>> GetAsync(Expression<Func<User, bool>> predicate)
    {
        return Result
            .Found(
                await Data
                    .Include(u => u.Memberships)
                    .ThenInclude(m => m.Game)
                    .FirstOrDefaultAsync(predicate),
                "Пользователь",
                "Пользователь не найден")
            .OnSuccess(u => u.GetView());
    }

    public async Task<Result<User>> CreateAsync(User.Form form)
    {
        var validation = await Data.ValidateAsync("Ошибка валидации",
            new Rule<User>(u => form.Email != null && u.Email == form.Email, 
                "Пользователь с таким Email уже существует", 
                nameof(form.Email)),
            new Rule<User>(u => form.Login != null && u.Login == form.Login, 
                "Пользователь с таким логином уже существует", 
                nameof(form.Login)),
            new Rule<User>(u => form.DiscordId != null && u.DiscordId == form.DiscordId, 
                "К текущему аккаунту discordId уже привязан другой аккаунт RndId", 
                nameof(form.DiscordId)));

        if (!validation.IsValid) return Result.Fail<User>(validation.Message);
        
        var result = await User.New.TryCreateAsync(form);
        if (result.IsFailed) return result;
        
        Data.Add(result.Value);
        await Context.SaveChangesAsync();

        return result.OnSuccess(u => u.GetView());
    }
    
    public async Task<Result<User>> UpdateAsync(Guid userId, User.Form form)
    {
        var validation = await Data.ValidateAsync("Ошибка валидации",
            new Rule<User>(u => form.Email != null && u.Email == form.Email && u.Id != userId, 
                "Пользователь с таким Email уже существует", 
                nameof(form.Email)),
            new Rule<User>(u => form.Login != null && u.Login == form.Login && u.Id != userId, 
                "Пользователь с таким логином уже существует", 
                nameof(form.Login)),
            new Rule<User>(u => form.DiscordId != null && u.DiscordId == form.DiscordId && u.Id != userId, 
                "К текущему аккаунту discordId уже привязан другой аккаунт RndId", 
                nameof(form.DiscordId)));
        
        if (!validation.IsValid) return Result.Fail<User>(validation.Message);
        
        var result = await GetAsync(userId);
        if (result.IsFailed) return result;

        result.Update(await result.Value.TryUpdateAsync(form));
        if (result.IsFailed) return result;

        await Context.SaveChangesAsync();
        
        return result;
    }
    
    public async Task<Result<User>> BindDiscordAsync(Guid userId, ulong discordId)
    {
        return await UpdateAsync(userId, new User.Form(DiscordId: discordId));
    }
    
    public async Task<Result<User>> UnbindDiscordAsync(Guid userId)
    {
        var result = await GetAsync(userId);
        if (result.IsFailed) return result;

        var form = result.Value.GetForm();
        form.DiscordId = null;
        
        result.Update(await result.Value.TryClearAsync(form));
        if (result.IsFailed) return result;

        await Context.SaveChangesAsync();
        
        return result;
    }
}