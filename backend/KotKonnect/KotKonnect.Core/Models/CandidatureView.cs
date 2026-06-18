namespace KotKonnect.Core.Models;

using KotKonnect.Core.Enums;

// Vue aplatie d'une candidature pour le frontend.
public class CandidatureView
{
    public int CandidatureID { get; set; }
    public int BienID { get; set; }
    public int EtudiantID { get; set; }
    public string? MessageEtudiant { get; set; }
    public StatutCandidature Statut { get; set; }
    public DateTime DateCandidature { get; set; }

    public string TitreBien { get; set; } = string.Empty;
    public string VilleBien { get; set; } = string.Empty;

    // Renseigné uniquement côté propriétaire.
    public string? EmailEtudiant { get; set; }
}
