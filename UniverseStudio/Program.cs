using MudBlazor.Services;
using UniverseStudio.Components;
using UniverseStudio.Controllers;
using UniverseStudio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("https://localhost:5010/", "http://localhost:5009/");
builder.WebHost.UseStaticWebAssets();

builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.AddScoped<IFetchService, FetchService>();
builder.Services.AddScoped<SteamControllerFilter>();

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
app.UseAntiforgery();

app.MapControllers();

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();