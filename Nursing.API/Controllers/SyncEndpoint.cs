using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Nursing.API.Models;

namespace Nursing.API.Endpoints;

public class SyncRequest
{
    public DateTime LastSync { get; set; }
    public List<Feeding> Feedings { get; set; } = null!;
}

public class SyncResponse
{
    public List<Feeding> Feedings { get; set; } = null!;
}

public class Sync : Endpoint<SyncRequest, Results<Ok<SyncResponse>, BadRequest>>
{
    public INursingContext Context { get; set; } = null!;
    public override void Configure()
    {
        Post("/sync");

    }

    public override async Task<Results<Ok<SyncResponse>, BadRequest>> ExecuteAsync(SyncRequest req, CancellationToken ct)
    {
        foreach (var feeding in req.Feedings)
        {
            if (feeding.Deleted.HasValue)
            {
                var existing = await Context.Feedings.FindAsync(feeding.Id);
                if (existing != null)
                {
                    Context.Feedings.Remove(existing);
                }
            }
            else
            {
                if (feeding.LastUpdated == feeding.Created)
                {
                    Context.Feedings.Add(feeding);
                }
                else
                {
                    var existing = await Context.Feedings.FindAsync(feeding.Id);
                    if (existing != null)
                    {
                        Context.Feedings.Entry(existing).CurrentValues.SetValues(feeding);
                    }
                }
            }
        }
        var feedings = await Context.Feedings.Where(x => x.LastUpdated > req.LastSync).ToListAsync(ct);

        return TypedResults.Ok(new SyncResponse
        {
            Feedings = feedings
        });
    }
}