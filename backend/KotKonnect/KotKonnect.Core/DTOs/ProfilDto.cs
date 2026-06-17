namespace KotKonnect.Core.DTOs;

// Sortie API : ce que le frontend reçoit quand il lit un profil.
public class ProfilDto
{
    public int ProfilID { get; set; }
    public int UtilisateurID { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Ville { get; set; }
    public string? Ecole { get; set; }
    public decimal? BudgetMax { get; set; }
}
