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
    private readonly PostgresContext _context;
    public SyncController(PostgresContext context)
    {
        _context = context;
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
        var lastSync = sync.LastSync;
        var feedings = sync.Feedings;

        try
        {
            var currentUser = await _context.Users.FirstAsync(u => u.Id == GetUserId());

            var toSend = await _context.Feedings
                .Where(f => f.LastUpdated > lastSync && f.GroupId == currentUser.GroupId)
                .Select(f => new FeedingDto(f))
                .ToListAsync();

            var ids = feedings.Select(x => x.Id).ToList();

            var existing = await _context.Feedings.Where(f => ids.Contains(f.Id))
                .Select(x => new { x.Id, x.GroupId })
                .ToListAsync();

            var badIds = existing
                .Where(x => x.GroupId != currentUser.GroupId)
                .Select(x => x.Id)
                .ToList();

            var existingIds = existing.Where(x => x.GroupId == currentUser.GroupId).Select(x => x.Id).ToList();

            var updateList = feedings
                .Where(x => existingIds.Contains(x.Id))
                .Select(x => new Feeding(x, currentUser.GroupId))
                .ToList();

            var insertList = feedings
                .Where(x => !existingIds.Contains(x.Id) && !badIds.Contains(x.Id))
                .Select(x => new Feeding(x, currentUser.GroupId))
                .ToList();

            _context.ChangeTracker.Clear();

            _context.Feedings.UpdateRange(updateList);

            _context.Feedings.AddRange(insertList);

            var numChanged = await _context.SaveChangesAsync();

            return Ok(new SyncResult { Success = true, Feedings = toSend, BadIds = badIds, Updates = numChanged });
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
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity!.Name);

        var invites = await _context.Invites
            .Where(i => i.UserId == currentUser.Id)
            .GroupJoin(_context.Users, i => i.GroupId, u => u.GroupId, (i, u) => new InviteViewModel
            {
                Id = i.GroupId,
                Users = u.Select(x => x.UserName!).ToList()
            })
            .ToListAsync();

        return Ok(invites);
    }

    [HttpPost("sendInvite")]
    public async Task<IActionResult> SendInvite([FromBody] string username)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity!.Name);
        var invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (invitedUser == null)
        {
            //return Ok here to prevent users from knowing if a user exists
            return Ok();
        }

        if(await _context.Invites.AnyAsync(x => x.UserId == invitedUser.Id && x.GroupId == currentUser.GroupId))
        {
            return Ok();
        }

        _context.Invites.Add(new()
        {
            GroupId = currentUser.GroupId,
            UserId = invitedUser.Id
        });
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("acceptInvite")]
    public async Task<IActionResult> AcceptInvite([FromBody] Guid id)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity!.Name);
        var invite = await _context.Invites.FirstOrDefaultAsync(i => i.UserId == currentUser.Id && i.GroupId == id);
        if (currentUser == null || invite == null)
        {
            return BadRequest();
        }

        currentUser.GroupId = invite.GroupId;
        _context.Invites.Remove(invite);
        await _context.SaveChangesAsync();

        return Ok(id);
    }

    [HttpPost("declineInvite")]
    public async Task<IActionResult> DeclineInvite([FromBody] Guid id)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity!.Name);
        var invite = await _context.Invites.FirstOrDefaultAsync(i => i.UserId == currentUser.Id && i.GroupId == id);
        if (currentUser == null || invite == null)
        {
            return BadRequest();
        }

        _context.Invites.Remove(invite);
        await _context.SaveChangesAsync();
        return Ok(id);
    }
}