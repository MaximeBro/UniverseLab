using ScumDB.Models;

namespace ScumDB.Services;

public interface IFetchService
{
    protected Task<Dictionary<string, string>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs);
    public Task AddAsync(IEnumerable<string?> steamIds);
    public Task UpdateAsync(IEnumerable<SteamAccountModel> models);
}