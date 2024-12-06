using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Components.Components;

public partial class AuthorizedComponentBase : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;
    
    [Parameter] public RenderFragment? Content { get; set; }
    [Parameter] public string Roles { get; set; } = "User, Admin";

    protected UserModel? User { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        User = await UserService.GetUserAsync(AuthenticationState);
    }
}