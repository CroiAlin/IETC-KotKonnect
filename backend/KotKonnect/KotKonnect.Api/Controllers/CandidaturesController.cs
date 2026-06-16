namespace KotKonnect.Api.Controllers;

using System.Security.Claims;
using KotKonnect.Core.DTOs;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Enums;
using KotKonnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/candidatures")]
public class CandidaturesController : ControllerBase
{
    private readonly ICandidatureRepository _candidatureRepository;
    private readonly IBienRepository _bienRepository;

    public CandidaturesController(ICandidatureRepository candidatureRepository, IBienRepository bienRepository)
    {
        _candidatureRepository = candidatureRepository;
        _bienRepository = bienRepository;
    }

    // POST /api/candidatures -> un étudiant postule à un bien PUBLIE
    [HttpPost]
    [Authorize(Roles = "ETUDIANT")]
    public async Task<ActionResult<CandidatureDto>> Postuler(CreateCandidatureRequest request)
    {
        var etudiantId = GetUserId();

        var bien = await _bienRepository.GetByIdAsync(request.BienID);
        if (bien is null || bien.Statut != StatutBien.PUBLIE)
        {
            return NotFound();
        }
        if (await _candidatureRepository.ExistsAsync(request.BienID, etudiantId)) 
        {
            return Conflict(new { message = "Vous avez déjà postulé à ce bien." });
        }

        var candidature = new Candidature
        {
            BienID = request.BienID,
            EtudiantID = etudiantId,
            MessageEtudiant = request.MessageEtudiant,
            Statut = StatutCandidature.ENVOYE,
        };

        var id = await _candidatureRepository.CreateAsync(candidature);

        var cree = await _candidatureRepository.GetByIdAsync(id);
        return Ok(ToDto(cree!));
    }

    // GET /api/candidatures/mes-candidatures -> les candidatures de l'étudiant connecté
    [HttpGet("mes-candidatures")]
    [Authorize(Roles = "ETUDIANT")]
    public async Task<ActionResult<List<CandidatureDto>>> MesCandidatures()
    {
        var candidatures = await _candidatureRepository.GetByEtudiantAsync(GetUserId());
        return Ok(candidatures.Select(ToDto).ToList());
    }

    // GET /api/candidatures/recues -> les candidatures reçues sur les biens du propriétaire connecté
    [HttpGet("recues")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<ActionResult<List<CandidatureDto>>> Recues()
    {
        var candidatures = await _candidatureRepository.GetByProprietaireAsync(GetUserId());
        return Ok(candidatures.Select(ToDto).ToList());
    }

    // PUT /api/candidatures/{id}/statut -> le propriétaire fait évoluer le statut (VU / ACCEPTE / REFUSE)
    [HttpPut("{id:int}/statut")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<IActionResult> ChangerStatut(int id, UpdateStatutRequest request)
    {
        var candidature = await _candidatureRepository.GetByIdAsync(id);
        if (candidature is null)
        {
            return NotFound();
        }
        if (candidature.Bien!.ProprietaireID != GetUserId())
        {
            return Forbid();
        }
        if (!Enum.TryParse<StatutCandidature>(request.Statut, out var statut))
        {
            return BadRequest(new { message = "Statut invalide"});
        }

        await _candidatureRepository.UpdateStatutAsync(id, statut);
        return NoContent();
    }

    // ID de l'utilisateur connecté, lu dans le JWT
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Mapping entité -> DTO 
    private static CandidatureDto ToDto(Candidature c) => new()
    {
        CandidatureID = c.CandidatureID,
        BienID = c.BienID,
        EtudiantID = c.EtudiantID,
        MessageEtudiant = c.MessageEtudiant,
        Statut = c.Statut.ToString(),
        DateCandidature = c.DateCandidature,
        TitreBien = c.Bien?.Titre ?? string.Empty,
        VilleBien = c.Bien?.Ville ?? string.Empty,
        EmailEtudiant = c.Etudiant?.Email,
    };
}
