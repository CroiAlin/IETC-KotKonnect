namespace KotKonnect.Api.Controllers;

using System.Security.Claims;
using KotKonnect.Core.DTOs;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/profils")]
[Authorize]
public class ProfilsController : ControllerBase
{
    private readonly IProfilRepository _profilRepository;

    public ProfilsController(IProfilRepository profilRepository)
    {
        _profilRepository = profilRepository;
    }

    // GET /api/profils/me -> le profil de l'utilisateur connecté (n'importe quel rôle)
    [HttpGet("me")]
    public async Task<ActionResult<ProfilDto>> GetMonProfil()
    {
        var profil = await _profilRepository.GetByUtilisateurIdAsync(GetUserId());
        if (profil is null)
        {
            return NotFound();
        }
        return Ok(ToDto(profil));
    }

    // PUT /api/profils/me -> l'utilisateur connecté modifie SON propre profil
    [HttpPut("me")]
    public async Task<ActionResult<ProfilDto>> UpdateMonProfil(UpdateProfilRequest request)
    {
        var profil = await _profilRepository.GetByUtilisateurIdAsync(GetUserId());
        if (profil is null)
        {
            return NotFound();
        }

        // On reporte les champs reçus sur l'entité chargée, puis on persiste.
        profil.Nom = request.Nom;
        profil.Prenom = request.Prenom;
        profil.Telephone = request.Telephone;
        profil.Ville = request.Ville;
        profil.Ecole = request.Ecole;
        profil.BudgetMax = request.BudgetMax;

        await _profilRepository.UpdateAsync(profil);
        return Ok(ToDto(profil));
    }

    // GET /api/profils/{utilisateurId} -> profil d'un autre utilisateur.
    // ⚠️ Sécurité de base pour l'instant (juste [Authorize] de la classe).
    // La règle métier « un propriétaire ne voit que le profil d'un étudiant
    // ayant postulé à un de ses biens » sera ajoutée en demande D (Forbid 403 sinon).
    [HttpGet("{utilisateurId:int}")]
    public async Task<ActionResult<ProfilDto>> GetProfil(int utilisateurId)
    {
        var profil = await _profilRepository.GetByUtilisateurIdAsync(utilisateurId);
        if (profil is null)
        {
            return NotFound();
        }
        return Ok(ToDto(profil));
    }

    // ID de l'utilisateur connecté, lu dans le JWT (claim NameIdentifier).
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Mapping entité -> DTO
    private static ProfilDto ToDto(Profil p) => new()
    {
        ProfilID = p.ProfilID,
        UtilisateurID = p.UtilisateurID,
        Nom = p.Nom,
        Prenom = p.Prenom,
        Telephone = p.Telephone,
        Ville = p.Ville,
        Ecole = p.Ecole,
        BudgetMax = p.BudgetMax
    };
}
