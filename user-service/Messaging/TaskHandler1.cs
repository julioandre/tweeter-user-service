using Confluent.Kafka;
using KafkaFlow.TypedHandler;
using Newtonsoft.Json;
using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

namespace user_service.Messaging;

public class TaskHandler1:ITaskHandler

{
    private readonly ILogger<string> _logger;
    private readonly string bootstrapServers = "127.0.0.1:9092";

    public TaskHandler1(ILogger<string> logger)
    {
        _logger = logger;
    }
    public async Task Handle(CancellationToken stoppingToken)
    {
        string topic = "followServiceTopic";
        string groupId = "test_group";
        var config = new ConsumerConfig {GroupId = groupId, BootstrapServers = bootstrapServers, AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest};
        try
        {
            using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                try
                {
                    consumerBuilder.Subscribe(topic);
                    Console.WriteLine("Subscribed to Topic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Couldnt Subscribe"+ ex);
                }
               
                
                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Starting to Consume");
                        var consumer = consumerBuilder.Consume(stoppingToken);
                        var orderRequest = JsonConvert.DeserializeObject<string>(consumer.Message.Value);
                        Console.WriteLine("Consuming");
                        //var userEntity = _userService.GetUser(orderRequest);
                        Console.WriteLine("Sending ID to followService" + orderRequest);
                    }
                    
                }catch (OperationCanceledException) {
                    consumerBuilder.Close();
                }
            }
        
        }catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }
}