namespace KotKonnect.Api.EndPoints;

using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public static class AuthRoutes
{
    public static WebApplication AddAuthRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/auth").WithTags("Auth");

        group.MapPost("register", async (RegisterRequest request, IAuthUseCases auth) =>
            Results.Ok(await auth.RegisterAsync(request)))
            .AllowAnonymous();

        group.MapPost("login", async (LoginRequest request, IAuthUseCases auth) =>
            Results.Ok(await auth.LoginAsync(request)))
            .AllowAnonymous();

        group.MapPost("refresh", async (RefreshRequest request, IAuthUseCases auth) =>
            Results.Ok(await auth.RefreshAsync(request.RefreshToken)))
            .AllowAnonymous();

        return app;
    }
}
