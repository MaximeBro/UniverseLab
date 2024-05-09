using System.Text.Json.Serialization;

namespace ScumDB.Models;

public class SteamAccountModel
{
	[JsonPropertyName("id")]
	public Guid Id { get; set; } = Guid.NewGuid();
	
	[JsonPropertyName("steamId")]
	public string SteamId { get; set; } = string.Empty;
	
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;
	public DateTime SavedAt { get; set; } = DateTime.Now;
}