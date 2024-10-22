
using System.Text;
using CommandService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandService.AsyncDataServices
{
	public class MessageBusSubscriber : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly IEventProcessor _eventProcessor;
		private IConnection _connection;
		private IModel _channel;
		private string _queueName;

		public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
		{
			_configuration= configuration;
			_eventProcessor = eventProcessor;
			InitializeRabbitMQ();
		}

		private void InitializeRabbitMQ()
		{
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
				_queueName = _channel.QueueDeclare().QueueName;
				_channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
				
				_connection.ConnectionShutdown += RabbitMQ_ConnectionShutDown;
				
				System.Console.WriteLine("==> Listerning to Message Bus");
			}
			catch(Exception ex)
			{
				System.Console.WriteLine($"==> Could not connect to Listen from rabbit mq message bus : {ex.Message}");
			}
		}

		private void RabbitMQ_ConnectionShutDown(object? sender, ShutdownEventArgs e)
		{
			System.Console.WriteLine("==> Rabbit MQ Connection Shutdown");
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();
			
			var consumer = new EventingBasicConsumer(_channel);
			
			consumer.Received += (ModuleHandle, ea) =>
			{
				System.Console.WriteLine("==> Event Recieved!");
				var body = ea.Body;
				var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
				
				_eventProcessor.ProcessEvent(notificationMessage);
			};
			
			_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
			
			return Task.CompletedTask;
		}

		public override void Dispose()
		{
			if(_channel.IsOpen)
			{
				_channel.Close();
				_connection.Close();
			}
			base.Dispose();
		}
		
	}
}