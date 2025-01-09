using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniverseStudio.Database;
using UniverseStudio.Extensions;
using UniverseStudio.Models;
using UniverseStudio.Services;

namespace UniverseStudio.Controllers;

[Route("/api/v1/auth/google")]
public class GoogleAuthController : AuthenticationController
{
    public GoogleAuthController(TokensCacheService tokensCacheService, IDbContextFactory<MainDbContext> factory) 
        : base(tokensCacheService, factory) { }
    
    [HttpPost("login")]
    public async Task LoginAsync([FromBody] string callbackUri)
    {
        await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
        {
            RedirectUri = $"/api/v1/auth/google/challenge?callbackUri={Uri.EscapeDataString(callbackUri)}"
        });
    }

    [HttpPost("challenge")]
    public async Task<IActionResult> ChallengeAsync([FromQuery] string callbackUri)
    {
        if (string.IsNullOrWhiteSpace(callbackUri)) return BadRequest();
        
        if (HttpContext.User.Identity is ClaimsIdentity { IsAuthenticated: true } identity)
        {
            var email = UserHelper.GetClaimValue(HttpContext.User, ClaimTypes.Email);
            var hash = UserHelper.HashEmail(email);
            
            identity.AddClaim(new Claim("uid", hash));
            HttpContext.User = new ClaimsPrincipal(identity);
            
            await using var db = await Factory.CreateDbContextAsync();
            var existingUser = await db.Users.FirstOrDefaultAsync(x => x.InternalIdentifier == hash);
            if (existingUser is null)
            {
                existingUser = await UserHelper.GetUserAsync(HttpContext.User, AuthType.Google);
                db.Users.Add(existingUser);
                await db.SaveChangesAsync();
            }
            
            var token = new UserTokenModel() { User = existingUser, Type = AuthType.Custom };
            TokensCache.AddToken(existingUser.InternalIdentifier, token);
            // TODO: Check if user is already authentified
            
            return Ok(token);
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
                await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
                return Ok();
            }

            return BadRequest($"The requested token doesn't depend on this authentication type. Current token authentication type: {token.Type.Humanize()}.");
        }

        return BadRequest("This internalId doesn't have any valid token right now.");
    }
}