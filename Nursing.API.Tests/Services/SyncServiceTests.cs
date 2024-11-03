using Nursing.API.Models;
using Nursing.API.Services;
using Nursing.API.Tests.Utilities;
using Nursing.Core.Models;
using Nursing.Core.Models.DTO;
using Xunit;

namespace Nursing.API.Tests.Services;

public class SyncServiceTests : IDisposable
{
    private readonly PostgresContext _context;
    private readonly ISyncService _syncService;
    private readonly Guid _userId;
    private readonly Guid _groupId;

    public SyncServiceTests()
    {
        _context = TestDbContext.Create();
        _syncService = new SyncService(_context);
        _userId = Guid.NewGuid();
        _groupId = Guid.NewGuid();

        // Setup test user
        _context.Users.Add(new NursingUser
        {
            Id = _userId,
            GroupId = _groupId,
            UserName = "test@test.com",
            RefreshTokens = new List<RefreshToken>()
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task SyncFeedings_ShouldInsertNewFeedings()
    {
        // Arrange
        var feeding = new FeedingDto
        {
            Id = Guid.NewGuid(),
            LeftBreastTotal = TimeSpan.FromMinutes(5),
            RightBreastTotal = TimeSpan.FromMinutes(5),
            TotalTime = TimeSpan.FromMinutes(10),
            Started = DateTime.UtcNow,
            Finished = DateTime.UtcNow.AddMinutes(10),
            LastIsLeft = true,
            LastUpdated = DateTime.UtcNow
        };

        var sync = new SyncModel
        {
            LastSync = DateTime.UtcNow.AddHours(-1),
            Feedings = [feeding]
        };

        // Act
        var result = await _syncService.SyncFeedings(sync, _userId);

        // Assert
        Assert.True(result.Success);
        Assert.Empty(result.BadIds);
        Assert.Equal(1, result.Updates);

        var savedFeeding = await _context.Feedings.FindAsync(feeding.Id);
        Assert.NotNull(savedFeeding);
        Assert.Equal(_groupId, savedFeeding.GroupId);
    }

    [Fact]
    public async Task DeleteFeedings_ShouldMarkFeedingsAsDeleted()
    {
        // Arrange
        var feedingId = Guid.NewGuid();
        _context.Feedings.Add(new Feeding
        {
            Id = feedingId,
            GroupId = _groupId,
            LeftBreastTotal = TimeSpan.FromMinutes(5),
            RightBreastTotal = TimeSpan.FromMinutes(5),
            TotalTime = TimeSpan.FromMinutes(10),
            Started = DateTime.UtcNow,
            Finished = DateTime.UtcNow.AddMinutes(10),
            LastIsLeft = true,
            LastUpdated = DateTime.UtcNow
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _syncService.DeleteFeedings(new[] { feedingId }, _userId);

        // Assert
        Assert.Equal(1, result);
        var deletedFeeding = await _context.Feedings.FindAsync(feedingId);
        Assert.NotNull(deletedFeeding?.Deleted);
    }

    public void Dispose()
    {
        TestDbContext.Destroy(_context);
    }
}
