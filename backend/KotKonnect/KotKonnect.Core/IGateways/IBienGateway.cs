namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Models;

public interface IBienGateway
{
    Task<List<BienImmobilier>> GetAllPubliesAsync();
    Task<List<BienImmobilier>> GetByProprietaireAsync(int proprietaireId);
    Task<BienImmobilier?> GetByIdAsync(int bienId);
    Task<int> CreateAsync(BienImmobilier bien);
    Task<bool> UpdateAsync(BienImmobilier bien);
    Task<bool> SoftDeleteAsync(int bienId); // Statut -> SUPPRIME (pas de DELETE physique)
    Task<int> AddPhotoAsync(Photo photo);
    Task<bool> DeletePhotoAsync(int photoId, int bienId);
}
