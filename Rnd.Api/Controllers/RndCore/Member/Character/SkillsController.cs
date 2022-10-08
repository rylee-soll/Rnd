﻿using Microsoft.AspNetCore.Mvc;
using Rnd.Api.Client.Models.RndCore.Skills;

namespace Rnd.Api.Controllers.RndCore.Member.Character;

[ApiController]
[Route("rndcore/member/{memberId:guid}/character/{characterId:guid}/[controller]")]
public class SkillsController : ControllerBase
{
    [HttpGet]
    public Task<ActionResult<SkillsModel>> Show(Guid memberId, Guid characterId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut]
    public Task<ActionResult<SkillsModel>> Set(Guid memberId, Guid characterId, SkillsSetModel set)
    {
        throw new NotImplementedException();
    }
}