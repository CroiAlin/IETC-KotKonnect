namespace KotKonnect.Infrastructure.Repositories.Abstractions;

using KotKonnect.Infrastructure.Models;

public interface ICandidatureRepository
{
    Task<bool> ExistsAsync(int bienId, int etudiantId);
    Task<int> CreateAsync(Candidature candidature);
    Task<List<Candidature>> GetByEtudiantAsync(int etudiantId);
    Task<List<Candidature>> GetByProprietaireAsync(int proprietaireId);
    Task<Candidature?> GetByIdAsync(int candidatureId);
    Task<bool> UpdateStatutAsync(int candidatureId, string statut);
    Task<bool> EtudiantAPostuleChezProprioAsync(int etudiantId, int proprietaireId);
}
