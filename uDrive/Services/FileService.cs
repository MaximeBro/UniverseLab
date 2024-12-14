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

    public bool CreateFile(UserFile file)
    {
        var basePath = configuration["Files:ParentFolder"];
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            { 
                throw new InvalidOperationException("[Configuration] Files:ParentFolder parameter is null or empty.");
            }
            
            var userPath = Path.Combine(Directory.GetCurrentDirectory(), basePath, file.UserIdentifier);
            var fullPath = Path.Combine(userPath, file.GetFullPath());
            if (File.Exists(fullPath))
            {
                logger.LogWarning("User file already exists (user id: {id}). Aborting creation...", file.UserIdentifier);
                return false;
            }

            File.Create(fullPath);
            logger.LogInformation("User file at {path} successfully created for user {id}.", fullPath, file.UserIdentifier);

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while creating user file (user id: {id}).", file.UserIdentifier);
        }

        return false;
    }

    public bool FinalDeleteFile(UserFile file)
    {
        var basePath = configuration["Files:ParentFolder"];
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            { 
                throw new InvalidOperationException("[Configuration] Files:ParentFolder parameter is null or empty.");
            }
            
            var userPath = Path.Combine(Directory.GetCurrentDirectory(), basePath, file.UserIdentifier);
            var fullPath = Path.Combine(userPath, file.GetFullPath());
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                logger.LogWarning("Deleted file (id: {fileId}) for user {userId}.", file.Id, file.UserIdentifier);
                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while deleting file (id: {fileId}) for user {userId}.", file.Id, file.UserIdentifier);
        }

        return false;
    }
    
    public bool FinalDeleteDirectory(UserFolder folder)
    {
        var basePath = configuration["Files:ParentFolder"];
        try
        {
            if (string.IsNullOrWhiteSpace(basePath))
            { 
                throw new InvalidOperationException("[Configuration] Files:ParentFolder parameter is null or empty.");
            }
            
            var userPath = Path.Combine(Directory.GetCurrentDirectory(), basePath, folder.UserIdentifier);
            var fullPath = Path.Combine(userPath, folder.GetFullPath());
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                logger.LogWarning("Deleted folder (id: {folderId}) for user {userId}.", folder.Id, folder.UserIdentifier);
                return true;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while deleting folder (id: {folderId}) for user {userId}.", folder.Id, folder.UserIdentifier);
        }

        return false;
    }
}