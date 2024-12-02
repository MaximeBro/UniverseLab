using System.Globalization;
using MudBlazor;

namespace uDrive.Extensions;

public static class Hardcoded
{
    public static CultureInfo French => new CultureInfo("fr-FR");
    public static CultureInfo English => new CultureInfo("en-US");
    
    public static DialogOptions DialogOptions => new DialogOptions
    {
        CloseOnEscapeKey = true,
        BackdropClick = true,
        CloseButton = true,
        NoHeader = true
    };
}