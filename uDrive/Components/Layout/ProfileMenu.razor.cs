using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Components.Layout;

public partial class ProfileMenu : ComponentBase
{
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;
    
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    private UserModel? _user;
    private MudMenu _menu = new();
    
    protected override async Task OnInitializedAsync()
    {
        await RefreshStateAsync();
    }

    private async Task RefreshStateAsync()
    {
        _user = await UserService.GetUserAsync(AuthenticationState);
        StateHasChanged();
    }
    
    private void Login()
    {
        NavManager.NavigateTo("/api/login", true);
    }
    
    private void Logout()
    {
        NavManager.NavigateTo("/api/logout", true);
    }
}