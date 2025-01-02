using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

internal sealed class CookieSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.Any(authScheme => authScheme.Name == IdentityConstants.ApplicationScheme))
        {
            var cookieSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Cookie authorization.",
                In = ParameterLocation.Cookie,
                BearerFormat = "Tokean",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Auth"
            };
            var requirements = new Dictionary<string, OpenApiSecurityScheme>
            {
                ["Cookie"] = cookieSecurityScheme
            };
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes = requirements;

            // Apply it as a requirement for all operations
            foreach (var operation in document.Paths.Where(x => x.Key.StartsWith("/api")).SelectMany(path => path.Value.Operations))
            {
                operation.Value.Security.Add(new OpenApiSecurityRequirement
                {
                    [cookieSecurityScheme] = ["Cookie"]
                });
            }
        }
    }
}