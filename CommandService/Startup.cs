using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.EventProcessing;
using CommandsService.Data;
using CommandsService.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

namespace CommandService
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		private readonly IWebHostEnvironment _env;

		public Startup(IConfiguration configuration, IWebHostEnvironment env)
		{
			Configuration = configuration;
			_env = env;
		}


		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AppDbContext>(opt =>
					 opt.UseInMemoryDatabase("InMem"));

			services.AddScoped<ICommandRepo,CommandRepo>();
			services.AddSingleton<IEventProcessor, EventProcessor>();
			services.AddScoped<IPlatformDataClient, PlatformDataClient>();
			services.AddControllers();
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			services.AddHostedService<MessageBusSubscriber>();
		}


		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
			
			PrepDb.PrepPopulation(app);
		}
	}
}