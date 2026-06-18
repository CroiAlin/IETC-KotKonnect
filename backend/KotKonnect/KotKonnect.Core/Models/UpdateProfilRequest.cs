namespace KotKonnect.Core.Models;

// Pas d'ID : on modifie le profil du connecté (lu dans le JWT).
public class UpdateProfilRequest
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Ville { get; set; }
    public string? Ecole { get; set; }
    public decimal? BudgetMax { get; set; }
}
