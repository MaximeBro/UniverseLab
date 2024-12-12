using System.ComponentModel.DataAnnotations;

namespace uDrive.Models.Forms;

public class DirectoryModel
{
    [Required(ErrorMessage = "Ce champ est requis")]
    [RegularExpression(@"^[a-zA-Z0-9àâäéèêëîïôöùûüç\s._-]+$", 
        ErrorMessage = "Le nom contient des caractères interdits (\"/\", \"\\\", \":\", \"$\", \"$\", \"<\", \">\", \"?\", \"|\").")]
    [MinLength(3, ErrorMessage = "Le nom doit être composé de 3 caractères au minimum.")]
    public string Name { get; set; } = string.Empty;
}