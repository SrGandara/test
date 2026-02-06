namespace SynerkosFirmas.DependencyInjection
{
    public static class CustomJWTAuthentication
    {
        public static void AddCustomJWTAuthentication(this WebApplicationBuilder builder)
        {
            var azureAd = builder.Configuration.GetSection("AzureAd");
            var authority = $"{azureAd["Instance"]}/{azureAd["TenantId"]}/v2.0";
            var audience = $"api://{azureAd["ClientId"]}";

            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = authority;
                    options.Audience = azureAd["ClientId"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new()
                    {
                        NameClaimType = "preferred_username",
                        RoleClaimType = "roles",
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
        }
    }
}