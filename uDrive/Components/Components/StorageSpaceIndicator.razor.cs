using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using uDrive.Extensions;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Components.Components;

public partial class StorageSpaceIndicator : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;
    [Parameter] public Size Size { get; set; } = Size.Medium;
    [Parameter]
    [Category(CategoryTypes.ComponentBase.Common)]
    public string? Class { get; set; }
    
    [Inject] public IConfiguration Configuration { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;
    [Inject] public FileService FileService { get; set; } = null!;

    private UserModel? _user;
    private long _currentStorage;
    private long _maxStorage;

    protected override async Task OnInitializedAsync()
    {
        _currentStorage = 0L;
        _maxStorage = long.Parse(Configuration["Files:MaxStorage"] ?? FileUtils.DefaultMaxStorage.ToString());
        
        _user = await UserService.GetUserAsync(AuthenticationStateTask);
        if (_user != null)
        {
            var files = await FileService.GetUserFilesAsync(_user);
            _currentStorage = files.Sum(x => x.FileSize);
            _maxStorage = (await FileService.GetUserMainFolderAsync(_user))?.MaxStorage ?? 0L;
        }
        
        StateHasChanged();
    }
}