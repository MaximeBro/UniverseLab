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
using uDrive.Services;

namespace uDrive.Components.Components;

public partial class InteractiveItemActionsMenu
{
    [Parameter] public InteractiveItem Item { get; set; } = null!;
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public InteractiveService InteractiveService { get; set; } = null!;

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
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Title, "Placer dans la corbeille" }, { x => x.Text, $"Voulez-vous vraiment placer ce {Item.Type.Humanize()} dans la corbeille ? Il sera automatiquement supprimé au bout de 30 jours si vous ne le récupérez pas." } };
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
                    DeletionAskedAt = DateTime.UtcNow,
                });
            }
            else
            {
                db.UserFolders.Update(new UserFolder
                {
                    Id = Item.Id,
                    UserIdentifier = Item.UserIdentifier,
                    Name = Item.Name,
                    Color = Item.Color,
                    DeletionAsked = true,
                    DeletionAskedAt = DateTime.UtcNow,
                });
            }

            Item.LastEditedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
            InteractiveService.InvokeOnItemDeleted(Item);
            // TODO: Display custom toast
        }
    }
}