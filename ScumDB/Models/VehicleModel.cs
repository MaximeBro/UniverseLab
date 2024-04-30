namespace ScumDB.Models;

public class VehicleModel
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Blueprint { get; set; } = string.Empty;
	public double PositionX { get; set; }
	public double PositionY { get; set; }
	public double PositionZ { get; set; }
	public int VehicleId { get; set; }
	public string? OwnerId { get; set; }
	public string? OwnerName { get; set; }
}