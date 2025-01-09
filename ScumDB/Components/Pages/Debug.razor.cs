using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ScumDB.Components.Pages;

public partial class Debug : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

    private ClaimsPrincipal? _claims;
    
    protected override async Task OnInitializedAsync()
    {
        var state = await AuthenticationState;
        if (state.User.Identity is { IsAuthenticated: true })
        {
            _claims = state.User;
            StateHasChanged();
        }
    }
}