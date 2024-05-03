using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using ScumDB.Databases;
using ScumDB.Models;
using ScumDB.Services;

namespace ScumDB.Components.Pages;

public partial class Admin
{
	[Inject] public IDbContextFactory<ScumDbContext> Factory { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;
	[Inject] public FetchService FetchService { get; set; } = null!;

	private string _text = string.Empty;

	private async Task TryInsertAsync()
	{
		var vehicles = new List<VehicleModel>();
		
		var lines = _text.Split("#", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
		foreach (var line in lines)
		{
			var data = line.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
			if (data.Count() >= 8 && data[7].StartsWith("7"))
			{
				try
				{
					vehicles.Add(new VehicleModel
					{
						Name = GetVehicleName(data[1]),
						Blueprint = data[1],
						OwnerId = data[7],
						VehicleId = int.Parse(data[0].Replace(":", string.Empty)),
						PositionX = data[3].Replace("X=", string.Empty),
						PositionY = data[4].Replace("Y=", string.Empty),
						PositionZ = data[5].Replace("Z=", string.Empty),
					});
				}
				catch (Exception)
				{
					continue;
				}
			}
		}

		var db = await Factory.CreateDbContextAsync();
		var toAdd = vehicles.Where(x => db.Vehicles.All(y => y.VehicleId != x.VehicleId && y.OwnerId != x.OwnerId)).ToList();
		db.Vehicles.AddRange(toAdd);

		Snackbar.Add($"{toAdd.Count()} véhicule(s) ajouté(s)", Severity.Success, options =>
		{
			options.VisibleStateDuration = 1500;
		});
		await db.SaveChangesAsync();
		await db.DisposeAsync();

		await FetchService.FetchSteamNamesAsync(vehicles.Select(x => x.OwnerId).Distinct());
	}

	private string GetVehicleName(string blueprint)
	{
		return blueprint switch
		{
			"BPC_Laika" => "Laika",
			"BPC_WolfsWagen" => "WolfsWagen",
			"BPC_Rager" => "Rager",
			"BPC_Dirtbike" => "Moto-cross",
			"BPC_MountainBike" => "VTT",
			"BPC_CityBike" => "Bicyclette",
			"BP_WheelBarrow_Metal" => "Brouette en métal",
			"BP_WheelBarrow_Improvised" => "Brouette improvisée",
			"BPC_Kinglet_Duster" => "Avion",
			"BPC_SUP" => "Paddle",
			"BPC_Barba" => "Bateau pirate",
			_ => $"Inconnu ({blueprint})"
		};
	}
}