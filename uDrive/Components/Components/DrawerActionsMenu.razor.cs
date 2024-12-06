using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using uDrive.Components.Dialogs;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models;
using uDrive.Services;

namespace uDrive.Components.Components;

public partial class DrawerActionsMenu : AuthorizedComponentBase
{
    [Parameter] public Size Size { get; set; } = Size.Medium;
    
    [Parameter]
    [Category(CategoryTypes.ComponentBase.Common)]
    public string? Class { get; set; }
    
    [Parameter] public Action<bool>? OnOpenChanged { get; set; }
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public FileService FileService { get; set; } = null!;
    
    private MudMenu _actionsMenu = null!;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task CreateDirectoryAsync()
    {
        await _actionsMenu.CloseMenuAsync();
        
        var options = Hardcoded.DialogOptions;
        options.MaxWidth = MaxWidth.Medium;
        var instance = await DialogService.ShowAsync<DirectoryDialog>(string.Empty, options);
        var result = await instance.Result;

        if (result is { Data: string name })
        {
            var folder = new UserFolder
            {
                UserIdentifier = User!.Identifier,
                Name = name,
            };
            if (FileService.CreateDirectory(folder))
            {
                await using var db = await Factory.CreateDbContextAsync();
                db.UserFolders.Add(folder);
                await db.SaveChangesAsync();
                // TODO: Send notification to update user view & display custom toast
                Snackbar.Add<Toast>(null, Severity.Normal);
            }
        }
    }

    private async Task CreateFileAsync()
    {
        await _actionsMenu.CloseMenuAsync();
    }

    private void OpenChangedHandler(bool value) => OnOpenChanged?.Invoke(value);
}