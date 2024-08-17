using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.Core.Models.DTO;

namespace Nursing.API.Controllers
{
    [Authorize]
    public class SyncController : ControllerBase
    {
        private readonly SqlContext _context;
        public SyncController(SqlContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Sync(DateTime lastSync, FeedingDto[] feedings)
        {
            if (User.Identity == null)
            {
                return Unauthorized();
            }

            try
            {
                var currentUser = await _context.Users.FirstAsync(u => u.UserName == User.Identity.Name);

                var toSend = await _context.Feedings
                    .Where(f => f.LastUpdated > lastSync && f.GroupId == currentUser.GroupId)
                    .ToListAsync();

                var ids = feedings.Select(x => x.Id).ToList();

                var badIds = await _context.Feedings.Where(f => ids.Contains(f.Id) && f.GroupId != currentUser.GroupId)
                    .Select(x => x.Id)
                    .ToListAsync();

                _context.Feedings.UpdateRange(feedings
                    .Where(x => !badIds.Contains(x.Id))
                    .Select(x => new Feeding(x, currentUser.GroupId)));

                await _context.SaveChangesAsync();

                return Ok(new { success = true, feedings = toSend, badIds });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> Invite(string userName)
        {
            if (User.Identity == null)
            {
                return Unauthorized();
            }

            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            var invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (currentUser == null || invitedUser == null)
            {
                return BadRequest();
            }

            var invite = new Invite
            {
                GroupId = currentUser.GroupId,
                UserId = invitedUser.Id
            };

            _context.Invites.Add(invite);

            return Ok();
        }

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

            return Ok();
        }
    }
}
