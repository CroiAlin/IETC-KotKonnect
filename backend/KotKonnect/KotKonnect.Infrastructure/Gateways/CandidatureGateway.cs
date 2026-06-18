namespace KotKonnect.Infrastructure.Gateways;

using KotKonnect.Core.Enums;
using KotKonnect.Core.IGateways;
using KotKonnect.Infrastructure.Repositories.Abstractions;
using CoreModels = KotKonnect.Core.Models;
using InfraModels = KotKonnect.Infrastructure.Models;

public class CandidatureGateway : ICandidatureGateway
{
    private readonly ICandidatureRepository _repository;

    public CandidatureGateway(ICandidatureRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> ExistsAsync(int bienId, int etudiantId) => _repository.ExistsAsync(bienId, etudiantId);

    public Task<int> CreateAsync(CoreModels.Candidature candidature) => _repository.CreateAsync(ToInfra(candidature));

    public async Task<List<CoreModels.Candidature>> GetByEtudiantAsync(int etudiantId) =>
        (await _repository.GetByEtudiantAsync(etudiantId)).Select(ToCore).ToList();

    public async Task<List<CoreModels.Candidature>> GetByProprietaireAsync(int proprietaireId) =>
        (await _repository.GetByProprietaireAsync(proprietaireId)).Select(ToCore).ToList();

    public async Task<CoreModels.Candidature?> GetByIdAsync(int candidatureId)
    {
        var candidature = await _repository.GetByIdAsync(candidatureId);
        return candidature is null ? null : ToCore(candidature);
    }

    public Task<bool> UpdateStatutAsync(int candidatureId, StatutCandidature statut) =>
        _repository.UpdateStatutAsync(candidatureId, statut.ToString());

    public Task<bool> EtudiantAPostuleChezProprioAsync(int etudiantId, int proprietaireId) =>
        _repository.EtudiantAPostuleChezProprioAsync(etudiantId, proprietaireId);

    private static CoreModels.Candidature ToCore(InfraModels.Candidature c) => new()
    {
        CandidatureID = c.CandidatureID,
        BienID = c.BienID,
        EtudiantID = c.EtudiantID,
        MessageEtudiant = c.MessageEtudiant,
        Statut = Enum.Parse<StatutCandidature>(c.Statut),
        DateCandidature = c.DateCandidature,
        Bien = c.Bien is null ? null : BienGateway.ToCore(c.Bien),
        Etudiant = c.Etudiant is null ? null : UtilisateurGateway.ToCore(c.Etudiant)
    };

    private static InfraModels.Candidature ToInfra(CoreModels.Candidature c) => new()
    {
        CandidatureID = c.CandidatureID,
        BienID = c.BienID,
        EtudiantID = c.EtudiantID,
        MessageEtudiant = c.MessageEtudiant,
        Statut = c.Statut.ToString(),
        DateCandidature = c.DateCandidature
    };
}
