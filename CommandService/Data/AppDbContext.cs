using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
	public class AppDbContext: DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions): base(dbContextOptions)
		{
			
		}
		
		public DbSet<Platform> Platforms { get; set; }
		public DbSet<Command> Commands { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Platform>()
						.HasMany(p => p.Commands)
						.WithOne(p => p.Platform!)
						.HasForeignKey(x => x.PlatformId);
						
			modelBuilder.Entity<Command>()
						.HasOne(c => c.Platform)
						.WithMany(c => c.Commands)
						.HasForeignKey(c => c.PlatformId);
		}
		
	}
}