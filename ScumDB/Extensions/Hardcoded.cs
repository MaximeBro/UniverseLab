using MudBlazor;

namespace ScumDB.Extensions;

public static class Hardcoded
{
	public static DialogOptions DialogOptions => new DialogOptions()
	{
		CloseButton = false,
		NoHeader = true,
		DisableBackdropClick = false,
		MaxWidth = MaxWidth.Medium,
		CloseOnEscapeKey = true
	};

		
	public static Action<SnackbarOptions> SnackbarOptions = options =>
	{
		options.ShowCloseIcon = false;
		options.VisibleStateDuration = 1500;
		options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
	};
	
	public static string GetVehicleName(string blueprint)
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
			"BPC_Barba" => "Radeau",
			_ => $"Inconnu ({blueprint})"
		};
	}
}