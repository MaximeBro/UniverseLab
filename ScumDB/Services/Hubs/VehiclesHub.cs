using Microsoft.AspNetCore.SignalR;
using ScumDB.Extensions;

namespace ScumDB.Services.Hubs;

public class VehiclesHub : Hub
{
    public const string HubUrl = Hardcoded.Hubs.PublicEndpoint + "/vehicles";
    public const string HubMethod = "BroadcastAsync";

    public async Task BroadcastAsync(string message, Guid senderId)
    {
        await Clients.All.SendAsync(HubMethod, message, senderId);
    }
}