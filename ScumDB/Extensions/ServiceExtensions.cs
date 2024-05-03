using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ScumDB.Services;

namespace ScumDB.Extensions;

public static class ServiceExtensions
{

	public static void UseLogin(this WebApplication @this)
	{
		@this.Map("/api/login/{token:guid}", async (HttpContext context, UserTokenHandler tokenHandler, Guid token) =>
		{
			if (!tokenHandler.Tokens.TryGetValue(token, out var claims))
			{
				return Results.Unauthorized();
			}
			
			var authProperties = new AuthenticationProperties { AllowRefresh = true, IsPersistent = true };
			await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims, authProperties);

			return Results.Redirect("/");
		});
	}
	
	public static void UseLogout(this WebApplication @this)
	{
		@this.Map("/api/logout/{token:guid}", async (HttpContext context, UserTokenHandler tokenHandler, Guid token) =>
		{
			tokenHandler.Tokens.Remove(token);
			await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			
			return Results.Redirect("/");
		});
	}
}