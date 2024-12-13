using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using uDrive.Components.Components;
using uDrive.Database;
using uDrive.Models.Views;
using uDrive.Extensions;
using uDrive.Services;

namespace uDrive.Components.Pages;

public partial class Home : AuthorizedComponentBase, IDisposable
{
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    [Inject] public InteractiveService InteractiveService { get; set; } = null!;
    
    private List<InteractiveItem> _items = [];
    private List<BreadcrumbItem> _breadcrumbItems = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (User is null) return;

        InteractiveService.OnItemAdded += AddItem;
        InteractiveService.OnItemDeleted += DeleteItem;
        
        _breadcrumbItems.Add(new BreadcrumbItem("", null, true));
        
        await using var db = await Factory.CreateDbContextAsync();
        var folders = db.UserFolders.Where(x => !x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();
        var files = db.UserFiles.Where(x => !x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();
        
        _items.AddRange(folders.Select(x => x.ToInteractiveItem()).ToList());
        _items.AddRange(files.Select(x => x.ToInteractiveItem()).ToList());
        _items = _items.OrderBy(x => x.CreatedAt).ToList();
    }

    private void AddItem(InteractiveItem item)
    {
        _items.Add(item);
        StateHasChanged();
    }
    
    private void DeleteItem(InteractiveItem item)
    {
        _items.Remove(item);
        StateHasChanged();
    }

    public void Dispose()
    {
        InteractiveService.OnItemAdded -= AddItem;
        InteractiveService.OnItemDeleted -= DeleteItem;
    }
}