using Microsoft.EntityFrameworkCore;
using uDrive.Models;

namespace uDrive.Database;

public class MainDbContext : DbContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserMainFolder> UserMainFolders { get; set; }
    public DbSet<UserFolder> UserFolders { get; set; }
    public DbSet<UserFile> UserFiles { get; set; }
    
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tables unique keys
        modelBuilder.Entity<UserModel>()
            .HasKey(e => e.Identifier);

        modelBuilder.Entity<UserMainFolder>()
            .HasKey(e => e.Id);

        modelBuilder.Entity<UserFolder>()
            .HasKey(e => e.Id);
        
        modelBuilder.Entity<UserFile>()
            .HasKey(e => e.Id);
        
        // Tables relationships & foreign keys
        // UserMainFolder x UserModel
        modelBuilder.Entity<UserMainFolder>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserIdentifier)
            .OnDelete(DeleteBehavior.Cascade);

        // UserFolder x UserModel
        modelBuilder.Entity<UserFolder>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserIdentifier)
            .OnDelete(DeleteBehavior.Cascade);

        // UserFolder x UserFolder
        modelBuilder.Entity<UserFolder>()
            .HasOne(e => e.Parent)
            .WithMany()
            .HasForeignKey(e => e.ParentId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
        
        // UserFile x UserModel
        modelBuilder.Entity<UserFile>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserIdentifier)
            .OnDelete(DeleteBehavior.Cascade);
        
        // UserFile x UserFolder
        modelBuilder.Entity<UserFile>()
            .HasOne(e => e.Parent)
            .WithMany()
            .HasForeignKey(e => e.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Tables navigation
        modelBuilder.Entity<UserMainFolder>()
            .Navigation(e => e.User)
            .AutoInclude();

        modelBuilder.Entity<UserFolder>()
            .Navigation(e => e.User)
            .AutoInclude();
        
        modelBuilder.Entity<UserFile>()
            .Navigation(e => e.User)
            .AutoInclude();
        
        modelBuilder.Entity<UserFile>()
            .Navigation(e => e.Parent)
            .AutoInclude();
    }
}