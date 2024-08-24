using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;
using ScumDB.Components.Dialogs;
using ScumDB.Components.Dialogs.Vehicles;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Models;

namespace ScumDB.Components.Pages;

public partial class Vehicles
{
	[Inject] public IDbContextFactory<ScumDbContext> Factory { get; set; } = null!;
	[Inject] public IDialogService DialogService { get; set; } = null!;
	[Inject] public IJSRuntime JsRuntime { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;

	private List<VehicleModel> _vehicles = [];

	private string _search = string.Empty;
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
		await RefreshDataAsync();
	}

	private async Task AddVehicleAsync()
	{
		var instance = await DialogService.ShowAsync<SimpleAdd>(string.Empty, Hardcoded.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await RefreshDataAsync();
			StateHasChanged();
		}
	}
	
	private async Task AddVehiclesAsync()
	{
		var instance = await DialogService.ShowAsync<BulkAdd>(string.Empty, Hardcoded.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await RefreshDataAsync();
			StateHasChanged();
		}
	}

	private async Task RemoveVehicleAsync(VehicleModel vehicle)
	{
		var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment supprimer ce véhicule ?" } };
		var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await using var db = await Factory.CreateDbContextAsync();
			db.Vehicles.Remove(vehicle);
			await db.SaveChangesAsync();
			await RefreshDataAsync();
			StateHasChanged();
		}
	}

	private async Task EditVehiclesAsync()
	{
		var instance = await DialogService.ShowAsync<BulkEdit>(string.Empty, Hardcoded.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await RefreshDataAsync();
			StateHasChanged();
		}
	}
	
	private async Task PurgeVehiclesAsync()
	{
		var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment tenter une purge des véhicules ?" } };
		var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
		var result = await instance.Result;
		if (result is { Data: true })
		{
			await using var db = await Factory.CreateDbContextAsync();
			var nbDeleted = await db.Vehicles.ExecuteDeleteAsync();
			await db.SaveChangesAsync();
			await RefreshDataAsync();
			StateHasChanged();
			Snackbar.Add($"{nbDeleted} véhicules supprimés !", Severity.Success, options =>
			{
				options.VisibleStateDuration = 1500;
				options.ShowCloseIcon = false;
			});
		}
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
		await using var db = await Factory.CreateDbContextAsync();
		_vehicles = db.Vehicles.AsNoTracking().Where(x => !string.IsNullOrWhiteSpace(x.OwnerId) && x.OwnerId.StartsWith("7")).OrderBy(x => x.OwnerId).ToList();
		var accounts = db.Accounts.AsNoTracking().ToList();

		foreach (var vehicle in _vehicles)
		{
			vehicle.OwnerName = accounts.FirstOrDefault(x => x.SteamId == vehicle.OwnerId)?.Name;
		}
	}
}