using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using ScumDB.Components;
using ScumDB.Databases;
using ScumDB.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5005/", "http://localhost:5004/");

builder.Services.AddDbContextFactory<ScumDbContext>(options =>
{
	options.UseSqlite("Data Source=./stored-data/scum.db");
});

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor().AddHubOptions(options => options.MaximumReceiveMessageSize = 64 * 1024);
builder.Services.AddMudServices();
builder.Services.AddHttpClient();

builder.Services.AddScoped<FetchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

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