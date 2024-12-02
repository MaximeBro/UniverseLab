using Microsoft.EntityFrameworkCore;
using uDrive.Database;
using uDrive.Models.Enums;

namespace uDrive.Services;

public class FileService(IDbContextFactory<MainDbContext> dbContextFactory, IConfiguration configuration, ILogger<FileService> logger)
{
    public Task<InvalidOperationException?> InitUserFolderAsync(UserModel user)
    {
        var basePath = configuration["Files:ParentFolder"];
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            {
                return Task.FromException<InvalidOperationException>(new InvalidOperationException("[Configuration] Files:ParentFolder parameter is null or empty."))!;
            }

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), basePath, user.Identifier);
            if (Directory.Exists(folderPath))
            {
                logger.LogWarning("User folder already exists (user id: {id}). Removing content and creating a new one...", user.Identifier);
                Directory.Delete(basePath, true);
            }

            Directory.CreateDirectory(folderPath);
            logger.LogInformation("User folder at {path} successfully created for user {id}.", folderPath, user.Identifier);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while initializing user folder (user id: {id}).", user.Identifier);
        }

        return Task.FromResult<InvalidOperationException?>(null);
    }

    public async Task<UserMainFolder?> GetUserMainFolderAsync(UserModel? user)
    {
        if (user is null) return null;
        
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return db.UserMainFolders.FirstOrDefault(x => x.UserIdentifier == user.Identifier);
    }
    
    public async Task<IEnumerable<UserFile>> GetUserFilesAsync(UserModel user)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return db.Files.Where(x => x.UserIdentifier == user.Identifier).ToList();
    }
}