using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Nursing.API.Endpoints;

public class UserInformationResponse
{
    public string Email { get; set; } = null!;
}

public class UserInformation : EndpointWithoutRequest<Results<Ok<UserInformationResponse>, UnauthorizedHttpResult>>
{
    public override void Configure()
    {
        Get("/userinfo");
    }

    public override async Task<Results<Ok<UserInformationResponse>, UnauthorizedHttpResult>> ExecuteAsync(CancellationToken ct)
    {
        await Task.CompletedTask;
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (User.Identity == null || email == null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(new UserInformationResponse
        {
            Email = email
        });
    }
}