using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs;

public class UsersHub : Hub
{
	public const string HubUrl = "/api/refresh/users";
	public const string HubMethod = "BroadcastAsync";

	public async Task BroadcastAsync(string message, Guid circuit)
	{
		await Clients.All.SendAsync(HubMethod, message, circuit);
	}
}