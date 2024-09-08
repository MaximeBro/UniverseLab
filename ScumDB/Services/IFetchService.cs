using ScumDB.Models;

namespace ScumDB.Services;

public interface IFetchService
{
    protected Task<List<SteamAccountModel>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs);
    public Task AddAccountsAsync(params string?[] steamIds);
    public Task<int> AddAccountsAsync(SteamAccountModel[] accounts);
    public Task UpdateAsync(IEnumerable<SteamAccountModel> models);
    public Task<SteamAccountModel[]> ParseAsync(string content);
}