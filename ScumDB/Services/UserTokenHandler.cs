using System.Security.Claims;

namespace ScumDB.Services;

public class UserTokenHandler
{
	public Dictionary<Guid, ClaimsPrincipal> Tokens { get; } = new();
}