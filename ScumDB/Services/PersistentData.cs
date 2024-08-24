using System.Text.Json;

namespace ScumDB.Services;

public class PersistentData(ILogger<PersistentData> logger)
{
    public IReadOnlyDictionary<string, string> Blueprints { get; private set; } = null!;
    
    public async Task InitAsync(WebApplication app)
    {
        try
        {
            var vehiclesPath = new DirectoryInfo(Path.Combine(app.Environment.ContentRootPath, "../config/vehicles.json")).FullName;
            var blueprints = await File.ReadAllTextAsync(vehiclesPath);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(blueprints);
            Blueprints = dictionary?.AsReadOnly() ?? new Dictionary<string, string>().AsReadOnly();
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }
}