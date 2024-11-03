using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;
using System.Security.Claims;

namespace Nursing.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;
    private readonly IInviteService _inviteService;

    public SyncController(ISyncService syncService, IInviteService inviteService)
    {
        _syncService = syncService;
        _inviteService = inviteService;
    }

    private Guid GetUserId()
    {
        var idClaim = User.Claims.First(c => c.Type == ClaimTypes.Sid);
        return Guid.Parse(idClaim.Value);
    }
    
    [HttpPost("sync")]
    [ProducesResponseType<SyncResult>(200)]
    public async Task<IActionResult> Sync([FromBody] SyncModel sync)
    {
        try
        {
            var result = await _syncService.SyncFeedings(sync, GetUserId());
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("invites")]
    [ProducesResponseType<List<InviteViewModel>>(200)]
    public async Task<IActionResult> GetInvites()
    {
        var invites = await _inviteService.GetInvites(User.Identity!.Name!);
        return Ok(invites);
    }

    [HttpPost("sendInvite")]
    public async Task<IActionResult> SendInvite([FromBody] string username)
    {
        await _inviteService.SendInvite(username, User.Identity!.Name!);
        return Ok();
    }

    [HttpPost("acceptInvite")]
    public async Task<IActionResult> AcceptInvite([FromBody] Guid id)
    {
        try
        {
            var result = await _inviteService.AcceptInvite(id, User.Identity!.Name!);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [HttpPost("declineInvite")]
    public async Task<IActionResult> DeclineInvite([FromBody] Guid id)
    {
        try
        {
            var result = await _inviteService.DeclineInvite(id, User.Identity!.Name!);
            return Ok(result);
        }
        catch (InvalidOperationException)
        {
            return BadRequest();
        }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] Guid[] ids)
    {
        var numberUpdated = await _syncService.DeleteFeedings(ids, GetUserId());
        return Ok(numberUpdated);
    }
}