namespace ScumDB.Models;

public class SteamAccountModel
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string SteamId { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public DateTime SavedAt { get; set; } = DateTime.Now;
}