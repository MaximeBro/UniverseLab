using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace ScumDB.Components.Components;

public partial class AuthorizedComponent
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Parameter] public RenderFragment? Content { get; set; }
    [Parameter] public string? Roles { get; set; }

    public void InvokeStateHasChanged() => StateHasChanged();
}