using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using uDrive.Components.Dialogs;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models.Enums;
using uDrive.Services;

namespace uDrive.Components.Layout;

public partial class NavDrawer : ComponentBase
{
    [CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public NavigationManager NavManager { get; set; } = null!;
    [Inject] public UserService UserService { get; set; } = null!;
    [Inject] public FileService FileService { get; set; } = null!;

    private UserModel _user = null!;
    private UserMainFolder _mainFolder = null!;
    
    protected override async Task OnInitializedAsync()
    {
        var user = await UserService.GetUserAsync(AuthenticationState);
        var mainFolder = await FileService.GetUserMainFolderAsync(user);
        if (user is null || mainFolder is null)
        {
            NavManager.NavigateTo("/", true);
            return;
        }
        
        _user = user;
        _mainFolder = mainFolder;
    }

    private async Task CreateDirectoryAsync()
    {
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.Medium;
        var instance = await DialogService.ShowAsync<DirectoryDialog>(string.Empty, options);
        var result = await instance.Result;

        if (result is { Data: string name })
        {
            await using var db = await Factory.CreateDbContextAsync();
            db.UserFolders.Add(new UserFolder
            {
                UserIdentifier = _user.Identifier,
                Name = name,
                ParentId = _mainFolder.Id,
            });
            await db.SaveChangesAsync();
            // TODO: Send notification to update user view
        }
    }
}