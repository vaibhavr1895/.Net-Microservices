using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
	public static class PrepDb
	{
		public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
		{
			using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
			{
				SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
			}
		}

		private static void SeedData(AppDbContext context, bool isProduction)
		{
			if(isProduction)
			{
				System.Console.WriteLine("--> Attempting to apply migrations ....");
				try
				{
					context.Database.Migrate();					
				}catch(Exception ex)
				{
					System.Console.WriteLine($" could not migrate {ex.Message}");
				}
			}
			if(!context.Platforms.Any())
			{
				Console.WriteLine("Data not available");
				context.Platforms.AddRange(
					new Platform(){ Name= "Dot net", Publisher= "Microsoft", Cost="Free"},
					new Platform(){ Name= "Kubernetes", Publisher= "Google", Cost="Free"}
				);
				context.SaveChanges();
			}
			else
			{
				Console.WriteLine("Data awvailable");	
			}
		}
	}
}