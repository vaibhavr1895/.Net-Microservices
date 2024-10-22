using System.ComponentModel;
using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CommandService.EventProcessing
{
	public class EventProcessor : IEventProcessor
	{
		private readonly IServiceScopeFactory _serviceScopeFactory;
		private readonly IMapper _mapper;

		public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
		{
			_serviceScopeFactory = serviceScopeFactory;
			_mapper = mapper;
		}
		
		public void ProcessEvent(string message)
		{
			var eventType = DetermineEvent(message);
			
			switch(eventType)
			{
				case EventType.PlatformPublished:
					AddPlatform(message);
					break;
				default:
					break;
			}
		}
		
		private EventType DetermineEvent(string notificationMessage)
		{
			System.Console.WriteLine("==> Determining the Event");
			
			var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
			
			switch(eventType.Event)
			{
				case "Platform_Published":
					System.Console.WriteLine("Platform published event detected");
					return EventType.PlatformPublished;
				default:
					System.Console.WriteLine("Could not determine the event type");
					return EventType.Undertermined;
				
			}
		}
		
		private void AddPlatform(string platformPublishedMessage)
		{
			using(var scope = _serviceScopeFactory.CreateScope())
			{
				var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
				
				var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublsihedDto>(platformPublishedMessage);
				
				try
				{
					var platform = _mapper.Map<Platform>(platformPublishedDto);
					if(!repo.ExternalPlatformExists(platform.ExternalID))
					{
						repo.CreatePlatform(platform);
						repo.SaveChanges();
						System.Console.WriteLine("=> Platform created");
					}
					else
					{
						System.Console.WriteLine("==> Platform already exists");
					}
				}
				catch(Exception ex)
				{
					System.Console.WriteLine($"==> Could not add platform to db {ex.Message}");
				}
			}
		}
	}
	
	enum EventType
	{
		PlatformPublished,
		Undertermined
	}
}