using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ScumDB.Components;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Services;
using ScumDB.Services.Hubs;

var builder = WebApplication.CreateBuilder(args);

var configPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../config/vehicles.json")).FullName;
var accountsPath = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "../config/scum-admin-accounts.json")).FullName;
builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile(accountsPath, optional: false, reloadOnChange: true);

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
builder.Services.AddControllers();

builder.Services.AddSingleton<IFetchService, FetchService>();
builder.Services.AddSingleton<IVehicleService, VehicleService>();
builder.Services.AddSingleton<UserTokenHandler>();
builder.Services.AddScoped<NotificationService>();

builder.Services.AddSignalR();

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Max data supported by interop calls (used for inputs)
builder.Services.AddServerSideBlazor().AddHubOptions(options => options.MaximumReceiveMessageSize = 10 * 1024 * 1024);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseLogin();
app.UseLogout();

app.MapHub<VehiclesHub>(VehiclesHub.HubUrl);

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