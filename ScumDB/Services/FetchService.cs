using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScumDB.Databases;
using ScumDB.Models;
using ScumDB.Models.Requests;

namespace ScumDB.Services;

public class FetchService(IHttpClientFactory clientFactory, IConfiguration configuration, IDbContextFactory<ScumDbContext> factory) : IFetchService
{
	/// <summary>
	/// Fetches all the steam profiles of the given ids using configuration's api key
	/// </summary>
	/// <param name="steamIDs">A list of <see cref="KeyValuePair{TKey,TValue}"/> as a pair of steamID and given-name</param>
	public async Task<Dictionary<string, string>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs)
	{
		Dictionary<string, string> accounts = [];

		var apiKey = configuration.GetSection("Steam")["API.Key"] ?? string.Empty;
		var client = clientFactory.CreateClient();
		foreach (var id in steamIDs)
		{
			if (id is null) continue;
			var response = await client.GetAsync($"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={id}");
			if (response.StatusCode == HttpStatusCode.OK)
			{
				var content = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<ApiResponse>(content);
				if (result is { Response: { Players: [var first, ..] } })
				{
					accounts.Add(id, result.Response.Players.First().PersonaName ?? string.Empty);
				}
			}
		}

		client.Dispose();
		return accounts;
	}
	
	public async Task AddAsync(IEnumerable<string?> steamIDs)
	{
		var accounts = await FetchSteamNamesAsync(steamIDs);
		var db = await factory.CreateDbContextAsync();
		await db.Accounts.AddRangeAsync(accounts.Select(x => new SteamAccountModel
		{
			SteamId = x.Key,
			Name = x.Value
		}));

		await db.SaveChangesAsync();
		await db.DisposeAsync();
	}

	public async Task UpdateAsync(IEnumerable<SteamAccountModel> models)
	{
		var accounts = await FetchSteamNamesAsync(models.Select(x => x.SteamId).ToList());
		var db = await factory.CreateDbContextAsync();
		var toUpdate = await db.Accounts.AsNoTracking().Where(x => accounts.Select(y => y.Key).Contains(x.SteamId)).ToListAsync();
		foreach (var account in toUpdate)
		{
			if (accounts.TryGetValue(account.SteamId, out var name))
			{
				account.SavedAt = DateTime.Now;
				account.Name = name;
			}
		}

		db.UpdateRange(toUpdate);
		await db.SaveChangesAsync();
		await db.DisposeAsync();
	}
}