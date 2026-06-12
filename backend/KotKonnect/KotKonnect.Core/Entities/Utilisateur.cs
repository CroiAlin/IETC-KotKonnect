namespace KotKonnect.Core.Entities;

using KotKonnect.Core.Enums;

public class Utilisateur
{
    public int UtilisateurID { get; set; }
    public string Email { get; set; } = string.Empty;
    public string MotDePasseHash { get; set; } = string.Empty;
    public RoleUtilisateur Role { get; set; }
    public DateTime DateCreation { get; set; }
}
