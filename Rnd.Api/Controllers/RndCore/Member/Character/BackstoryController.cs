﻿using Microsoft.AspNetCore.Mvc;
using Rnd.Api.Client.Models.RndCore.Backstory;

namespace Rnd.Api.Controllers.RndCore.Member.Character;

[ApiController]
[Route("rndcore/member/{memberId:guid}/character/{characterId:guid}/[controller]")]
public class BackstoryController : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<BackstoryModel>> Show(Guid memberId, Guid characterId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut]
    public Task<ActionResult<BackstoryModel>> Set(Guid memberId, Guid characterId, BackstorySetModel set)
    {
        throw new NotImplementedException();
    }
}