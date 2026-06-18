namespace KotKonnect.Infrastructure.Models;

// Persistance : table UTILISATEURS (avec le hash, Role en chaîne).
public class Utilisateur
{
    public int UtilisateurID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime DateCreation { get; set; }
}
