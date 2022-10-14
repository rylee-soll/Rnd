﻿using Microsoft.AspNetCore.Mvc;
using Rnd.Api.Client.Models.Basic.Game;

namespace Rnd.Api.Controllers.Basic.Users;

[ApiController]
[Route("basic/users/{userId:guid}/[controller]")]
public class GamesController : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<ActionResult<GameModel>> Get(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet]
    public Task<ActionResult<List<GameModel>>> List(Guid userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPost]
    public Task<ActionResult<GameModel>> Create(Guid userId, GameFormModel form)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("{id:guid}")]
    public Task<ActionResult<GameModel>> Edit(Guid userId, Guid id, GameFormModel form)
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete("{id:guid}")]
    public Task<ActionResult<GameModel>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}