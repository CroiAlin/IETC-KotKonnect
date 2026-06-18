namespace KotKonnect.Infrastructure.Gateways;

using KotKonnect.Core.Enums;
using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using CoreModels = KotKonnect.Core.Models;
using InfraModels = KotKonnect.Infrastructure.Models;

public class BienGateway : IBienGateway
{
    private readonly IBienRepository _repository;

    public BienGateway(IBienRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CoreModels.BienImmobilier>> GetAllPubliesAsync() =>
        (await _repository.GetAllPubliesAsync()).Select(ToCore).ToList();

    public async Task<List<CoreModels.BienImmobilier>> GetByProprietaireAsync(int proprietaireId) =>
        (await _repository.GetByProprietaireAsync(proprietaireId)).Select(ToCore).ToList();

    public async Task<CoreModels.BienImmobilier?> GetByIdAsync(int bienId)
    {
        var bien = await _repository.GetByIdAsync(bienId);
        return bien is null ? null : ToCore(bien);
    }

    public Task<int> CreateAsync(CoreModels.BienImmobilier bien) => _repository.CreateAsync(ToInfra(bien));

    public Task<bool> UpdateAsync(CoreModels.BienImmobilier bien) => _repository.UpdateAsync(ToInfra(bien));

    public Task<bool> SoftDeleteAsync(int bienId) => _repository.SoftDeleteAsync(bienId);

    public Task<int> AddPhotoAsync(CoreModels.Photo photo) => _repository.AddPhotoAsync(ToInfra(photo));

    public Task<bool> DeletePhotoAsync(int photoId, int bienId) => _repository.DeletePhotoAsync(photoId, bienId);

    // Public : réutilisé par CandidatureGateway.
    public static CoreModels.BienImmobilier ToCore(InfraModels.BienImmobilier b) => new()
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
        Statut = Enum.Parse<StatutBien>(b.Statut),
        Photos = b.Photos.Select(ToCore).ToList()
    };

    private static CoreModels.Photo ToCore(InfraModels.Photo p) => new()
    {
        PhotoID = p.PhotoID,
        BienID = p.BienID,
        UrlImage = p.UrlImage,
        Ordre = p.Ordre
    };

    private static InfraModels.BienImmobilier ToInfra(CoreModels.BienImmobilier b) => new()
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
        Statut = b.Statut.ToString()
    };

    private static InfraModels.Photo ToInfra(CoreModels.Photo p) => new()
    {
        PhotoID = p.PhotoID,
        BienID = p.BienID,
        UrlImage = p.UrlImage,
        Ordre = p.Ordre
    };
}
