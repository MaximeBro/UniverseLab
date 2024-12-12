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
    private string InvalidCharsPattern => @"[^a-zA-Z0-9àâäéèêëîïôöùûüç\s._-]";
    private string ExtensionPattern = @"\.[a-zA-Z0-9]+$";
    
    public override bool IsValid(object? value)
    {
        string? fileName = value as string;

        if (string.IsNullOrEmpty(fileName))
        {
            ErrorMessage = "Le nom est requis.";
            return false;
        } 
        
        if (fileName.Length < 3)
        {
            ErrorMessage = "Le nom doit être composé de 3 caractères au minimum.";
            return false;
        } 
        
        if (Regex.IsMatch(fileName, InvalidCharsPattern))
        {
            ErrorMessage = "Le nom contient des caractères interdits (/, \\, :, $, $, <, >, ?, |).";
            return false;
        } 
        
        if (!Regex.IsMatch(fileName, ExtensionPattern))
        {
            ErrorMessage = "Le fichier doit avoir une extension valide.";
            return false;
        }

        return true;
    }
}