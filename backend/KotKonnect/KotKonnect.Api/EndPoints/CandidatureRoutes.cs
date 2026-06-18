namespace KotKonnect.Api.EndPoints;

using System.Security.Claims;
using KotKonnect.Api.Extensions;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public static class CandidatureRoutes
{
    public static WebApplication AddCandidatureRoutes(this WebApplication app)
    {
        var group = app.MapGroup("api/candidatures").WithTags("Candidatures");

        group.MapPost("", async (CreateCandidatureRequest request, ClaimsPrincipal user, ICandidatureUseCases candidatures) =>
            Results.Ok(await candidatures.PostulerAsync(request, user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("ETUDIANT"));

        group.MapGet("mes-candidatures", async (ClaimsPrincipal user, ICandidatureUseCases candidatures) =>
            Results.Ok(await candidatures.GetMesCandidaturesAsync(user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("ETUDIANT"));

        group.MapGet("recues", async (ClaimsPrincipal user, ICandidatureUseCases candidatures) =>
            Results.Ok(await candidatures.GetRecuesAsync(user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        group.MapPut("{id:int}/statut", async (int id, UpdateStatutRequest request, ClaimsPrincipal user, ICandidatureUseCases candidatures) =>
        {
            await candidatures.ChangerStatutAsync(id, request, user.GetUserId());
            return Results.NoContent();
        })
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        return app;
    }
}
