using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ScumDB.Components.Components;

public partial class DiscordWidget
{
    [Parameter] public string ServerId { get; set; } = null!;
    [Parameter] public string InviteId { get; set; } = null!;
    [Parameter] public string? FooterText { get; set; }
    [Parameter] public string JoinButtonText { get; set; } = "Join us";
    
    private readonly HttpClient _client = new();
    private WidgetStatus? _widgetStatus;
    private bool _failed;

    protected override void OnParametersSet()
    {
        if (string.IsNullOrWhiteSpace(ServerId))
        {
            throw new ArgumentException("ServerId parameter cannot be null or empty !");
        }

        if (string.IsNullOrWhiteSpace(InviteId))
        {
            throw new ArgumentException("InviteId parameter cannot be null or empty !");
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetServerStatusAsync();
        }
    }

    private async Task GetServerStatusAsync()
    {
        await Task.Delay(250);
        var response = await _client.GetAsync($"https://discord.com/api/guilds/{ServerId}/widget.json");
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            _widgetStatus = JsonSerializer.Deserialize<WidgetStatus>(json);
        }
        else
        {
            _failed = true;
        }
        StateHasChanged();
    }

    private Color GetStatusColor(string status)
    {
        return status switch
        {
            "online" => Color.Success,
            "idle" => Color.Warning,
            "dnd" => Color.Error,
            _ => Color.Default
        };
    }
    
    private sealed class WidgetStatus
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

        [JsonPropertyName("channels")] public List<Channel> Channels { get; set; } = [];

        [JsonPropertyName("members")] public List<Member> Members { get; set; } = [];

        [JsonPropertyName("presence_count")] public int PresenceCount { get; set; }
    }

    private sealed class Channel
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

        [JsonPropertyName("position")] public int Position { get; set; }
    }

    private sealed class Member
    {
        [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;

        [JsonPropertyName("username")] public string Username { get; set; } = string.Empty;

        [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

        [JsonPropertyName("avatar_url")] public string AvatarUrl { get; set; } = string.Empty;

        [JsonPropertyName("channel_id")] public string ChannelId { get; set; } = string.Empty;
        
        [JsonPropertyName("game")] public Activity? Game { get; set; }
    }

    private sealed class Activity
    {
        [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    }
}