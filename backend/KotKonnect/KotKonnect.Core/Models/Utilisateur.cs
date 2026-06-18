namespace KotKonnect.Core.Models;

using KotKonnect.Core.Enums;

// Domaine : sans le mot de passe (le hash reste en Infrastructure).
public class Utilisateur
{
    public int UtilisateurID { get; set; }
    public string Email { get; set; } = string.Empty;
    public RoleUtilisateur Role { get; set; }
    public DateTime DateCreation { get; set; }
}
