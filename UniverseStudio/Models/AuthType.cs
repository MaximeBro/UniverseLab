using System.ComponentModel;

namespace UniverseStudio.Models;

public enum AuthType
{
    [Description("internal")]
    Internal,
    
    [Description("custom")]
    Custom,
    
    [Description("google")]
    Google,
    
    [Description("steam")]
    Steam
}