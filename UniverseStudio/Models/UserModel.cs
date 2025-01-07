using System.ComponentModel.DataAnnotations;

namespace UniverseStudio.Models;

public class UserModel
{
    [MaxLength(8)]
    public required string InternalIdentifier { get; init; }
    
    [MaxLength(200)]
    public required string Identifier { get; init; }

    [MaxLength(200)]
    public required string GivenName { get; init; }

    [MaxLength(300)]
    public required string Email { get; init; }
    
    [MaxLength(300)]
    public required string PasswordHash { get; init; }
    
    [MaxLength(512)]
    public string? Profile { get; init; }

    public UserRole Role { get; init; }
}