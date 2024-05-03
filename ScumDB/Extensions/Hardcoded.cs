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
}