
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Comsumer_Application
{
    public class ConsumerService : BackgroundService
    {

        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<ConsumerService> _logger;

        public ConsumerService(IConfiguration configuration, ILogger<ConsumerService> logger)
        {
            _logger = logger;
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "ConsumerGroup",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("MessageQueue");

            while (!stoppingToken.IsCancellationRequested)
            {
                ProcessMessage(stoppingToken);
                Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }

            _consumer.Close();
        }

        public void ProcessMessage(CancellationToken stoppingToken)
        {
            try
            {
                var consumeResult = _consumer.Consume(stoppingToken);
                var message = consumeResult.Message.Value;

                _logger.LogInformation($"Message Received : {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing message: {ex.Message}");
            }
        }
    }
}
