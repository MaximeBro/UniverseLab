using Microsoft.AspNetCore.Components;

namespace UniverseStudio.Components.Pages;

public partial class HomePage : ComponentBase
{
    [Inject] public IConfiguration Configuration { get; set; } = null!;

    private bool steamApiAvailable;
    
    protected override void OnInitialized()
    {
        steamApiAvailable = !bool.Parse(Configuration.GetSection("Controllers")["maintenance"]!);
    }
}