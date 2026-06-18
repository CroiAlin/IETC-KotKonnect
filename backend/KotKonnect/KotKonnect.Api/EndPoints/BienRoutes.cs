namespace KotKonnect.Api.EndPoints;

using System.Security.Claims;
using KotKonnect.Api.Extensions;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public static class BienRoutes
{
    public static WebApplication AddBienRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/biens").WithTags("Biens");

        // Liste publique (statut PUBLIE).
        group.MapGet("", async (IBienUseCases biens) =>
            Results.Ok(await biens.GetPubliesAsync()))
            .AllowAnonymous();

        // Avant {id:int} (la contrainte :int évite la collision).
        group.MapGet("mes-biens", async (ClaimsPrincipal user, IBienUseCases biens) =>
            Results.Ok(await biens.GetMesBiensAsync(user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        // Détail public (404 si introuvable).
        group.MapGet("{id:int}", async (int id, IBienUseCases biens) =>
        {
            var bien = await biens.GetByIdAsync(id);
            return bien is null ? Results.NotFound() : Results.Ok(bien);
        })
            .AllowAnonymous();

        group.MapPost("", async (CreateBienRequest request, ClaimsPrincipal user, IBienUseCases biens) =>
            Results.Ok(await biens.CreateAsync(request, user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        group.MapPut("{id:int}", async (int id, UpdateBienRequest request, ClaimsPrincipal user, IBienUseCases biens) =>
        {
            await biens.UpdateAsync(id, request, user.GetUserId());
            return Results.NoContent();
        })
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        // Soft delete.
        group.MapDelete("{id:int}", async (int id, ClaimsPrincipal user, IBienUseCases biens) =>
        {
            await biens.DeleteAsync(id, user.GetUserId());
            return Results.NoContent();
        })
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        group.MapPost("{bienId:int}/photos", async (int bienId, AddPhotoRequest request, ClaimsPrincipal user, IBienUseCases biens) =>
        {
            await biens.AjouterPhotoAsync(bienId, request, user.GetUserId());
            return Results.NoContent();
        })
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        group.MapDelete("{bienId:int}/photos/{photoId:int}", async (int bienId, int photoId, ClaimsPrincipal user, IBienUseCases biens) =>
        {
            await biens.SupprimerPhotoAsync(bienId, photoId, user.GetUserId());
            return Results.NoContent();
        })
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        return app;
    }
}
