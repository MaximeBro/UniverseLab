using System.Net;
using System.Text.Json;
using UniverseStudio.Models.Requests;

namespace UniverseStudio.Services;

public class FetchService(IConfiguration configuration, IHttpClientFactory clientFactory) : IFetchService
{
	/// <summary>
	/// Fetches all the steam profiles of the given ids using configuration's api key
	/// </summary>
	/// <param name="steamID">A list of <see cref="KeyValuePair{TKey,TValue}"/> as a pair of steamID and given-name</param>
	public async Task<Player?> FetchSteamNamesAsync(string steamID)
	{
		var apiKey = configuration.GetSection("Steam")["API.Key"] ?? string.Empty;
		var client = clientFactory.CreateClient();

		var response = await client.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamID}");
		if (response.StatusCode == HttpStatusCode.OK)
		{
			var content = await response.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<ApiResponse>(content);
			if (result is { Response.Players: [_, ..] })
			{
				return result.Response.Players.FirstOrDefault();
			}
		}

		return null;
	}
}