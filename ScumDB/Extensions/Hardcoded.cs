using System.Globalization;
using MudBlazor;
using ScumDB.Components.Pages;

namespace ScumDB.Extensions;

public static class Hardcoded
{
	public static CultureInfo French => new CultureInfo("fr-FR");
	
	public static readonly DialogOptions DialogOptions = new DialogOptions
	{
		CloseButton = false,
		NoHeader = true,
		BackdropClick = true,
		MaxWidth = MaxWidth.Medium,
		CloseOnEscapeKey = true
	};
		
	public static readonly Action<SnackbarOptions> SnackbarOptions = options =>
	{
		options.ShowCloseIcon = false;
		options.VisibleStateDuration = 1500;
		options.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
	};

	public static readonly BlockMask SteamIdMask = new BlockMask("", 
		new Block('7'), new Block('6'), new Block('5'), new Block('6'), new Block('1', 2, 2), // 765611
					new Block('0', 11, 11)); // Unique identifier represented as 11 digits
	
	public static readonly PatternMask PositionMask = new PatternMask("000000.0000");
	
	public static readonly PatternMask VehicleIdMask = new PatternMask("00000") { Placeholder = '_' };
	
	public static string GetVehicleName(string blueprint, IConfiguration configuration)
	{
		var name = configuration[blueprint] ?? string.Empty;
		return string.IsNullOrWhiteSpace(name) ? $"Inconnu {blueprint}" : name;
	}

	public static AggregateDefinition<T> GetAggregateCountOf<T>() => new()
	{
		Type = AggregateType.Count,
		DisplayFormat = "{value} éléments sélectionnés"
	};
}