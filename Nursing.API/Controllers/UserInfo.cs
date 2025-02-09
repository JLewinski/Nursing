using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Nursing.API.Endpoints;

public class UserInformationEndpoint : EndpointWithoutRequest<Results<Ok<UserInformationResponse>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/userinfo");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Task.CompletedTask;

        var email = User.FindFirstValue(ClaimTypes.Email);
        if (User.Identity == null || email == null)
        {
            Response = TypedResults.Unauthorized();
            return;
        }

        Response = TypedResults.Ok(new UserInformationResponse
        {
            Email = email
        });
    }
}