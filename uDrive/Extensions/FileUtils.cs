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
        if (@this < 1000) return "-"; 
            
        int order = @this > 0 ? (int)Math.Floor(Math.Log(@this, 1024)) : 0;
        double size = @this / Math.Pow(1024, order);
        return $"{size:0.##} {SupportedLengthFormats[order]}";
    }
}