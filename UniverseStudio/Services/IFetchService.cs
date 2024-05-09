using UniverseStudio.Models;
using UniverseStudio.Models.Requests;

namespace UniverseStudio.Services;

public interface IFetchService
{
	public Task<Player?> FetchSteamNamesAsync(string steamID);
}