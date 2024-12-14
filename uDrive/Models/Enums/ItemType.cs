using System.ComponentModel;

namespace uDrive.Models.Enums;

public enum ItemType
{
    [Description("fichier")]
    File,
    
    [Description("dossier")]
    Folder
}