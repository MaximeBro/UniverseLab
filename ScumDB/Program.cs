using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ScumDB.Components;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5005/", "http://localhost:5004/");
builder.WebHost.UseStaticWebAssets();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	options.Cookie.Name = "ScumDb.Authentication.Token";
	options.LoginPath = "/web/login";
	options.LogoutPath = "/";
});

builder.Services.AddDbContextFactory<ScumDbContext>(options =>
{
	options.UseSqlite("Data Source=../stored-data/scum.db"); // solution folder
});


builder.Services.AddMudServices();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<UserTokenHandler>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IFetchService, FetchService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Max data supported by interop calls (used for inputs)
builder.Services.AddServerSideBlazor().AddHubOptions(options => options.MaximumReceiveMessageSize = 64 * 1024);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

#pragma warning disable ASP0014
// Extends session idle time to 1h30 (experimental)
// app.UseEndpoints(e => e.MapBlazorHub(options => options.WebSockets.CloseTimeout = TimeSpan.FromMinutes(90)));
#pragma warning disable ASP0014

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.UseLogin();
app.UseLogout();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

await RunMigrationAsync<ScumDbContext>(app);

await app.RunAsync();
return;

async Task RunMigrationAsync<T>(IHost webApp) where T : DbContext
{
	var factory = webApp.Services.GetRequiredService<IDbContextFactory<T>>();
	var db = await factory.CreateDbContextAsync();
	await db.Database.MigrateAsync();
	await db.DisposeAsync();
}