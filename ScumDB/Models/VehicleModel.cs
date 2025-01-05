namespace ScumDB.Models;

public class VehicleModel
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Blueprint { get; set; } = string.Empty;
	public string PositionX { get; set; } = string.Empty;
	public string PositionY { get; set; } = string.Empty;
	public string PositionZ { get; set; } = string.Empty;
	public int VehicleId { get; set; }
	public string? OwnerId { get; set; }
	public string? OwnerName { get; set; }
}