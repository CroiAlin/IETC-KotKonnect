namespace KotKonnect.Api.EndPoints;

using System.Security.Claims;
using KotKonnect.Api.Extensions;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public static class ProfilRoutes
{
    public static WebApplication AddProfilRoutes(this WebApplication app)
    {
        // Tout le groupe : authentifié (n'importe quel rôle).
        var group = app.MapGroup("api/profils").WithTags("Profils").RequireAuthorization();

        group.MapGet("me", async (ClaimsPrincipal user, IProfilUseCases profils) =>
            Results.Ok(await profils.GetMonProfilAsync(user.GetUserId())));

        group.MapPut("me", async (UpdateProfilRequest request, ClaimsPrincipal user, IProfilUseCases profils) =>
            Results.Ok(await profils.UpdateMonProfilAsync(request, user.GetUserId())));

        // Proprio : profil d'un candidat ayant postulé chez lui (sinon 403).
        group.MapGet("{utilisateurId:int}", async (int utilisateurId, ClaimsPrincipal user, IProfilUseCases profils) =>
            Results.Ok(await profils.GetProfilAsync(utilisateurId, user.GetUserId())))
            .RequireAuthorization(p => p.RequireRole("PROPRIETAIRE"));

        return app;
    }
}
