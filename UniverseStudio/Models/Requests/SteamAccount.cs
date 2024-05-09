﻿using System.Text.Json.Serialization;

namespace UniverseStudio.Models.Requests;

public class Player
{
	[JsonPropertyName("steamid")]
	public string? SteamId { get; set; }

	[JsonPropertyName("communityvisibilitystate")]
	public int CommunityVisibilityState { get; set; }

	[JsonPropertyName("profilestate")]
	public int ProfileState { get; set; }

	[JsonPropertyName("personaname")]
	public string? PersonaName { get; set; }

	[JsonPropertyName("profileurl")]
	public string? ProfileUrl { get; set; }

	[JsonPropertyName("avatar")]
	public string? Avatar { get; set; }

	[JsonPropertyName("avatarmedium")]
	public string? AvatarMedium { get; set; }

	[JsonPropertyName("avatarfull")]
	public string? AvatarFull { get; set; }

	[JsonPropertyName("avatarhash")]
	public string? AvatarHash { get; set; }

	[JsonPropertyName("personastate")]
	public int PersonaState { get; set; }

	[JsonPropertyName("realname")]
	public string? RealName { get; set; }

	[JsonPropertyName("primaryclanid")]
	public string? PrimaryClanId { get; set; }

	[JsonPropertyName("timecreated")]
	public long TimeCreated { get; set; }

	[JsonPropertyName("personastateflags")]
	public int PersonaStateFlags { get; set; }

	[JsonPropertyName("loccountrycode")]
	public string? LocCountryCode { get; set; }
}

public class Response
{
	[JsonPropertyName("players")]
	public List<Player> Players { get; set; } = [];
}

public class ApiResponse
{
	[JsonPropertyName("response")]
	public Response? Response { get; set; }
}
