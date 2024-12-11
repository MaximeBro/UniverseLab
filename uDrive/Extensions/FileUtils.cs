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

    public static readonly string[] SupportedLengthFormats = ["o", "Ko", "Mo", "Go", "To"];
    public static readonly string[] CodeExtensions = ["cs", "razor", "dll", "exe", "json", "xml", "xaml", "java", "js", "jar", "ts", "tsx", "c", "py", "cshtml"];
    public static readonly string[] ImageExtensions = ["png", "jpg", "jpeg", "webp", "gif", "apng", "avif", "svg", "bmp", "ico", "tiff"];
    public static readonly string[] VideoExtensions = ["mp4", "mov", "avi", "wmv", "avchd", "webm", "flv"];
    public static readonly string[] MusicExtensions = ["mp3", "wav", "acc", "flac", "ogg"];

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
            return $"./{@this.FileName}";
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
            UserIdentifier = @this.UserIdentifier,
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
            UserIdentifier = @this.UserIdentifier,
            Name = @this.FileName,
            ParentId = @this.ParentId,
            Parent = @this.Parent,
            Type = ItemType.File,
            Color = @this.Color
        };
    }

    public static string ExtractExtension(this string @this) => @this.Split(".").LastOrDefault(string.Empty);
    
    public static string IconByExtension(this string @this)
    {
        var extension = @this.ExtractExtension();
        return extension switch
        {
            "txt" => Icons.Custom.FileFormats.FileDocument,
            "xlsx" => Icons.Custom.FileFormats.FileExcel,
            "docx" => Icons.Custom.FileFormats.FileWord,
            "pdf" => Icons.Custom.FileFormats.FilePdf,
            var x when ImageExtensions.Contains(x) => Icons.Custom.FileFormats.FileImage,
            var x when MusicExtensions.Contains(x) => Icons.Custom.FileFormats.FileMusic,
            var x when VideoExtensions.Contains(x) => Icons.Custom.FileFormats.FileVideo,
            var x when CodeExtensions.Contains(x) => Icons.Custom.FileFormats.FileCode,
            _ => Icons.Material.Rounded.QuestionMark
        };
    }

    public static FileType FileTypeByExtension(this string @this)
    {
        return @this.ExtractExtension() switch
        {
            "txt"  or "xlsx" or "docx" or "pdf" => FileType.Document,
            "zip" or "tar" or "7zip" or "targz" => FileType.Archive, 
            var x when ImageExtensions.Contains(x) => FileType.Image,
            var x when MusicExtensions.Contains(x) => FileType.Audio,
            var x when VideoExtensions.Contains(x) => FileType.Video,
            var x when CodeExtensions.Contains(x) => FileType.Code,
            _ => FileType.Unknown
        };
    }

    public static string Icon(this InteractiveItem @this)
    {
        return @this.Type switch
        {
            ItemType.File => @this.Name.IconByExtension(),
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