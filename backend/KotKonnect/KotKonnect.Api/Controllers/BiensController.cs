namespace KotKonnect.Api.Controllers;

using System.Security.Claims;
using KotKonnect.Core.DTOs;
using KotKonnect.Core.Entities;
using KotKonnect.Core.Enums;
using KotKonnect.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/biens")]
public class BiensController : ControllerBase
{
    private readonly IBienRepository _bienRepository;

    public BiensController(IBienRepository bienRepository)
    {
        _bienRepository = bienRepository;
    }

    // GET /api/biens  -> liste publique (statut PUBLIE), accessible à tous
    [HttpGet]
    public async Task<ActionResult<List<BienDto>>> GetPublies()
    {
        var biens = await _bienRepository.GetAllPubliesAsync();
        return Ok(biens.Select(ToDto).ToList());
    }

    // GET /api/biens/{id} -> détail d'un bien, accessible à tous
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BienDto>> GetById(int id)
    {
        var bien = await _bienRepository.GetByIdAsync(id);
        if (bien is null) 
        { 
            return NotFound(); 
        }

        return Ok(ToDto(bien));

    }

    // GET /api/biens/mes-biens -> les biens du propriétaire connecté
    [HttpGet("mes-biens")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<ActionResult<List<BienDto>>> GetMesBiens()
    {
        var biens = await _bienRepository.GetByProprietaireAsync(GetUserId());
        return Ok(biens.Select(ToDto).ToList());
    }

    // POST /api/biens -> création (propriétaire uniquement)
    [HttpPost]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<ActionResult<BienDto>> Create(CreateBienRequest request)
    {
        var bien = new BienImmobilier
        {
            ProprietaireID = GetUserId(),
            Titre = request.Titre,
            Description = request.Description,
            Adresse = request.Adresse,
            Ville = request.Ville,
            CodePostal = request.CodePostal,
            Surface = request.Surface,
            NombrePieces = request.NombrePieces,
            LoyerBase = request.LoyerBase,
            Charges = request.Charges,
            Statut = StatutBien.BROUILLON
        };

        var id = await _bienRepository.CreateAsync(bien);
        var bienCree = await _bienRepository.GetByIdAsync(id);
        return Ok(ToDto(bienCree!));
    }

    // PUT /api/biens/{id} -> édition (uniquement le propriétaire DU bien)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<IActionResult> Update(int id, UpdateBienRequest request)
    {

        var bien = await _bienRepository.GetByIdAsync(id);

        if (bien == null)
        {
            return NotFound();
        }
        else if (bien.ProprietaireID != GetUserId())
        {
            return Forbid();
        }
        bien.Titre = request.Titre; 
        bien.Description = request.Description;
        bien.Adresse = request.Adresse;
        bien.Ville = request.Ville;
        bien.CodePostal = request.CodePostal;
        bien.Surface = request.Surface;
        bien.NombrePieces = request.NombrePieces;
        bien.LoyerBase = request.LoyerBase;
        bien.Charges = request.Charges;
        if (!Enum.TryParse<StatutBien>(request.Statut, out var statut))
        {
            return BadRequest(new { message = "Statut invalide." });
        }
        bien.Statut = statut;

        await _bienRepository.UpdateAsync(bien);
        return NoContent();
    }

    // DELETE /api/biens/{id} -> soft delete (uniquement le propriétaire DU bien)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<IActionResult> Delete(int id)
    {
        var bien = await _bienRepository.GetByIdAsync(id);

        if (bien == null)
        {
            return NotFound();
        }
        else if (bien.ProprietaireID != GetUserId())
        {
            return Forbid();
        }

        await _bienRepository.SoftDeleteAsync(id);

        return NoContent();
    }

    // POST /api/biens/{bienId}/photos -> ajoute une photo (URL) au bien (propriétaire owner)
    [HttpPost("{bienId:int}/photos")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<IActionResult> AjouterPhoto(int bienId, AddPhotoRequest request)
    {
        var bien = await _bienRepository.GetByIdAsync(bienId);
        if (bien is null) return NotFound();
        if (bien.ProprietaireID != GetUserId()) return Forbid();

        var photo = new Photo
        {
            BienID = bienId,
            UrlImage = request.UrlImage,
            Ordre = bien.Photos.Count,
        };
        await _bienRepository.AddPhotoAsync(photo);
        return NoContent();
    }

    // DELETE /api/biens/{bienId}/photos/{photoId} -> supprime une photo (propriétaire owner)
    [HttpDelete("{bienId:int}/photos/{photoId:int}")]
    [Authorize(Roles = "PROPRIETAIRE")]
    public async Task<IActionResult> SupprimerPhoto(int bienId, int photoId)
    {
        var bien = await _bienRepository.GetByIdAsync(bienId);
        if (bien is null) return NotFound();
        if (bien.ProprietaireID != GetUserId()) return Forbid();

        await _bienRepository.DeletePhotoAsync(photoId, bienId);
        return NoContent();
    }

    // ---- helpers ----

    // Lit l'ID de l'utilisateur connecté depuis le JWT (claim NameIdentifier)
    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Mapping entité -> DTO (boilerplate)
    private static BienDto ToDto(BienImmobilier b) => new()
    {
        BienID = b.BienID,
        ProprietaireID = b.ProprietaireID,
        Titre = b.Titre,
        Description = b.Description,
        Adresse = b.Adresse,
        Ville = b.Ville,
        CodePostal = b.CodePostal,
        Surface = b.Surface,
        NombrePieces = b.NombrePieces,
        LoyerBase = b.LoyerBase,
        Charges = b.Charges,
        Statut = b.Statut.ToString(),
        Photos = b.Photos.Select(p => new PhotoDto
        {
            PhotoID = p.PhotoID,
            UrlImage = p.UrlImage,
            Ordre = p.Ordre,
        }).ToList(),
    };
}
