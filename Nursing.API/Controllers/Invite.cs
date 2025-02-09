using System.Security.Claims;
using FastEndpoints;
using Nursing.API.Models;

namespace Nursing.API.Endpoints;

public sealed class InviteEndpoint : Endpoint<InviteRequest>
{
    public INursingContext Context { get; set; } = null!;
    public override void Configure()
    {
        Post("/invite");
    }

    public override async Task HandleAsync(InviteRequest req, CancellationToken ct)
    {
        Context.Invites.Add(new (){
            Accepted = false,
            UserId1 = User.FindFirstValue(ClaimTypes.Email)!,
            UserId2 = req.Email,
        });

        await Context.SaveChangesAsync(ct);

        Response = TypedResults.Ok();
    }
}
