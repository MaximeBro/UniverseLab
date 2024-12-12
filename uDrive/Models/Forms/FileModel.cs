using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace uDrive.Models.Forms;

public class FileModel
{
    [ValidateFileName]
    public string Name { get; set; } = string.Empty;
}

sealed class ValidateFileName : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        string? fileName = value as string;

        if (string.IsNullOrEmpty(fileName))
        {
            ErrorMessage += "Le nom est requis. " + Environment.NewLine;
            return false;
        }
        
        if (fileName.Length < 3)
        {
            ErrorMessage += "Le nom doit être composé de 3 caractères au minimum. " + Environment.NewLine;
            return false;
        }
        
        string invalidCharsPattern = @"[^a-zA-Z0-9àâäéèêëîïôöùûüç\s._-]";
        if (Regex.IsMatch(fileName, invalidCharsPattern))
        {
            ErrorMessage += "Le nom contient des caractères interdits (\"/\", \"\\\", \":\", \"$\", \"$\", \"<\", \">\", \"?\", \"|\"). " + Environment.NewLine;
            return false;
        }
        
        string extensionPattern = @"\.[a-zA-Z0-9]+$";
        if (!Regex.IsMatch(fileName, extensionPattern))
        {
            ErrorMessage = "Le fichier doit avoir une extension valide. ";
            return false;
        }

        return true;
    }
}