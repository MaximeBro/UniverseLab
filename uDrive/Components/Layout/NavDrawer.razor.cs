using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Components.Layout;

public partial class NavDrawer : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;

    private UserModel _user = null!;
    private bool _docked = true;
    
    protected override async Task OnInitializedAsync()
    {
        var user = await UserService.GetUserAsync(AuthenticationState);
        if (user is null)
        {
            // TODO: Handle unauthenticated user with parent component
            return;
        }
        
        _user = user;
    }
}