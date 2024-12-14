using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using uDrive.Components.Components;
using uDrive.Components.Dialogs;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models;
using uDrive.Models.Enums;
using uDrive.Models.Views;
using uDrive.Services;

namespace uDrive.Components.Pages;

public partial class Trash : AuthorizedComponentBase
{
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public FileService FileService { get; set; } = null!;

    private List<InteractiveItem> _items = [];

    private string HoveredClass => "border-dashed";
    private string SelectedClass => "border-solid";

    private string ItemClass(InteractiveItem item) =>
        item.Selected ? SelectedClass : item.Hovered ? HoveredClass : string.Empty;

    private string ItemStyle(InteractiveItem item) =>
        item.Selected || item.Hovered ? "var(--brand-0)" : "var(--background-800)";

    private List<InteractiveItem> SelectedItems => _items.Where(x => x.Selected).ToList();

    private string SelectedItemsText => SelectedItems.Count > 0
        ? $"{SelectedItems.Count} {(SelectedItems.Count > 1 ? "éléments sélectionné(s)" : "élément sélectionné")}"
        : string.Empty;

    private string SelectButtonIcon => SelectedItems.Count > 0
        ? Icons.Material.Rounded.IndeterminateCheckBox
        : Icons.Material.Rounded.SelectAll;

    private string SelectButtonTooltip => SelectedItems.Count > 0 ? "Annuler la sélection" : "Tout sélectionner";

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        if (User is null) return;
        
        await using var db = await Factory.CreateDbContextAsync();
        var folders = db.UserFolders.Where(x => x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();
        var files = db.UserFiles.Where(x => x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();

        _items.Clear();
        _items.AddRange(folders.Select(x => x.ToInteractiveItem()).ToList());
        _items.AddRange(files.Select(x => x.ToInteractiveItem()).ToList());
        StateHasChanged();
    }
        
    private void HandleSelection()
    {
        var selected = SelectedItems.Count == 0;
        _items.ForEach(x => x.Selected = selected);
        StateHasChanged();
    }

    private async Task ForciblyDeleteAsync()
    {
        var text = SelectedItems.Count > 1
            ? "Voulez-vous vraiment supprimer ces éléments ? Vous ne pourrez pas les récupérer."
            : "Voulez-vous vraiment supprimer cet élément ? Vous ne pourrez pas le récupérer.";
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Title, "Suppression définitive" }, { x => x.Text, text } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(null, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;

        if (result is { Data: true })
        {
            var files = SelectedItems.Where(x => x.Type == ItemType.File).Select(x => x.ToUserFile()).ToList();
            var folders = SelectedItems.Where(x => x.Type == ItemType.Folder).Select(x => x.ToUserFolder()).ToList();
            await using var db = await Factory.CreateDbContextAsync();
            db.UserFiles.RemoveRange(files);
            db.UserFolders.RemoveRange(folders);
            await db.SaveChangesAsync();

            await InvokeAsync(() =>
            {
                folders.ForEach(x => FileService.FinalDeleteDirectory(x));
                files.ForEach(x => FileService.FinalDeleteFile(x));
            });
            
            _items = _items.Where(x => !x.Selected).ToList();
            StateHasChanged();
            // TODO: Display custom toast
        }
    }

    private async Task RestoreAsync()
    {
        var files = SelectedItems.Where(x => x.Type == ItemType.File).Select(x => x.ToUserFile()).ToList();
        var folders = SelectedItems.Where(x => x.Type == ItemType.Folder).Select(x => x.ToUserFolder()).ToList();

        files.ForEach(x =>
        {
            x.DeletionAsked = false;
            x.DeletionAskedAt = null;
        });
        
        folders.ForEach(x =>
        {
            x.DeletionAsked = false;
            x.DeletionAskedAt = null;
        });

        await using var db = await Factory.CreateDbContextAsync();
        db.UserFiles.UpdateRange(files);
        db.UserFolders.UpdateRange(folders);
        await db.SaveChangesAsync();

        _items = _items.Where(x => !x.Selected).ToList();
        StateHasChanged();
        
        // TODO: Display custom toast
    }
}