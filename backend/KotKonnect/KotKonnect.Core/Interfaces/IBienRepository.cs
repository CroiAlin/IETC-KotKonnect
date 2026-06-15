namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;

public interface IBienRepository
{
    // Liste publique : uniquement les biens au statut PUBLIE (avec leurs photos)
    Task<List<BienImmobilier>> GetAllPubliesAsync();

    // Les biens d'un propriétaire (tous statuts sauf SUPPRIME)
    Task<List<BienImmobilier>> GetByProprietaireAsync(int proprietaireId);

    // Détail d'un bien (avec ses photos)
    Task<BienImmobilier?> GetByIdAsync(int bienId);

    Task<int> CreateAsync(BienImmobilier bien);
    Task<bool> UpdateAsync(BienImmobilier bien);

    // Soft delete : passe le Statut à SUPPRIME (jamais de DELETE physique)
    Task<bool> SoftDeleteAsync(int bienId);

    // Photos d'un bien
    Task<int> AddPhotoAsync(Photo photo);
    Task<bool> DeletePhotoAsync(int photoId, int bienId);
}
