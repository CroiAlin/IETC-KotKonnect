namespace KotKonnect.Core.DTOs;

public class BienDto
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
    public string Statut { get; set; } = string.Empty;
    public List<PhotoDto> Photos { get; set; } = new();
}

public class PhotoDto
{
    public int PhotoID { get; set; }
    public string UrlImage { get; set; } = string.Empty;
    public int Ordre { get; set; }
}
