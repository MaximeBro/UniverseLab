using Microsoft.AspNetCore.Mvc;
using UniverseStudio.Models;
using UniverseStudio.Services;

namespace UniverseStudio.Controllers;

[Route("/api/v1/[controller]")]
[ServiceFilter(typeof(SteamControllerFilter))]
public class SteamController(IFetchService fetchService) : Controller
{
	
	[HttpGet("GetSteamAccount/{steamID}")]
	public async Task<IActionResult> GetSteamAccount(string steamID)
	{
		var model = await fetchService.FetchSteamNamesAsync(steamID);
		if (model is null)
		{
			return NotFound();
		}
		
		var steamAccount = new SteamAccountModel { SteamId = model.SteamId ?? string.Empty, Name = model.PersonaName ?? string.Empty };
		return Ok(steamAccount);
	}
}