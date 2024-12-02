using System.ComponentModel.DataAnnotations;

namespace uDrive.Models.Forms;

public class DirectoryModel
{
    [Required(ErrorMessage = "Ce champ est requis")]
    [MinLength(3)]
    public string Name { get; set; } = string.Empty;
}