using System.Text.Json.Serialization;

namespace LauncherNet.Models;

public class AgentModel
{
    [JsonInclude]
    [JsonPropertyName("name")] public string GameName = "Minecraft";
    
    [JsonInclude]
    [JsonPropertyName("version")] public int Version = 1;
}