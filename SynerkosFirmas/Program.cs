using SynerkosFirmas.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.AddCustomJWTAuthentication();
builder.Services.AddAuthorization();
builder.AddCustomOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
        options.OAuthClientId("6f705c70-47e4-45d3-88cc-c264919e2338");
        options.OAuthUsePkce();
        options.OAuthScopeSeparator(" ");
    });
    Console.WriteLine("Opened swagger for use!");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
