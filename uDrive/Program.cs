using MudBlazor.Services;
using uDrive.Components;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:5005/", "http://localhost:5004/");
builder.WebHost.UseStaticWebAssets();

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Max data supported by interop calls (used for inputs)
builder.Services.AddServerSideBlazor().AddHubOptions(options => options.MaximumReceiveMessageSize = 1 * 1024 * 1024); // 1 MB

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
return;