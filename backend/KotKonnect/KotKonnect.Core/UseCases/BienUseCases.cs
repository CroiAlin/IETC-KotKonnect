namespace KotKonnect.Core.UseCases;

using KotKonnect.Core.Enums;
using KotKonnect.Core.Exceptions;
using KotKonnect.Core.IGateways;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public class BienUseCases : IBienUseCases
{
    private readonly IBienGateway _bienGateway;

    public BienUseCases(IBienGateway bienGateway)
    {
        _bienGateway = bienGateway;
    }

    public Task<List<BienImmobilier>> GetPubliesAsync() => _bienGateway.GetAllPubliesAsync();

    public Task<BienImmobilier?> GetByIdAsync(int id) => _bienGateway.GetByIdAsync(id);

    public Task<List<BienImmobilier>> GetMesBiensAsync(int proprietaireId) =>
        _bienGateway.GetByProprietaireAsync(proprietaireId);

    public async Task<BienImmobilier> CreateAsync(CreateBienRequest request, int proprietaireId)
    {
        var bien = new BienImmobilier
        {
            ProprietaireID = proprietaireId,
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

        var id = await _bienGateway.CreateAsync(bien);
        return (await _bienGateway.GetByIdAsync(id))!;
    }

    public async Task UpdateAsync(int id, UpdateBienRequest request, int proprietaireId)
    {
        var bien = await ChargerEtVerifierProprietaireAsync(id, proprietaireId);

        if (!Enum.TryParse<StatutBien>(request.Statut, out var statut))
            throw new ArgumentException("Statut invalide.");

        bien.Titre = request.Titre;
        bien.Description = request.Description;
        bien.Adresse = request.Adresse;
        bien.Ville = request.Ville;
        bien.CodePostal = request.CodePostal;
        bien.Surface = request.Surface;
        bien.NombrePieces = request.NombrePieces;
        bien.LoyerBase = request.LoyerBase;
        bien.Charges = request.Charges;
        bien.Statut = statut;

        await _bienGateway.UpdateAsync(bien);
    }

    public async Task DeleteAsync(int id, int proprietaireId)
    {
        await ChargerEtVerifierProprietaireAsync(id, proprietaireId);
        await _bienGateway.SoftDeleteAsync(id);
    }

    public async Task AjouterPhotoAsync(int bienId, AddPhotoRequest request, int proprietaireId)
    {
        var bien = await ChargerEtVerifierProprietaireAsync(bienId, proprietaireId);

        var photo = new Photo
        {
            BienID = bienId,
            UrlImage = request.UrlImage,
            Ordre = bien.Photos.Count
        };
        await _bienGateway.AddPhotoAsync(photo);
    }

    public async Task SupprimerPhotoAsync(int bienId, int photoId, int proprietaireId)
    {
        await ChargerEtVerifierProprietaireAsync(bienId, proprietaireId);
        await _bienGateway.DeletePhotoAsync(photoId, bienId);
    }

    // 404 si introuvable, 403 si ce n'est pas le bien du propriétaire.
    private async Task<BienImmobilier> ChargerEtVerifierProprietaireAsync(int bienId, int proprietaireId)
    {
        var bien = await _bienGateway.GetByIdAsync(bienId)
            ?? throw new KeyNotFoundException("Bien introuvable.");

        if (bien.ProprietaireID != proprietaireId)
            throw new ForbiddenException("Ce bien ne vous appartient pas.");

        return bien;
    }
}
