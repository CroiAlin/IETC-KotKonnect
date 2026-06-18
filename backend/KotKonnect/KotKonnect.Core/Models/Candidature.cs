namespace KotKonnect.Core.Models;

using KotKonnect.Core.Enums;

public class Candidature
{
    public int CandidatureID { get; set; }
    public int BienID { get; set; }
    public int EtudiantID { get; set; }
    public string? MessageEtudiant { get; set; }
    public StatutCandidature Statut { get; set; }
    public DateTime DateCandidature { get; set; }

    // Bien/Etudiant renseignés par le gateway selon la requête.
    public BienImmobilier? Bien { get; set; }
    public Utilisateur? Etudiant { get; set; }
}
