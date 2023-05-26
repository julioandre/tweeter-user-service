using System.Diagnostics;
using Confluent.Kafka;
using Newtonsoft.Json;
using user_service.Services;

namespace user_service.Messaging;

public class Consumers:IConsumers
{
    private readonly string bootstrapServers = "localhost:9092";
    private IUserService _userService;
    private IProducers _producers;

    public Consumers(IUserService userService, IProducers producers)
    {
        _userService = userService;
        _producers = producers;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

   

    public Task GetIdAsync(string topic, CancellationToken cancellationToken)
    { 
        string groupId = "test_group";
        var config = new ConsumerConfig {GroupId = groupId, BootstrapServers = bootstrapServers, AutoOffsetReset = AutoOffsetReset.Earliest};
        try
        {
            using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumerBuilder.Subscribe(topic);
                var cancelToken = new CancellationTokenSource();
                try
                {
                    while (true)
                    {
                        var consumer = consumerBuilder.Consume(cancelToken.Token);
                        var orderRequest = JsonConvert.DeserializeObject<string>(consumer.Message.Value);
                        var userEntity = _userService.GetUser(orderRequest);
                        _producers.SendId("userTopic", orderRequest);
                        Debug.WriteLine("Sending ID to followService");
                    }
                }catch (OperationCanceledException) {
                    consumerBuilder.Close();
            }
        }
        
    }catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}