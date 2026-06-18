namespace KotKonnect.Infrastructure.Models;

public class RefreshToken
{
    public int TokenID { get; set; }
    public int UtilisateurID { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime DateExpiration { get; set; }
    public bool Revoque { get; set; }
}
