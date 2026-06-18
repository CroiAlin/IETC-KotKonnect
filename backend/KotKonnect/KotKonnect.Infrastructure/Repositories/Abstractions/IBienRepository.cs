namespace KotKonnect.Infrastructure.Repositories.Abstractions;

using KotKonnect.Infrastructure.Models;

public interface IBienRepository
{
    Task<List<BienImmobilier>> GetAllPubliesAsync();
    Task<List<BienImmobilier>> GetByProprietaireAsync(int proprietaireId);
    Task<BienImmobilier?> GetByIdAsync(int bienId);
    Task<int> CreateAsync(BienImmobilier bien);
    Task<bool> UpdateAsync(BienImmobilier bien);
    Task<bool> SoftDeleteAsync(int bienId);
    Task<int> AddPhotoAsync(Photo photo);
    Task<bool> DeletePhotoAsync(int photoId, int bienId);
}
