using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScumDB.Databases;
using ScumDB.Models;

namespace ScumDB.Services;

public class FetchService(IHttpClientFactory clientFactory, IDbContextFactory<ScumDbContext> factory) : IFetchService
{
	/// <summary>
	/// Fetches all the steam profiles of the given ids using configuration's api key
	/// </summary>
	/// <param name="steamIDs">A list of <see cref="KeyValuePair{TKey,TValue}"/> as a pair of steamID and given-name</param>
	public async Task<List<SteamAccountModel>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs)
	{
		List<SteamAccountModel> accounts = [];
		
		using var client = clientFactory.CreateClient();
		foreach (var id in steamIDs)
		{
			if (id is null) continue;
			var response = await client.GetAsync($"https://www.universestudio.net/api/v1/Steam/GetSteamAccount/{id}");
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var result = JsonSerializer.Deserialize<SteamAccountModel>(content);
				if(result != null) accounts.Add(result);
				response.Dispose();
			}
		}
		
		return accounts;
	}
	
	public async Task AddAccountsAsync(params string?[] steamIDs)
	{
		var accounts = await FetchSteamNamesAsync(steamIDs);
		await using var db = await factory.CreateDbContextAsync();
		var newAccounts = accounts.Where(x => !db.Accounts.Select(y => y.SteamId).Contains(x.SteamId)).ToList();
		await db.Accounts.AddRangeAsync(newAccounts);
		await db.SaveChangesAsync();
	}

	public async Task<int> AddAccountsAsync(SteamAccountModel[] accounts)
	{
		await using var db = await factory.CreateDbContextAsync();
		var newAccounts = accounts.Where(x => !db.Accounts.Select(y => y.SteamId).Contains(x.SteamId)).ToList();
		await db.Accounts.AddRangeAsync(newAccounts);
		await db.SaveChangesAsync();
		
		return newAccounts.Count;
	}
	
	public async Task UpdateAsync(IEnumerable<SteamAccountModel> models)
	{
		var accounts = await FetchSteamNamesAsync(models.Select(x => x.SteamId).ToList());
		await using var db = await factory.CreateDbContextAsync();
		var toUpdate = await db.Accounts.AsNoTracking().Where(x => accounts.Select(y => y.SteamId).Contains(x.SteamId)).ToListAsync();
		foreach (var account in toUpdate)
		{
			var old = accounts.FirstOrDefault(x => x.SteamId == account.SteamId);
			if (old != null)
			{
				account.SavedAt = DateTime.Now;
				account.Name = old.Name;
			}
		}

		db.UpdateRange(toUpdate);
		await db.SaveChangesAsync();
	}

	public Task<SteamAccountModel[]> ParseAsync(string content)
	{
		List<SteamAccountModel> accounts = [];

		var lines = content.Split("\n").Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		foreach (var accountLines in lines.Chunk(6))
		{
			var fullName = accountLines[1].Replace("Steam:", string.Empty).Trim();
			accounts.Add(new SteamAccountModel
			{
				Name = accountLines[0].Substring(4),
				SteamName = fullName.Substring(0, fullName.IndexOf('(')).Trim(),
				SteamId = accountLines[1].Substring(accountLines[1].IndexOf('7')).Replace(")", string.Empty),
			});
		}

		return Task.FromResult(accounts.ToArray());
	}
}