namespace uDrive.Models.Enums;

public class UserMainFolder
{
    public string UserIdentifier { get; set; } = string.Empty;
    public UserModel? User { get; set; }
    public Guid Id { get; set; } = Guid.NewGuid();
    public long MaxStorage { get; set; }
    public int MaxSubFolders { get; set; }
}