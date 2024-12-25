using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.API.Tests.Utilities;
using Nursing.Core.Models;
using Xunit;

namespace Nursing.API.Tests.Services;

public class InviteServiceTests : IDisposable
{
    private readonly PostgresContext _context;
    private readonly IInviteService _inviteService;
    private readonly NursingUser _currentUser;
    private readonly NursingUser _invitedUser;

    public InviteServiceTests()
    {
        _context = TestDbContext.Create();
        _inviteService = new InviteService(_context);

        // Setup test users
        _currentUser = new NursingUser
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid(),
            UserName = "current@test.com",
            // RefreshTokens = new List<RefreshToken>()
        };

        _invitedUser = new NursingUser
        {
            Id = Guid.NewGuid().ToString(),
            GroupId = Guid.NewGuid(),
            UserName = "invited@test.com",
            // RefreshTokens = new List<RefreshToken>()
        };

        _context.Users.AddRange(_currentUser, _invitedUser);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SendInvite_ShouldCreateInvite()
    {
        // Act
        await _inviteService.SendInvite(_invitedUser.UserName!, _currentUser.UserName!);

        // Assert
        var invite = await _context.Invites.FindAsync(_currentUser.GroupId);
        Assert.NotNull(invite);
    }

    [Fact]
    public async Task GetInvites_ShouldReturnUserInvites()
    {
        // Arrange
        _context.Invites.Add(new Invite
        {
            GroupId = _currentUser.GroupId,
            UserId = _invitedUser.Id
        });
        await _context.SaveChangesAsync();

        // Act
        var invites = await _inviteService.GetInvites(_invitedUser.UserName!);

        // Assert
        Assert.Single(invites);
        Assert.Equal(_currentUser.GroupId, invites[0].Id);
    }

    [Fact]
    public async Task AcceptInvite_ShouldUpdateUserGroup()
    {
        // Arrange
        var invite = new Invite
        {
            GroupId = _currentUser.GroupId,
            UserId = _invitedUser.Id
        };
        _context.Invites.Add(invite);
        await _context.SaveChangesAsync();

        // Act
        await _inviteService.AcceptInvite(_currentUser.GroupId, _invitedUser.UserName!);

        // Assert
        var updatedUser = await _context.Users.FindAsync(_invitedUser.Id);
        Assert.Equal(_currentUser.GroupId, updatedUser!.GroupId);
        Assert.Null(await _context.Invites.FindAsync(_currentUser.GroupId));
    }

    [Fact]
    public async Task DeclineInvite_ShouldRemoveInvite()
    {
        // Arrange
        var invite = new Invite
        {
            GroupId = _currentUser.GroupId,
            UserId = _invitedUser.Id
        };
        _context.Invites.Add(invite);
        await _context.SaveChangesAsync();

        // Act
        await _inviteService.DeclineInvite(_currentUser.GroupId, _invitedUser.UserName!);

        // Assert
        Assert.Null(await _context.Invites.FindAsync(_currentUser.GroupId));
    }

    public void Dispose()
    {
        TestDbContext.Destroy(_context);
    }
}
