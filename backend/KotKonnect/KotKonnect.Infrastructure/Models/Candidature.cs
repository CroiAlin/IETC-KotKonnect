namespace KotKonnect.Infrastructure.Models;

// Persistance : table CANDIDATURES (Statut en chaîne).
public class Candidature
{
    public int CandidatureID { get; set; }
    public int BienID { get; set; }
    public int EtudiantID { get; set; }
    public string? MessageEtudiant { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime DateCandidature { get; set; }

    public BienImmobilier? Bien { get; set; }
    public Utilisateur? Etudiant { get; set; }
}
