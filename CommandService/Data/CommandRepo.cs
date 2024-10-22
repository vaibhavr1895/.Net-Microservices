using CommandService.Models;

namespace CommandService.Data
{
	public class CommandRepo : ICommandRepo
	{
		private readonly AppDbContext _dbContext;

		public CommandRepo(AppDbContext appDbContext)
		{
			_dbContext = appDbContext;
		}
		
		public void CreateCommand(Command command, int platformId)
		{
			if(command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}
			command.PlatformId = platformId;
			_dbContext.Commands.Add(command);
		}

		public void CreatePlatform(Platform platform)
		{
			if(platform == null)
			{
				throw new ArgumentNullException("Platform provided is null, not allowed");
			}
			
			_dbContext.Platforms.Add(platform);
		}

		public bool ExternalPlatformExists(int externalPlatformId)
		{
			return _dbContext.Platforms.Any(x => x.ExternalID == externalPlatformId);
		}

		public IEnumerable<Platform> GetAllPlatforms()
		{
			return _dbContext.Platforms;
		}

		public Command GetCommand(int platformId, int commnadId)
		{
			return _dbContext.Commands.FirstOrDefault(x => x.Id == commnadId && x.PlatformId == platformId);
		}

		public IEnumerable<Command> GetCommandsForPlatform(int platformId)
		{
			return _dbContext.Commands.Where(x => x.PlatformId == platformId);
		}

		public bool PlatformExists(int platformId)
		{
			return _dbContext.Platforms.Any(x => x.Id == platformId);
		}

		public bool SaveChanges()
		{
			return _dbContext.SaveChanges() >= 0;
		}
	}
}