using Microsoft.EntityFrameworkCore;
using ScumDB.Models;

namespace ScumDB.Databases;

public class ScumDbContext : DbContext
{
	public DbSet<SteamAccountModel> Accounts { get; set; }
	public DbSet<VehicleModel> Vehicles { get; set; }
	
	public ScumDbContext(DbContextOptions<ScumDbContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		
	}
}