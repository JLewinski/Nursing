using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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

    private string GetUserId()
    {
        var idClaim = User.Claims.First(c => c.Type == ClaimTypes.Sid);
        return idClaim.Value;
    }
    
    [HttpPost("sync", Name = "Sync")]
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

    [HttpGet("invites", Name = "GetInvites")]
    [ProducesResponseType<List<InviteViewModel>>(200)]
    public async Task<IActionResult> GetInvites()
    {
        var invites = await _inviteService.GetInvites(User.Identity!.Name!);
        return Ok(invites);
    }

    [HttpPost("sendInvite", Name = "SendInvite")]
    public async Task<IActionResult> SendInvite([FromBody] string username)
    {
        await _inviteService.SendInvite(username, User.Identity!.Name!);
        return Ok();
    }

    [HttpPost("acceptInvite", Name = "AcceptInvite")]
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

    [HttpPost("declineInvite", Name = "DeclineInvite")]
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

    [HttpPost("delete", Name = "Delete")]
    public async Task<IActionResult> Delete([FromBody] Guid[] ids)
    {
        var numberUpdated = await _syncService.DeleteFeedings(ids, GetUserId());
        return Ok(numberUpdated);
    }
}