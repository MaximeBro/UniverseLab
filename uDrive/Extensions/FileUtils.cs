using MudBlazor;
using uDrive.Models;
using uDrive.Models.Enums;
using uDrive.Models.Views;

namespace uDrive.Extensions;

public static class FileUtils
{
    /// <summary>
    /// 2 Go = 2 * 1024 * 1024 * 1024
    /// </summary>
    public const long DefaultMaxStorage = 2147483648L;

    public static readonly string[] SupportedLengthFormats = { "o", "Ko", "Mo", "Go", "To" };

    public static string ToFileFormat(this long @this)
    {
        if (@this < 1000) return "--"; 
            
        int order = (int) Math.Floor(Math.Log(@this, 1024));
        double size = @this / Math.Pow(1024, order);
        return $"{size:0.##} {SupportedLengthFormats[order]}";
    }

    public static string GetFullPath(this UserFile @this)
    {
        if (@this.Parent is null || @this.Parent.Parent == null)
        {
            return $"/{@this.FileName}";
        }
        
        List<UserFolder> folders = [];
        var current = @this.Parent;
        while(current != null)
        {
            folders.Add(current);
            current = current.Parent;
        }

        folders.Reverse();
        return string.Join("/", folders.Select(x => x.Name));
    }
    
    public static string GetFullPath(this UserFolder @this)
    {
        if (@this.Parent is null || @this.Parent.Parent == null)
        {
            return $"./{@this.Name}";
        }
        
        List<UserFolder> folders = [];
        var current = @this.Parent;
        while(current != null)
        {
            folders.Add(current);
            current = current.Parent;
        }

        folders.Reverse();
        return string.Join("/", folders.Select(x => x.Name)).Insert(0, ".");
    }

    public static InteractiveItem ToInteractiveItem(this UserFolder @this)
    {
        return new InteractiveItem
        {
            Id = @this.Id,
            Name = @this.Name,
            ParentId = @this.ParentId,
            Parent = @this.Parent,
            Type = ItemType.Folder,
            Color = @this.Color
        };
    }
    
    public static InteractiveItem ToInteractiveItem(this UserFile @this)
    {
        return new InteractiveItem
        {
            Id = @this.Id,
            Name = @this.FileName,
            ParentId = @this.ParentId,
            Parent = @this.Parent,
            Type = ItemType.File,
            Color = null
        };
    }

    public static string Icon(this InteractiveItem @this)
    {
        return @this.Type switch
        {
            ItemType.File => Icons.Custom.FileFormats.FileDocument,
            ItemType.Folder => Icons.Custom.Uncategorized.Folder,
            _ => Icons.Material.Rounded.QuestionMark,
        };
    }

    public static string IconHovered(this InteractiveItem @this)
    {
        return @this.Type switch
        {
            ItemType.File => Icons.Material.Rounded.FileOpen,
            ItemType.Folder => Icons.Custom.Uncategorized.FolderOpen,
            _ => Icons.Material.Rounded.QuestionMark,
        };
    }
}