using MudBlazor.Services;
using SignalR.Components;
using SignalR.Databases;
using SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.Services.AddSignalR();
builder.Services.AddDbContextFactory<SignalRDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapHub<UsersHub>(UsersHub.HubUrl);

app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

app.Run();