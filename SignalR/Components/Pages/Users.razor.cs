using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using SignalR.Components.Dialogs;
using SignalR.Databases;
using SignalR.Hubs;
using SignalR.Models;

namespace SignalR.Components.Pages;

public partial class Users
{
	[Inject] public IDbContextFactory<SignalRDbContext> Factory { get; set; } = null!;
	[Inject] public IDialogService DialogService { get; set; } = null!;
	[Inject] public ISnackbar Snackbar { get; set; } = null!;
	[Inject] public NavigationManager NavManager { get; set; } = null!;

	private List<UserModel> users = [];
	
	private HubConnection? hubConnection;
	private Guid circuitId = Guid.NewGuid();
	
	protected override async Task OnInitializedAsync()
	{
		await RefreshDataAsync();
		hubConnection = new HubConnectionBuilder().WithUrl(NavManager.ToAbsoluteUri(UsersHub.HubUrl)).Build();

		hubConnection.On<string, Guid>(UsersHub.HubMethod, (message, circuit) =>
		{
			if (circuit == circuitId) return;
			
			if (message == nameof(Users))
			{
				InvokeAsync(async () =>
				{
					Snackbar.Add("Notification reçue !", Severity.Info);
					await RefreshDataAsync();
					StateHasChanged();
				});
			}
		});
		
		await hubConnection.StartAsync();
	}

	private async Task SendUpdateAsync() => await hubConnection!.InvokeAsync(UsersHub.HubMethod, nameof(Users), circuitId);

	private async Task AddUserAsync()
	{
		var instance = await DialogService.ShowAsync<UserDialog>(string.Empty);
		var result = await instance.Result;
		if (result is { Data: UserModel model })
		{
			var db = await Factory.CreateDbContextAsync();
			db.Users.Add(model);
			await db.SaveChangesAsync();
			await db.DisposeAsync();
			await RefreshDataAsync();
			StateHasChanged();

			await SendUpdateAsync();
		}
	}
	
	private async Task RemoveUserAsync(UserModel model)
	{
		var db = await Factory.CreateDbContextAsync();
		db.Users.Remove(model);
		await db.SaveChangesAsync();
		await db.DisposeAsync();
		await RefreshDataAsync();
		StateHasChanged();

		await SendUpdateAsync();
	}

	private async Task RefreshDataAsync()
	{
		var db = await Factory.CreateDbContextAsync();
		users = await db.Users.AsNoTracking().ToListAsync();
		await db.DisposeAsync();
	}
}