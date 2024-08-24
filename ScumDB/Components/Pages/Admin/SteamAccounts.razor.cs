using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using ScumDB.Components.Dialogs;
using ScumDB.Components.Dialogs.SteamAccounts;
using ScumDB.Databases;
using ScumDB.Extensions;
using ScumDB.Models;
using ScumDB.Services;

namespace ScumDB.Components.Pages.Admin;

public partial class SteamAccounts
{
    [Inject] public IDbContextFactory<ScumDbContext> Factory { get; set; } = null!;
    [Inject] public IFetchService FetchService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;

    private List<SteamAccountModel> _models = [];
    private string _search = string.Empty;

    private bool _loading;
    
    private List<BreadcrumbItem> _breadcrumbItems = [];

    private Func<SteamAccountModel, bool> QuickFilter => x =>
    {
        if (x.SteamId.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        if (x.Name.Contains(_search, StringComparison.OrdinalIgnoreCase)) return true;
        
        return false;
    };

    private AggregateDefinition<SteamAccountModel> _aggregation = Hardcoded.GetAggregateCountOf<SteamAccountModel>();
    
    protected override async Task OnInitializedAsync()
    {
        _breadcrumbItems = new List<BreadcrumbItem>(new[]
        {
            new BreadcrumbItem("Admin", null, disabled: true),
            new BreadcrumbItem("WebPanel", "/admin/web-panel/"),
            new BreadcrumbItem("Comptes steam", null, disabled: true)
        });
        await RefreshDataAsync();
    }

    private async Task AddAccountAsync()
    {
        var instance = await DialogService.ShowAsync<SteamAccountDialog>(string.Empty, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SteamAccountModel model })
        {
            await using var db = await Factory.CreateDbContextAsync();
            db.Accounts.Add(model);
            await db.SaveChangesAsync();
            await RefreshDataAsync();
            StateHasChanged();
        }
    }

    private async Task EditAccountAsync(SteamAccountModel model)
    {
        var parameters = new DialogParameters<SteamAccountDialog> { { x => x.Model, model } };
        var instance = await DialogService.ShowAsync<SteamAccountDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: SteamAccountModel newModel })
        {
            await using var db = await Factory.CreateDbContextAsync();
            newModel.Id = model.Id;
            db.Accounts.Update(newModel);
            await db.SaveChangesAsync();
            await RefreshDataAsync();
            StateHasChanged();
        }
    }

    private async Task DeleteAccountAsync(SteamAccountModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment supprimer ce compte steam ?" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: true })
        {
            await using var db = await Factory.CreateDbContextAsync();
            db.Accounts.Remove(model);
            await RefreshDataAsync();
            StateHasChanged();
        }
    }
    
    private async Task ForceUpdateAsync()
    {
        _loading = true;
        await FetchService.UpdateAsync(_models);
        await RefreshDataAsync();
        StateHasChanged();
        
        _loading = false;
        Snackbar.Add("Données mises à jour !", Severity.Success, Hardcoded.SnackbarOptions);
    }

    private async Task PurgeAsync()
    {
        _loading = true;
        var db = await Factory.CreateDbContextAsync();
        var distinct = (await db.Accounts.AsNoTracking().ToListAsync()).DistinctBy(x => x.SteamId);
        db.Accounts.RemoveRange(db.Accounts);
        db.Accounts.AddRange(distinct);
        await db.SaveChangesAsync();
        await db.DisposeAsync();
        await RefreshDataAsync();
        StateHasChanged();
        _loading = false;
        Snackbar.Add("Données mises à jour !", Severity.Success, Hardcoded.SnackbarOptions);
    }

    private async Task RemoveAccountAsync(SteamAccountModel model)
    {
        var parameters = new DialogParameters<ConfirmDialog> { { x => x.Text, "Voulez-vous vraiment supprimer ce compte steam ?" } };
        var instance = await DialogService.ShowAsync<ConfirmDialog>(string.Empty, parameters, Hardcoded.DialogOptions);
        var result = await instance.Result;
        if (result is { Data: true })
        {
            var db = await Factory.CreateDbContextAsync();
            db.Accounts.Remove(model);
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            
            await RefreshDataAsync();
            StateHasChanged();
        }
    }

    private async Task RefreshDataAsync()
    {
        var db = await Factory.CreateDbContextAsync();
        _models = await db.Accounts.AsNoTracking().ToListAsync();
        await db.DisposeAsync();
    }
}