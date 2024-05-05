using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ScumDB.Components.Pages.Admin;

public partial class WebPanel
{
    private List<BreadcrumbItem> _breadcrumbItems = [];

    protected override void OnInitialized()
    {
        _breadcrumbItems = new List<BreadcrumbItem>(new[]
        {
            new BreadcrumbItem("Admin", null, disabled: true),
            new BreadcrumbItem("WebPanel", "/admin/web-panel")
        });
    }
}