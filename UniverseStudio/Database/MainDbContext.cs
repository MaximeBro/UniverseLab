using Microsoft.EntityFrameworkCore;
using UniverseStudio.Models;

namespace UniverseStudio.Database;

public class MainDbContext : DbContext
{
    public DbSet<UserModel> Users { get; init; }
    
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }
}