namespace KotKonnect.Core.Models;

using KotKonnect.Core.Enums;

public class BienImmobilier
{
    public int BienID { get; set; }
    public int ProprietaireID { get; set; }
    public string Titre { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Adresse { get; set; } = string.Empty;
    public string Ville { get; set; } = string.Empty;
    public string CodePostal { get; set; } = string.Empty;
    public decimal Surface { get; set; }
    public int NombrePieces { get; set; }
    public decimal LoyerBase { get; set; }
    public decimal Charges { get; set; }
    public StatutBien Statut { get; set; }

    // Rempli par le gateway.
    public List<Photo> Photos { get; set; } = new();
}
