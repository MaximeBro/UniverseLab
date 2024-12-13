using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using uDrive.Components.Components;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models.Views;

namespace uDrive.Components.Pages;

public partial class Trash : AuthorizedComponentBase
{
    [Inject] public IDbContextFactory<MainDbContext> Factory { get; set; } = null!;
    
    private List<InteractiveItem> _items = [];

    private string HoveredClass => "border-dashed";
    private string SelectedClass => "border-solid";

    private string ItemClass(InteractiveItem item) =>
        item.Selected ? SelectedClass : item.Hovered ? HoveredClass : string.Empty;

    private string ItemStyle(InteractiveItem item) =>
        item.Selected || item.Hovered ? "var(--brand-0)" : "var(--background-800)";
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (User is null) return;
        
        await using var db = await Factory.CreateDbContextAsync();
        var folders = db.UserFolders.Where(x => x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();
        var files = db.UserFiles.Where(x => x.DeletionAsked && x.UserIdentifier == User!.Identifier).ToList();
        
        _items.AddRange(folders.Select(x => x.ToInteractiveItem()).ToList());
        _items.AddRange(files.Select(x => x.ToInteractiveItem()).ToList());
    }

}