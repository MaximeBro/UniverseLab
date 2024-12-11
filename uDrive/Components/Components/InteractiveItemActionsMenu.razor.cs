using Humanizer;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Utilities;
using uDrive.Components.Dialogs;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models;
using uDrive.Models.Enums;
using uDrive.Models.Views;

namespace uDrive.Components.Components;

public partial class InteractiveItemActionsMenu
{
    [Parameter] public InteractiveItem Item { get; set; } = null!;
    [Parameter] public Action<InteractiveItem>? OnDelete { get; set; }
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;

    private MudMenu _menu = null!;
    private MudColor _color = null!;

    protected override void OnInitialized()
    {
        _color = new MudColor(Item.Color);
        StateHasChanged();
    }

    private async Task OnColorChangedAsync(string color)
    {
        await using var db = await Factory.CreateDbContextAsync();
        Item.Color = color;
        if (Item.Type == ItemType.File)
        {
            db.UserFiles.Update(new UserFile
            {
                Id = Item.Id,
                UserIdentifier = Item.UserIdentifier,
                FileName = Item.Name,
                Color = color
            });
        }
        else
        {
            db.UserFolders.Update(new UserFolder
            {
                Id = Item.Id,
                UserIdentifier = Item.UserIdentifier,
                Name = Item.Name,
                Color = color
            });
        }

        await db.SaveChangesAsync();
    }

    private async Task DeleteItemAsync()
    {
        await _menu.CloseMenuAsync();
        await using var db = await Factory.CreateDbContextAsync();
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Title, "Supprimer" }, { x => x.Text, $"Voulez-vous vraiment supprimer ce {Item.Type.Humanize()} ?" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(null, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        
        if (result is { Data: true })
        {
            if (Item.Type == ItemType.File)
            {
                db.UserFiles.Update(new UserFile
                {
                    Id = Item.Id,
                    UserIdentifier = Item.UserIdentifier,
                    FileName = Item.Name,
                    Color = Item.Color,
                    DeletionAsked = true,
                    DeletionAskedAt = DateTime.Now,
                });
            }
            else
            {
                db.UserFolders.Remove(new UserFolder
                {
                    Id = Item.Id,
                    UserIdentifier = Item.UserIdentifier,
                    Name = Item.Name,
                    Color = Item.Color,
                    DeletionAsked = true,
                    DeletionAskedAt = DateTime.Now,
                });
            }

            await db.SaveChangesAsync();
            OnDelete?.Invoke(Item);
            // TODO: Notify view
        }
    }
}