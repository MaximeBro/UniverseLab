using ScumDB.Models;

namespace ScumDB.Services;

public interface IFetchService
{
    protected Task<List<SteamAccountModel>> FetchSteamNamesAsync(IEnumerable<string?> steamIDs);
    public Task AddAsync(IEnumerable<string?> steamIds);
    public Task UpdateAsync(IEnumerable<SteamAccountModel> models);
}