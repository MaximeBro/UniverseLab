using MudBlazor;

namespace uDrive.Models;

public class UserFile
{
    public string UserIdentifier { get; set; } = string.Empty;
    public UserModel? User { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; } = 0L;
    public string FilePath { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public FileType FileType { get; set; }
    public string FileIcon { get; set; } = Icons.Custom.FileFormats.FileDocument;
    
    public Guid ParentId { get; set; }
    public UserFolder? Parent { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastEditedAt { get; set; } = DateTime.UtcNow;
}