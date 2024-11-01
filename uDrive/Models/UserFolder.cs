namespace uDrive.Models;

public class UserFolder
{
    public string UserIdentifier { get; set; } = string.Empty;
    public UserModel? User { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public UserFolder? Parent { get; set; }
}