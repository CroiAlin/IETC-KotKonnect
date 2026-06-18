namespace KotKonnect.Core.UseCases;

using KotKonnect.Core.Enums;
using KotKonnect.Core.Exceptions;
using KotKonnect.Core.IGateways;
using KotKonnect.Core.Models;
using KotKonnect.Core.UseCases.Abstractions;

public class CandidatureUseCases : ICandidatureUseCases
{
    private readonly ICandidatureGateway _candidatureGateway;
    private readonly IBienGateway _bienGateway;

    public CandidatureUseCases(ICandidatureGateway candidatureGateway, IBienGateway bienGateway)
    {
        _candidatureGateway = candidatureGateway;
        _bienGateway = bienGateway;
    }

    public async Task<CandidatureView> PostulerAsync(CreateCandidatureRequest request, int etudiantId)
    {
        var bien = await _bienGateway.GetByIdAsync(request.BienID);
        if (bien is null || bien.Statut != StatutBien.PUBLIE)
            throw new KeyNotFoundException("Bien introuvable ou non publié.");

        if (await _candidatureGateway.ExistsAsync(request.BienID, etudiantId))
            throw new InvalidOperationException("Vous avez déjà postulé à ce bien.");

        var candidature = new Candidature
        {
            BienID = request.BienID,
            EtudiantID = etudiantId,
            MessageEtudiant = request.MessageEtudiant,
            Statut = StatutCandidature.ENVOYE
        };

        var id = await _candidatureGateway.CreateAsync(candidature);
        var cree = await _candidatureGateway.GetByIdAsync(id);
        return ToView(cree!);
    }

    public async Task<List<CandidatureView>> GetMesCandidaturesAsync(int etudiantId)
    {
        var candidatures = await _candidatureGateway.GetByEtudiantAsync(etudiantId);
        return candidatures.Select(ToView).ToList();
    }

    public async Task<List<CandidatureView>> GetRecuesAsync(int proprietaireId)
    {
        var candidatures = await _candidatureGateway.GetByProprietaireAsync(proprietaireId);
        return candidatures.Select(ToView).ToList();
    }

    public async Task ChangerStatutAsync(int candidatureId, UpdateStatutRequest request, int proprietaireId)
    {
        var candidature = await _candidatureGateway.GetByIdAsync(candidatureId)
            ?? throw new KeyNotFoundException("Candidature introuvable.");

        // Le bien (et donc son propriétaire) est joint par le gateway.
        if (candidature.Bien!.ProprietaireID != proprietaireId)
            throw new ForbiddenException("Cette candidature ne concerne pas un de vos biens.");

        if (!Enum.TryParse<StatutCandidature>(request.Statut, out var statut))
            throw new ArgumentException("Statut invalide.");

        await _candidatureGateway.UpdateStatutAsync(candidatureId, statut);
    }

    // Domaine imbriqué -> vue aplatie pour le frontend.
    private static CandidatureView ToView(Candidature c) => new()
    {
        CandidatureID = c.CandidatureID,
        BienID = c.BienID,
        EtudiantID = c.EtudiantID,
        MessageEtudiant = c.MessageEtudiant,
        Statut = c.Statut,
        DateCandidature = c.DateCandidature,
        TitreBien = c.Bien?.Titre ?? string.Empty,
        VilleBien = c.Bien?.Ville ?? string.Empty,
        EmailEtudiant = c.Etudiant?.Email
    };
}
