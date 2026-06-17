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
    private readonly ICandidatureRepository _candidatureRepository;

    public ProfilsController(IProfilRepository profilRepository, ICandidatureRepository candidatureRepository)
    {
        _profilRepository = profilRepository;
        _candidatureRepository = candidatureRepository;
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

    // GET /api/profils/{utilisateurId} -> un PROPRIETAIRE consulte le profil d'un candidat.
    // Sécurité : il ne peut voir QUE les étudiants ayant postulé à un de SES biens (sinon 403).
    [HttpGet("{utilisateurId:int}")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<ActionResult<ProfilDto>> GetProfil(int utilisateurId)
    {
        var aPostuleChezMoi = await _candidatureRepository.EtudiantAPostuleChezProprioAsync(utilisateurId, GetUserId());
        if (!aPostuleChezMoi)
        {
            return Forbid();
        }

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
