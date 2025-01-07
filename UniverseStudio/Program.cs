using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using UniverseStudio.Components;
using UniverseStudio.Controllers;
using UniverseStudio.Database;
using UniverseStudio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
	}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
	{
		options.Cookie.Name = "UniverseStudio.Auth";
		options.LoginPath = "/api/v1/auth/login";
		options.LogoutPath = "/api/v1/auth/logout";
	})
	.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
	{
		options.ClientId = builder.Configuration["Google:ClientId"]!;
		options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;
		options.ClaimActions.MapJsonKey("urn:google:profile", "link");
		options.ClaimActions.MapJsonKey("urn:google:image", "picture");
		options.SaveTokens = true;
	});


builder.Services.AddScoped<IFetchService, FetchService>();
builder.Services.AddScoped<SteamControllerFilter>();
builder.Services.AddSingleton<TokensCacheService>();

builder.Services.AddDbContextFactory<MainDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("MainDb")));

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

await app.RunAsync();