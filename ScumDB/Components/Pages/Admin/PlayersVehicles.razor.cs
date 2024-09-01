using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using ScumDB.Databases;
using ScumDB.Models;
using ScumDB.Services;

namespace ScumDB.Components.Pages.Admin;

public partial class PlayersVehicles
{
    [Inject] public IDbContextFactory<ScumDbContext> Factory { get; set; } = null!;
    [Inject] public IVehicleService VehicleService { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    
    private List<VehicleModel> _models = [];
    private string _search = string.Empty;

    private List<BreadcrumbItem> _breadcrumbItems = [];

    private Func<VehicleModel, bool> QuickFilter => x =>
    {
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.OwnerName) && x.OwnerName.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Blueprint.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.OwnerId != null && x.OwnerId.ToString()!.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (!string.IsNullOrWhiteSpace(x.OwnerName) && x.OwnerName.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.VehicleId.ToString().Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		
        return false;
    };
    
    private AggregateDefinition<VehicleModel> Aggregation => new()
    {
        Type = AggregateType.Count,
        DisplayFormat = "{value} éléments sélectionnés"
    };
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems = new List<BreadcrumbItem>(new[]
        {
            new BreadcrumbItem("Admin", null, disabled: true),
            new BreadcrumbItem("WebPanel", "/admin/web-panel/"),
            new BreadcrumbItem("Véhicules", null, disabled: true)
        });
        await RefreshDataAsync();
    }
    
    private async Task CopyToClipboardAsync(VehicleModel vehicle)
    {
        var coords = $"{vehicle.PositionX},{vehicle.PositionY},{vehicle.PositionZ}";
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", coords);
        Snackbar.Add("Coordonnées copiées dans le presse-papier", Severity.Info, options =>
        {
            options.VisibleStateDuration = 1500;
            options.ShowCloseIcon = false;
            options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
        });
    }

    private async Task CopyToClipboardAsync(int id)
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", id);
        Snackbar.Add("ID copié dans le presse-papier", Severity.Info, options =>
        {
            options.VisibleStateDuration = 1500;
            options.ShowCloseIcon = false;
            options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
        });
    }
    
    private async Task OpenMapAsync(VehicleModel vehicle)
    {
        var coords = $"{vehicle.PositionX},{vehicle.PositionY},{vehicle.PositionZ}";
        await JsRuntime.InvokeVoidAsync("open", $"https://scum-map.com/en/map/place/{coords}", "_blank");
    }

    private async Task RefreshDataAsync()
    {
        _models = await VehicleService.GetAllAsync();
    }
}