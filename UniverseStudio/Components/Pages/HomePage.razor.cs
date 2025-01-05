using Microsoft.AspNetCore.Components;

namespace UniverseStudio.Components.Pages;

public partial class HomePage : ComponentBase
{
    [Inject] public IConfiguration Configuration { get; set; } = null!;

    private bool _steamApiAvailable;
    private bool _scumAppAvailable;
    private string _scumAppVersion = string.Empty;

    private HttpClient _client = null!;
    
    protected override async Task OnInitializedAsync()
    {
        _steamApiAvailable = !bool.Parse(Configuration["maintenance"]!);
        _client = new HttpClient();
        _scumAppAvailable = await GetMaintenanceAsync("https://localhost:5005") ?? true;
        _scumAppVersion = await GetVersionAsync("https://localhost:5005") ?? "1.0.0";
    }

    private async Task<bool?> GetMaintenanceAsync(string appUrl)
    {
        var response = await _client.GetAsync($"{appUrl}/api/version/Maintenance");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (bool.TryParse(content, out bool isAvailable))
            {
                return !isAvailable;
            }
        }

        return null;
    }
    
    private async Task<string?> GetVersionAsync(string appUrl)
    {
        var response = await _client.GetAsync($"{appUrl}/api/version/GetVersion");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        return null;
    }
}