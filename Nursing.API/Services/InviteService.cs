using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;

namespace Nursing.API.Services;

public interface IInviteService
{
    Task<List<InviteViewModel>> GetInvites(string username);
    Task SendInvite(string username, string currentUsername);
    Task<Guid> AcceptInvite(Guid id, string username);
    Task<Guid> DeclineInvite(Guid id, string username);
}

public class InviteService : IInviteService
{
    private readonly PostgresContext _context;

    public InviteService(PostgresContext context)
    {
        _context = context;
    }

    public async Task<List<InviteViewModel>> GetInvites(string username)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == username);

        return await _context.Invites
            .Where(i => i.UserId == currentUser.Id)
            .GroupJoin(_context.Users, i => i.GroupId, u => u.GroupId, (i, u) => new InviteViewModel
            {
                Id = i.GroupId,
                Users = u.Select(x => x.UserName!).ToList()
            })
            .ToListAsync();
    }

    public async Task SendInvite(string username, string currentUsername)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == currentUsername);
        var invitedUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        if (invitedUser == null) return;

        if (await _context.Invites.AnyAsync(x => x.UserId == invitedUser.Id && x.GroupId == currentUser.GroupId))
        {
            return;
        }

        _context.Invites.Add(new()
        {
            GroupId = currentUser.GroupId,
            UserId = invitedUser.Id
        });
        await _context.SaveChangesAsync();
    }

    public async Task<Guid> AcceptInvite(Guid id, string username)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == username);
        var invite = await _context.Invites.FirstOrDefaultAsync(i => i.UserId == currentUser.Id && i.GroupId == id);
        
        if (invite == null) throw new InvalidOperationException("Invite not found");

        currentUser.GroupId = invite.GroupId;
        _context.Invites.Remove(invite);
        await _context.SaveChangesAsync();

        return id;
    }

    public async Task<Guid> DeclineInvite(Guid id, string username)
    {
        var currentUser = await _context.Users.FirstAsync(u => u.UserName == username);
        var invite = await _context.Invites.FirstOrDefaultAsync(i => i.UserId == currentUser.Id && i.GroupId == id);
        
        if (invite == null) throw new InvalidOperationException("Invite not found");

        _context.Invites.Remove(invite);
        await _context.SaveChangesAsync();
        return id;
    }
}
