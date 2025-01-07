using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UniverseStudio.Models;
using BC = BCrypt.Net.BCrypt;

namespace UniverseStudio.Extensions;

public static class UserHelper
{
    public static async Task<UserModel> GetUserAsync(ClaimsPrincipal claimsPrincipal, AuthType authType)
    {
        return new UserModel()
        {
            InternalIdentifier = GetClaimValue(claimsPrincipal, "uid"),
            Identifier = GetClaimValue(claimsPrincipal, ClaimTypes.NameIdentifier),
            GivenName = GetClaimValue(claimsPrincipal, ClaimTypes.Name),
            Email = GetClaimValue(claimsPrincipal, ClaimTypes.Email),
            PasswordHash = authType == AuthType.Google ? string.Empty : GetClaimValue(claimsPrincipal, ClaimTypes.Hash),
            Profile = authType == AuthType.Google ? await GetProfilePictureAsync(GetClaimValue(claimsPrincipal, "urn:google:image")) : GetClaimValue(claimsPrincipal, "avatar")
        };
    }
    
    public static ClaimsPrincipal GetUserClaims(UserModel user, AuthType authType)
    {
        var claims = new List<Claim>()
        {
            new Claim("uid", user.InternalIdentifier),
            new Claim(ClaimTypes.NameIdentifier, user.Identifier),
            new Claim(ClaimTypes.Name, user.GivenName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Hash, user.PasswordHash),
            new Claim(authType == AuthType.Google ? "urn:google:image" : "avatar", user.Profile ?? string.Empty)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims));
    }
    
    public static string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal.FindFirstValue(claimType) ?? string.Empty;
    }

    /// <summary>
    /// Recovers user's profile picture with <see cref="HttpClient"/> and converts it into base64 string.
    /// </summary>
    /// <param name="url">The url of the user's profile picture.</param>
    /// <returns>A base64 string if it succeeded. Otherwise, null.</returns>
    private static async Task<string?> GetProfilePictureAsync(string url)
    {
        if (!url.StartsWith("https")) return url;
        
        try
        {
            using var client = new HttpClient();
            byte[] imageData = await client.GetByteArrayAsync(url);
            return Convert.ToBase64String(imageData);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
    public static string HashEmail(string email)
    {
        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(email));
        
        return Convert.ToBase64String(hashBytes)
            .Replace("/", "_")
            .Replace("+", "-")
            .TrimEnd('=');
    }

    public static string HashPassword(string password)
    {
        return BC.HashPassword(password);
    }

    public static bool VerifyPassword(string password, string hash)
    {
        return BC.Verify(password, hash);
    }
}