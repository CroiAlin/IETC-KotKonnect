namespace KotKonnect.Core.UseCases.Abstractions;

using KotKonnect.Core.Models;

public interface ICandidatureUseCases
{
    Task<CandidatureView> PostulerAsync(CreateCandidatureRequest request, int etudiantId);
    Task<List<CandidatureView>> GetMesCandidaturesAsync(int etudiantId);
    Task<List<CandidatureView>> GetRecuesAsync(int proprietaireId);
    Task ChangerStatutAsync(int candidatureId, UpdateStatutRequest request, int proprietaireId);
}
