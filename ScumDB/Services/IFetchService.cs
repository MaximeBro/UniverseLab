using ScumDB.Models;

namespace ScumDB.Services;

public interface IFetchService
{
    protected Task<List<SteamAccountModel>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs);
    public Task AddAccountsAsync(params string?[] steamIds);
    public Task UpdateAsync(IEnumerable<SteamAccountModel> models);
}