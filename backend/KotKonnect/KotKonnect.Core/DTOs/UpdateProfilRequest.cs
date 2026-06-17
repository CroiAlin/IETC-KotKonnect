namespace KotKonnect.Core.DTOs;

// Entrée API : ce que le frontend envoie en PUT pour modifier le profil.
// Pas d'ID ici : on modifie TOUJOURS le profil de l'utilisateur connecté (lu dans le JWT).
public class UpdateProfilRequest
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? Telephone { get; set; }
    public string? Ville { get; set; }
    public string? Ecole { get; set; }
    public decimal? BudgetMax { get; set; }
}
