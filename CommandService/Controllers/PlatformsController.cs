using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers
{
	[Route("api/c/[controller]")]
	[ApiController]
	public class PlatformsController: ControllerBase
	{
		private readonly ICommandRepo _commandRepo;
		private readonly IMapper _mapper;

		public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
		{
			_commandRepo = commandRepo;
			_mapper = mapper;
		}
		
		[HttpPost]
		public ActionResult TestInboundConnection()
		{
			Console.WriteLine("--> Inbound POST  # Command Service");
			return Ok("Inbound test Platforms controller");
		}
		
		[HttpGet]
		public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
		{
			System.Console.WriteLine("Getting platforms from Command Service.");
			var platformsItems = _commandRepo.GetAllPlatforms();
			
			return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformsItems));
		}
	}
}