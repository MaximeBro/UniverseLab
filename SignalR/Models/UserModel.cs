namespace SignalR.Models;

public class UserModel
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string Firstname { get; set; } = string.Empty;
	public string Lastname { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
}