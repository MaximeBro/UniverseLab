using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScumDB.Databases;
using ScumDB.Models;

namespace ScumDB.Services;

public class FetchService(IHttpClientFactory clientFactory, IConfiguration configuration, IDbContextFactory<ScumDbContext> factory)
{
	public async Task FetchSteamNamesAsync(IEnumerable<string?> steamIDs)
	{
		List<KeyValuePair<string, string>> accounts = [];
		
		var apiKey = configuration.GetSection("Steam")["API.Key"] ?? string.Empty;
		var client = clientFactory.CreateClient();
		foreach (var id in steamIDs)
		{
			var response = await client.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={id}");
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var content = await response.Content.ReadAsStreamAsync();
				content.Position = 0;
				var doc = await JsonDocument.ParseAsync(content);
				accounts.Add(new (id, doc.RootElement.GetProperty("response").GetProperty("players")[0].GetProperty("personaname").GetString() ?? string.Empty));
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