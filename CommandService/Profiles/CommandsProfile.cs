using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
using PlatformService;

namespace CommandService.Profiles
{
	public class CommandsProfile: Profile
	{
		public CommandsProfile()
		{
			// Source -> Target
			CreateMap<Platform, PlatformReadDto>();
			CreateMap<Command, CommandReadDto>();
			CreateMap<CommandCreateDto, Command>();
			CreateMap<PlatformPublsihedDto, Platform>().ForMember(destination => destination.ExternalID, opt => opt.MapFrom(source => source.Id));
			CreateMap<GrpcPlatformModel, Platform>()
				.ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Commands, opt => opt.Ignore());
		}
	}
}