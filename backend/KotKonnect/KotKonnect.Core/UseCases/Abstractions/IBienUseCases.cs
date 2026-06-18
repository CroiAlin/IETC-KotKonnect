namespace KotKonnect.Core.UseCases.Abstractions;

using KotKonnect.Core.Models;

public interface IBienUseCases
{
    Task<List<BienImmobilier>> GetPubliesAsync();
    Task<BienImmobilier?> GetByIdAsync(int id);
    Task<List<BienImmobilier>> GetMesBiensAsync(int proprietaireId);
    Task<BienImmobilier> CreateAsync(CreateBienRequest request, int proprietaireId);
    Task UpdateAsync(int id, UpdateBienRequest request, int proprietaireId);
    Task DeleteAsync(int id, int proprietaireId);
    Task AjouterPhotoAsync(int bienId, AddPhotoRequest request, int proprietaireId);
    Task SupprimerPhotoAsync(int bienId, int photoId, int proprietaireId);
}
