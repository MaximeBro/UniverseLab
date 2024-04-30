using System.Globalization;

namespace ScumDB.Extensions;

public static class UtilityExtensions
{
	public static string ToMapFormat(this double @this) => @this.ToString("0.##", CultureInfo.InvariantCulture);
}