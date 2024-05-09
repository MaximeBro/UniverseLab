using Microsoft.EntityFrameworkCore;
using SignalR.Models;

namespace SignalR.Databases;

public class SignalRDbContext : DbContext
{
	public DbSet<UserModel> Users { get; set; }
	
	public SignalRDbContext(DbContextOptions<SignalRDbContext> options) : base(options) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseSqlite("Data Source=./stored-data/SignalR.db");
	}
}