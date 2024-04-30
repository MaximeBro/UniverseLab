using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Models;
using ScumDB.Services;

namespace ScumDB.Components.Pages;

public partial class Vehicles
{
	[Inject] public IDbContextFactory<ScumDbContext> Factory { get; set; } = null!;
	[Inject] public IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;

	private List<VehicleModel> _vehicles = new();

	private string _search = string.Empty;
	private Func<VehicleModel, bool> QuickFilter => x =>
	{
		if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (!string.IsNullOrWhiteSpace(x.OwnerName) && x.OwnerName.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.Blueprint.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
		if (x.OwnerId != null && x.OwnerId.ToString()!.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
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
		var db = await Factory.CreateDbContextAsync();
		_vehicles = db.Vehicles.AsNoTracking().Where(x => !string.IsNullOrWhiteSpace(x.OwnerId) && x.OwnerId.StartsWith("7")).OrderBy(x => x.OwnerId).ToList();
		var accounts = db.Accounts.AsNoTracking().ToList();
		await db.DisposeAsync();

		foreach (var vehicle in _vehicles)
		{
			vehicle.OwnerName = accounts.FirstOrDefault(x => x.SteamId == vehicle.OwnerId)?.Name;
		}
	}

	private async Task CopyToClipboardAsync(VehicleModel vehicle)
	{
		var coords = $"{vehicle.PositionX.ToMapFormat()},{vehicle.PositionY.ToMapFormat()},{vehicle.PositionZ.ToMapFormat()}";
		await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", coords);
		Snackbar.Add("Coordonnées copiées dans le presse-papier", Severity.Info, options =>
		{
			options.VisibleStateDuration = 1500;
			options.ShowCloseIcon = false;
			options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
		});
	}

	private async Task OpenMapAsync(VehicleModel vehicle)
	{
		var coords = $"{vehicle.PositionX.ToMapFormat()},{vehicle.PositionY.ToMapFormat()},{vehicle.PositionZ.ToMapFormat()}";
		await JsRuntime.InvokeVoidAsync("open", $"https://scum-map.com/en/map/{coords}", "_blank");
	}
}