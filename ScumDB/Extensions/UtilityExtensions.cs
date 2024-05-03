using System.Globalization;

namespace ScumDB.Extensions;

public static class UtilityExtensions
{
	public static string ToMapFormat(this double @this) => Math.Round(@this, 2).ToString(CultureInfo.InvariantCulture).Replace(",", ".");
}