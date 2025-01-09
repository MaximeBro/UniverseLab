using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ScumDB.Services;

namespace ScumDB.Components.Layout;

public partial class AppBar
{
    [Inject] public IWebHostEnvironment Environment { get; set; } = null!;
    [Inject] public NotificationService NotificationService { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
        
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    private string _name = string.Empty;
    private string _token = string.Empty;
    private MudMenu _menu = null!;
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshStateAsync();
        NotificationService.OnAuthenticationNotificationReceived += RefreshStateAsync;
    }

    private async Task RefreshStateAsync()
    {
        var state = await AuthenticationState;
        if (state.User.Identity is { IsAuthenticated: true })
        {
            _name = state.User.FindFirst(x => x.Type == ClaimTypes.GivenName)?.Value ?? string.Empty;
            _token = state.User.FindFirst(x => x.Type == "token")?.Value ?? string.Empty;
            StateHasChanged();
        }
    }

    private async Task ToAdminPanelAsync()
    {
        await _menu.CloseMenuAsync();
        NavManager.NavigateTo("/admin/web-panel/", true);
    }
    
    private async Task LogoutAsync()
    {
        await _menu.CloseMenuAsync();
        NavManager.NavigateTo($"/api/logout/{_token}", true);
    }
}