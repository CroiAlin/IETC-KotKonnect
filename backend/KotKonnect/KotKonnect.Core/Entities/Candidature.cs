namespace KotKonnect.Core.Entities;

using KotKonnect.Core.Enums;

public class Candidature
{
    public int CandidatureID { get; set; }
    public int BienID { get; set; }
    public int EtudiantID { get; set; }
    public string? MessageEtudiant { get; set; }
    public StatutCandidature Statut { get; set; }
    public DateTime DateCandidature { get; set; }

    // Remplis par le multi-mapping Dapper selon la requête (même principe que BienImmobilier.Photos) :
    //  - mes candidatures (étudiant)    -> Bien renseigné
    //  - candidatures reçues (proprio)  -> Bien + Etudiant renseignés
    public BienImmobilier? Bien { get; set; }
    public Utilisateur? Etudiant { get; set; }
}
