namespace KotKonnect.Core.Models;

public class CreateBienRequest
{
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;
    public decimal Surface { get; set; }
    public int NombrePieces { get; set; }
    public decimal LoyerBase { get; set; }
    public decimal Charges { get; set; }
}
