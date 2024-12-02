using uDrive.Models.Enums;

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
            return $"/ {@this.FileName}";
        }
        
        List<UserFolder> folders = [];
        var current = @this.Parent;
        while(current != null)
        {
            folders.Add(current);
            current = current.Parent;
        }

        folders.Reverse();
        return string.Join(" / ", folders.Select(x => x.Name));
    }
}