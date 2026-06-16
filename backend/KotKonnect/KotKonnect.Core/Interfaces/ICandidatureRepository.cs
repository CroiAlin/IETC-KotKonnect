namespace KotKonnect.Core.Interfaces;

using KotKonnect.Core.Entities;
using KotKonnect.Core.Enums;

public interface ICandidatureRepository
{
    // Vrai si l'étudiant a déjà postulé à ce bien (garde la contrainte UNIQUE BienID + EtudiantID)
    Task<bool> ExistsAsync(int bienId, int etudiantId);

    Task<int> CreateAsync(Candidature candidature);

    // Candidatures envoyées par un étudiant (chacune avec le bien visé)
    Task<List<Candidature>> GetByEtudiantAsync(int etudiantId);

    // Candidatures reçues sur les biens d'un propriétaire (avec bien + étudiant)
    Task<List<Candidature>> GetByProprietaireAsync(int proprietaireId);

    // Détail d'une candidature avec son bien (pour vérifier la propriété avant un changement de statut)
    Task<Candidature?> GetByIdAsync(int candidatureId);

    Task<bool> UpdateStatutAsync(int candidatureId, StatutCandidature statut);
}
