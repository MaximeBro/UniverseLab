using MudBlazor;
using uDrive.Models.Enums;

namespace uDrive.Models.Views;

public class InteractiveItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public UserFolder? Parent { get; set; }
    public ItemType Type { get; set; } = ItemType.Folder;
    public string? Color { get; set; } = "#FFF";
    public bool Hovered { get; set; }
}