using uDrive.Models.Enums;

namespace uDrive.Models.Views;

public class InteractiveItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserIdentifier { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public UserFolder? Parent { get; set; }
    public ItemType Type { get; set; } = ItemType.Folder;
    public string Color { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastEditedAt { get; set; }
    
    /// <summary>
    /// Property is only used for styling
    /// </summary>
    public bool Hovered { get; set; }
    
    /// <summary>
    /// Property is only used for styling
    /// </summary>
    public bool Selected { get; set; }
}