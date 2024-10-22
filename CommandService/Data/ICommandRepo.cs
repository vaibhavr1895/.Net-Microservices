using CommandService.Models;

namespace CommandService.Data
{
	public interface ICommandRepo
	{
		bool SaveChanges();
		
		
		IEnumerable<Platform> GetAllPlatforms();
		void CreatePlatform(Platform platform);
		bool PlatformExists(int platformId);
		bool ExternalPlatformExists(int externalPlatformId);
		
		
		IEnumerable<Command> GetCommandsForPlatform(int platformId);
		Command GetCommand(int platformId, int commnadId);
		void CreateCommand(Command command, int platformId);
	}
}