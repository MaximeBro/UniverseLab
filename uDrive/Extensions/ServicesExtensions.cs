using System.Security.Claims;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using uDrive.Database;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Extensions;

public static class ServicesExtensions
{
    /// <summary>
    /// Used to dynamically migrate project's databases when the app is launched (i.e. creates .db files in the respective folder)
    /// </summary>
    /// <param name="this">The web application as <see cref="IHost"/>.</param>
    /// <typeparam name="T">Type of your database.</typeparam>
    public static async Task RunMigrationAsync<T>(this IHost @this) where T : DbContext
    {
        var factory = @this.Services.GetRequiredService<IDbContextFactory<T>>();
        var db = await factory.CreateDbContextAsync();
        await db.Database.MigrateAsync();
        await db.DisposeAsync();
    }

    public static void UseGoogleAuthentication(this WebApplication @this)
    {
        @this.UseGoogleProvider();
    }

    private static void UseGoogleProvider(this WebApplication @this)
    {
        @this.Map("/api/login", async (HttpContext context) =>
        {
            await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = "/api/challenge/google",
            });
        });
        
        @this.Map("/api/challenge/google", async (HttpContext context, IDbContextFactory<MainDbContext> mainDbContext, UserService userService) =>
        {
            context.Response.Redirect("/");
            
            if (context.User.Identity is ClaimsIdentity { IsAuthenticated: true } userIdentity)
            {
                var user = await userService.GetUserAsync(context.User);
                if (user is null) return;

                var savedUser = await userService.GetSavedUserAsync(user.Identifier);
                var role = UserRole.User;
                
                if (savedUser is null)
                {
                    user.Role = role;
                    await userService.InitUserAsync(user);
                }
                else
                {
                    role = savedUser.Role;
                }
                
                userIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Humanize()));
                context.User = new ClaimsPrincipal(userIdentity);
            }
        });

        @this.Map("/api/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            context.Response.Redirect("/");
        });
    }
}