namespace KotKonnect.Core.DTOs;

// Sortie : vue aplatie d'une candidature pour le frontend
public class CandidatureDto
{
    public int CandidatureID { get; set; }
    public int BienID { get; set; }
    public int EtudiantID { get; set; }
    public string? MessageEtudiant { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime DateCandidature { get; set; }

    // Infos du bien visé (toujours renseignées)
    public string TitreBien { get; set; } = string.Empty;
    public string VilleBien { get; set; } = string.Empty;

    // Identité de l'étudiant (renseignée uniquement pour la vue du propriétaire)
    public string? EmailEtudiant { get; set; }
}
