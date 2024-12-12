using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace uDrive.Models.Forms;

public class DirectoryModel
{
    [ValidateFolderName]
    public string Name { get; set; } = string.Empty;
}

sealed class ValidateFolderName : ValidationAttribute
{
    private string InvalidCharsPattern => @"[<>:""/\\|?*]";
    
    public override bool IsValid(object? value)
    {
        string? folderName = value as string;

        if (string.IsNullOrEmpty(folderName))
        {
            ErrorMessage = "Le nom est requis.";
            return false;
        } 
        
        if (folderName.Length < 3)
        {
            ErrorMessage = "Le nom doit être composé de 3 caractères au minimum.";
            return false;
        }  
        
        if (Regex.IsMatch(folderName, InvalidCharsPattern))
        {
            ErrorMessage = "Le nom contient des caractères interdits (/, \\, :, $, $, <, >, ?, |).";
            return false;
        } 

        return true;
    }
}