using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScumDB.Databases;
using ScumDB.Models;
using ScumDB.Models.Requests;

namespace ScumDB.Services;

public class FetchService(IHttpClientFactory clientFactory, IConfiguration configuration, IDbContextFactory<ScumDbContext> factory)
{
	
	/// <summary>
	/// Fetches all the steam profiles of the given ids using configuration's api key
	/// </summary>
	/// <param name="steamIDs">A list of <see cref="KeyValuePair{TKey,TValue}"/> as a pair of steamID and given-name</param>
	public async Task FetchSteamNamesAsync(IEnumerable<string?> steamIDs)
	{
		List<KeyValuePair<string, string>> accounts = [];

		var apiKey = configuration.GetSection("Steam")["API.Key"] ?? string.Empty;
		var client = clientFactory.CreateClient();
		foreach (var id in steamIDs)
		{
			if (id is null) continue;
			var response =
				await client.GetAsync(
					$"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={id}");
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var content = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<ApiResponse>(content);
				if (result is { Response: { Players: [var first, ..] } })
				{
					accounts.Add(new(id, result.Response.Players.First().PersonaName ?? string.Empty));
				}
			}
		}

		var db = await factory.CreateDbContextAsync();
		foreach (var account in accounts.DistinctBy(x => x.Key))
		{
			if (db.Accounts.All(x => x.SteamId != account.Key))
			{
				db.Accounts.Add(new SteamAccountModel { SteamId = account.Key, Name = account.Value });
			}
		}

		await db.SaveChangesAsync();
		await db.DisposeAsync();
	}
}