namespace UniverseStudio.Models;

public class UserTokenModel
{
    public DateTime LastAuthenticatedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan MaxSessionDuration { get; set; } = TimeSpan.FromMinutes(30);
    public required UserModel User { get; init; }
    public AuthType Type { get; set; } = AuthType.Internal;
}