namespace KotKonnect.Core.DTOs;

// Entrée : ce que l'étudiant envoie pour postuler
public class CreateCandidatureRequest
{
    public int BienID { get; set; }
    public string? MessageEtudiant { get; set; }
}
