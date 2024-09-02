using System.Text.Json;

namespace ScumDB.Services;

public class PersistentData(ILogger<PersistentData> logger)
{
    public IReadOnlyDictionary<string, string> Blueprints { get; private set; } = null!;
    public IReadOnlyDictionary<string, string> Accounts { get; private set; } = null!;
    
    public async Task InitAsync(WebApplication app)
    {
        try
        {
            var vehiclesPath = new DirectoryInfo(Path.Combine(app.Environment.ContentRootPath, "../config/vehicles.json")).FullName;
            var accountsPath = new DirectoryInfo(Path.Combine(app.Environment.ContentRootPath, "../config/scum-admin-accounts.json")).FullName;
            
            var blueprints = await File.ReadAllTextAsync(vehiclesPath);
            var accounts = await File.ReadAllTextAsync(accountsPath);
            
            var blueprintsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(blueprints);
            var accountsDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(accounts);
            
            Blueprints = blueprintsDictionary?.AsReadOnly() ?? new Dictionary<string, string>().AsReadOnly();
            Accounts = accountsDictionary?.AsReadOnly() ?? new Dictionary<string, string>().AsReadOnly();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }
}