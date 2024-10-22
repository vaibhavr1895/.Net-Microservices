using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
	public class MessageBusClient : IMessageBusClient
	{
		private readonly IConfiguration _configuration;
		private readonly IConnection _connection;
		private readonly IModel _channel;

		public MessageBusClient(IConfiguration configuration)
		{
			_configuration = configuration;
			var factory = new ConnectionFactory()
			{
				HostName = _configuration["RabbitMQHost"],
				Port = int.Parse(_configuration["RabbitMQPort"])
			};
			
			try
			{
				_connection = factory.CreateConnection();
				_channel = _connection.CreateModel();
				
				_channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
				
				_connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
				
				System.Console.WriteLine("==> Connected to Rabbit MQ message bus");
			}
			catch(Exception ex)
			{
				System.Console.WriteLine($"==> Could not connect to rabbit mq message bus : {ex.Message}");
			}
		}

		public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
		{
			var message = JsonSerializer.Serialize(platformPublishedDto);
			if(_connection.IsOpen)
			{
				System.Console.WriteLine("==> RabbitMQ connection open, sending message.");
				SendMessage(message);
			}
			else
			{
				System.Console.WriteLine("==> RabbitMQ connection not open, not able to send message.");
			}
		}
		
		private void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs e)
		{
			System.Console.WriteLine("==> Rabbit MQ Connection Shutdown");
		}

		private void SendMessage(string message)
		{
			var body = Encoding.UTF8.GetBytes(message);
			
			_channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body);
			
			System.Console.WriteLine($"==> We have sent {message}");
		}
		
		public void Dispose()
		{
			System.Console.WriteLine("==> Message bus disposed.");
			if(_channel.IsOpen)
			{
				_channel.Close();
				_connection.Close();
			}
		}
	}
}