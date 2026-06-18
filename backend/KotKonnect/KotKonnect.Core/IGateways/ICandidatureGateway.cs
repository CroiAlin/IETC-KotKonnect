namespace KotKonnect.Core.IGateways;

using KotKonnect.Core.Enums;
using KotKonnect.Core.Models;

public interface ICandidatureGateway
{
    Task<bool> ExistsAsync(int bienId, int etudiantId);
    Task<int> CreateAsync(Candidature candidature);
    Task<List<Candidature>> GetByEtudiantAsync(int etudiantId);
    Task<List<Candidature>> GetByProprietaireAsync(int proprietaireId);
    Task<Candidature?> GetByIdAsync(int candidatureId);
    Task<bool> UpdateStatutAsync(int candidatureId, StatutCandidature statut);
    // Sécurité : l'étudiant a-t-il postulé chez ce propriétaire ?
    Task<bool> EtudiantAPostuleChezProprioAsync(int etudiantId, int proprietaireId);
}
