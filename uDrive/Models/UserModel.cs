namespace uDrive.Models.Enums;

public class UserModel
{
    /// <summary>
    /// Google given identifier.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name (i.e. Firstname Lastname).
    /// </summary>
    public string GivenName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// Profile picture of the user converted to base64. Can be null.
    /// </summary>
    public string? Profile { get; set; }
    
    /// <summary>
    /// uDrive given role, this is not related to Google.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
}