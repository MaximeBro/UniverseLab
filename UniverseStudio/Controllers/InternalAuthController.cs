using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseStudio.Database;
using UniverseStudio.Extensions;
using UniverseStudio.Models;
using UniverseStudio.Services;

namespace UniverseStudio.Controllers;

[Route("/api/v1/auth/internal")]
public class InternalAuthController : AuthenticationController
{
    public InternalAuthController(TokensCacheService tokensCacheService, IDbContextFactory<MainDbContext> factory) 
        : base(tokensCacheService, factory) { }

    [HttpPost("/login")]
    public async Task<IActionResult> LoginAsync([FromBody] string email, [FromBody] string password)
    {
        await using var db = await Factory.CreateDbContextAsync();
        var existingUser = await db.Users.FirstOrDefaultAsync(x => x.Email == email);
        if (existingUser != null)
        {
            var verified = UserHelper.VerifyPassword(password, existingUser.PasswordHash);
            if (verified)
            {
                var claims = UserHelper.GetUserClaims(existingUser, AuthType.Internal);
                await HttpContext.SignInAsync(InternalDefaults.AuthenticationScheme, claims);
                
                var token = new UserTokenModel() { User = existingUser, Type = AuthType.Internal };
                TokensCache.AddToken(existingUser.InternalIdentifier, token);
                
                return Ok(token);
            }
        }

        return Unauthorized();
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] string internalId)
    {
        var token = TokensCache.RemoveToken(internalId);
        if (token != null)
        {
            if (token.Type is AuthType.Internal)
            {
                await HttpContext.SignOutAsync(InternalDefaults.AuthenticationScheme);
                return Ok();
            }

            return BadRequest($"The requested token doesn't depend on this authentication type. Current token authentication type: {token.Type.Humanize()}.");
        }

        return BadRequest("This internalId doesn't have any valid token right now.");
    }
}