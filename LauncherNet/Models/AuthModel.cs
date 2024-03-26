using System.Text.Json.Serialization;

namespace LauncherNet.Models;

public class AuthModel
{
    [JsonInclude]
    [JsonPropertyName("agent")]
    public AgentModel Agent = new AgentModel();
    
    [JsonInclude]
    [JsonPropertyName("username")] public string Username { get; set; } = "maimemail.brochard.com@gmail.com";
    
    [JsonInclude]
    [JsonPropertyName("password")] public string Password { get; set; } = string.Empty;
}