using Microsoft.EntityFrameworkCore;
using uDrive.Database;
using uDrive.Extensions;
using uDrive.Models;

namespace uDrive.Services;

public class FileService(IDbContextFactory<MainDbContext> dbContextFactory, IConfiguration configuration, ILogger<FileService> logger)
{
    public Task<Exception?> InitUserFolderAsync(UserModel user)
    {
        try
        {
            CreateDirectory(new UserFolder
            {
                UserIdentifier = user.Identifier,
                Name = user.Identifier
            }, hasUser: false);
            return Task.FromResult<Exception?>(null);
        }
        catch (Exception e)
        {
            return Task.FromResult<Exception?>(e);
        }
    }

    public async Task<UserMainFolder?> GetUserMainFolderAsync(UserModel? user)
    {
        if (user is null) return null;
        
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return db.UserMainFolders
            .Include(x => x.User)
            .FirstOrDefault(x => x.UserIdentifier == user.Identifier);
    }
    
    public async Task<IEnumerable<UserFile>> GetUserFilesAsync(UserModel user)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return db.UserFiles
            .Include(x => x.User)
            .Include(x => x.Parent)
            .Where(x => x.UserIdentifier == user.Identifier).ToList();
    }

    public bool CreateDirectory(UserFolder folder, bool hasUser = true)
    {
        var basePath = configuration["Files:ParentFolder"];
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            { 
                throw new InvalidOperationException("[Configuration] Files:ParentFolder parameter is null or empty.");
            }
            
            var userPath = Path.Combine(Directory.GetCurrentDirectory(), basePath,
                hasUser ? folder.UserIdentifier : string.Empty);
            var fullPath = Path.Combine(userPath, folder.GetFullPath());
            if (Directory.Exists(fullPath))
            {
                logger.LogWarning("User folder already exists (user id: {id}). Removing content and creating a new one...", folder.UserIdentifier);
                Directory.Delete(basePath, true);
            }

            Directory.CreateDirectory(fullPath);
            logger.LogInformation("User folder at {path} successfully created for user {id}.", fullPath, folder.UserIdentifier);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while creating user folder (user id: {id}).", folder.UserIdentifier);
        }

        return false;
    }
}