using MudBlazor;

namespace uDrive.Models;

public class UserFile
{
    public string UserIdentifier { get; set; } = string.Empty;
    public UserModel? User { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0L;
    public string Extension { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public string Color { get; set; } = "#c5c7cd";

    public bool DeletionAsked { get; set; } = false;
    public DateTime? DeletionAskedAt { get; set; }
    
    public Guid? ParentId { get; set; }
    public UserFolder? Parent { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastEditedAt { get; set; } = DateTime.UtcNow;
}