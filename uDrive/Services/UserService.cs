using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models;

namespace uDrive.Services;

public class UserService(IDbContextFactory<MainDbContext> factory, IConfiguration configuration, ILogger<UserService> logger, FileService fileService)
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="newUser"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task InitUserAsync(UserModel newUser)
    {
        await using var db = await factory.CreateDbContextAsync();
        db.Users.Add(newUser);
        db.UserMainFolders.Add(new UserMainFolder
        {
            UserIdentifier = newUser.Identifier,
            MaxStorage = long.Parse(configuration["Files:MaxStorage"] ?? FileUtils.DefaultMaxStorage.ToString()),
            MaxSubFolders = int.Parse(configuration["Files:MaxSubFolders"] ?? "5"),
        });
        
        await db.SaveChangesAsync();
        var result = await fileService.InitUserFolderAsync(newUser);

        if (result != null)
        {
            logger.LogError(result.Message);
            throw result;
        }
    }
    
    /// <summary>
    /// Retrieves the user claims from its <see cref="AuthenticationState"/> and create an instance of <see cref="UserModel"/>.
    /// </summary>
    /// <param name="authenticationStateTask">The authentication state of the user.</param>
    /// <returns>A <see cref="UserModel"/> if the user is authenticated. Otherwise, null.</returns>
    public async Task<UserModel?> GetUserAsync(Task<AuthenticationState> authenticationStateTask)
    {
        var state = await authenticationStateTask;
        if (state.User.Identity is { IsAuthenticated: true })
        {
            var user = new UserModel
            {
                Identifier = GetClaimValue(state.User, ClaimTypes.NameIdentifier),
                GivenName = GetClaimValue(state.User, ClaimTypes.Name),
                Email = GetClaimValue(state.User, ClaimTypes.Email),
                Firstname = GetClaimValue(state.User, ClaimTypes.GivenName),
                Lastname = GetClaimValue(state.User, ClaimTypes.Surname),
                Profile = await GetProfilePictureAsync(GetClaimValue(state.User, "urn:google:image"))
            };

            return user;
        }

        return null;
    }

    /// <summary>
    /// Retrieves the saved user from database with its identifier.
    /// </summary>
    /// <param name="identifier">The Google identifier of the user as a string.</param>
    /// <returns>A <see cref="UserModel"/> if the user is stored in database. Otherwise, null.</returns>
    public async Task<UserModel?> GetSavedUserAsync(string identifier)
    {
        await using var db = await factory.CreateDbContextAsync();
        return await db.Users.FirstOrDefaultAsync(x => x.Identifier == identifier);
    }
    
    /// <summary>
    /// Retrieves the user claims from and create an instance of <see cref="UserModel"/>.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> of the user.</param>
    /// <returns>A <see cref="UserModel"/> if the user is authenticated. Otherwise, null.</returns>
    public async Task<UserModel?> GetUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is { IsAuthenticated: true })
        {
            var user = new UserModel
            {
                Identifier = GetClaimValue(claimsPrincipal, ClaimTypes.NameIdentifier),
                GivenName = GetClaimValue(claimsPrincipal, ClaimTypes.Name),
                Email = GetClaimValue(claimsPrincipal, ClaimTypes.Email),
                Firstname = GetClaimValue(claimsPrincipal, ClaimTypes.GivenName),
                Lastname = GetClaimValue(claimsPrincipal, ClaimTypes.Surname),
                Profile = await GetProfilePictureAsync(GetClaimValue(claimsPrincipal, "urn:google:image"))
            };

            return user;
        }

        return null;
    }

    private string GetClaimValue(ClaimsPrincipal claimsPrincipal, string claimType)
    {
        return claimsPrincipal.FindFirstValue(claimType) ?? string.Empty;
    }

    /// <summary>
    /// Recovers user's profile picture with <see cref="HttpClient"/> and converts it into base64 string.
    /// </summary>
    /// <param name="url">The url of the user's profile picture.</param>
    /// <returns>A base64 string if it succeeded. Otherwise, null.</returns>
    private async Task<string?> GetProfilePictureAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            byte[] imageData = await client.GetByteArrayAsync(url);
            return Convert.ToBase64String(imageData);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}