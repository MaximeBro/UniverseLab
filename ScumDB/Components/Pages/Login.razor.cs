using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using ScumDB.Extensions;
using ScumDB.Models.Enums;
using ScumDB.Services;
using IconsM = MudBlazor.Icons.Material;

namespace ScumDB.Components.Pages;

public partial class Login
{
	[Inject] public IConfiguration Configuration { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;
	[Inject] public UserTokenHandler UserTokenHandler { get; set; } = null!;
	[Inject] public NavigationManager NavManager { get; set; } = null!;
	[Inject] public NotificationService NotificationService { get; set; } = null!;
	[Inject] public PersistentData PersistentData { get; set; } = null!;
	
	[CascadingParameter] public Task<AuthenticationState> AuthenticationState { get; set; } = null!;

	private LoginModel _model = new();

	private bool _shown;
	private InputType PasswordInputType => _shown ? InputType.Text : InputType.Password;
	private string PasswordIcon => _shown ? IconsM.Filled.VisibilityOff : IconsM.Filled.Visibility;

	protected override async Task OnInitializedAsync()
	{
		var state = await AuthenticationState;
		if (state.User.Identity is { IsAuthenticated: true })
		{
			NavManager.NavigateTo("/", true);
		}
	}

	private void TryLogin()
	{
		var id = PersistentData.Accounts.Keys.FirstOrDefault(x => string.Equals(x, _model.Identifier, StringComparison.CurrentCultureIgnoreCase));
		if (!string.IsNullOrWhiteSpace(id) && PersistentData.Accounts[id] == _model.Password)
		{
			var guid = Guid.NewGuid();

			var claims = new[]
			{
				new Claim(ClaimTypes.GivenName, id),
				new Claim(ClaimTypes.Role, UserRole.Admin.ToString()),
				new Claim("token", guid.ToString()),
			};
		
			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
		
			UserTokenHandler.Tokens.Add(guid, claimsPrincipal);
			NavManager.NavigateTo($"/api/login/{guid}", true);
			NotificationService.SendAuthNotification();
			return;
		}
		else
		{
			Snackbar.Add("Mauvais couple d'identifiants", Severity.Error, Hardcoded.SnackbarOptions);
		}
	}
	
	private sealed class LoginModel
	{
		[Required(ErrorMessage = "Veuillez spécifier votre identifiant")]
		public string Identifier { get; set; } = string.Empty;
		
		[Required(ErrorMessage = "Veuillez spécifier votre mot de passe")]
		public string Password { get; set; } = string.Empty;
	}
}