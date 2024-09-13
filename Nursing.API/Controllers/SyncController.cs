using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;

namespace Nursing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly PostgresContext _context;
        public SyncController(PostgresContext context)
        {
            _context = context;
        }

        [HttpPost("sync")]
        [ProducesResponseType<SyncResult>(200)]
        public async Task<IActionResult> Sync([FromBody] SyncModel sync)
        {
            var lastSync = sync.LastSync;
            var feedings = sync.Feedings;

            if (User.Identity == null)
            {
                return Unauthorized();
            }

            try
            {
                var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);

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

                await _context.SaveChangesAsync();

                return Ok(new SyncResult { Success = true, Feedings = toSend, BadIds = badIds });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("invite")]
        [ProducesResponseType<Guid>(200)]
        public async Task<IActionResult> Invite(string userName)
        {
            if (User.Identity == null)
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (currentUser == null || invitedUser == null || User.Identity.Name == userName)
            {
                return BadRequest();
            }

            var invite = new Invite
            {
                GroupId = currentUser.GroupId,
                UserId = invitedUser.Id
            };

            _context.Invites.Add(invite);
            await _context.SaveChangesAsync();

            return Ok(invite.GroupId);
        }

        [HttpGet("acceptInvite/{id}")]
        public async Task<IActionResult> AcceptInvite(Guid id)
        {
            if (User.Identity == null)
            {
                return Unauthorized();
            }
            
            var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);
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
    }
}