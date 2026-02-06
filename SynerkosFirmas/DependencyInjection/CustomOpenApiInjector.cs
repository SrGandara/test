using Microsoft.OpenApi;

namespace SynerkosFirmas.DependencyInjection
{
    public static class CustomOpenApiInjector
    {
        public static void AddCustomOpenApi(this WebApplicationBuilder builder)
        {
            var azureAd = builder.Configuration.GetSection("AzureAd");
            var tenantId = azureAd["TenantId"];
            var backendClientId = azureAd["ClientId"];
            var scopeName = azureAd["Scopes"];

            var fullScope = $"api://{backendClientId}/{scopeName}";

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    document.Components.SecuritySchemes.Add("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            AuthorizationCode = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize"),
                                TokenUrl = new Uri($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { fullScope, "Access the API as User" },
                                }
                            }
                        }
                    });

                    document.Security = [
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecuritySchemeReference("oauth2"),
                                [fullScope]
                            }
                        }
                    ];

                    document.SetReferenceHostDocument();

                    return Task.CompletedTask;
                });
            });
        }
    }
}