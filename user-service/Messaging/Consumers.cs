using System.Diagnostics;
using Confluent.Kafka;
using Newtonsoft.Json;
using user_service.Services;
using NotImplementedException = System.NotImplementedException;

namespace user_service.Messaging;

public class Consumers:IHostedService
{
    private readonly string bootstrapServers = "127.0.0.1:9092";
    private IUserService _userService;
    private IProducers _producers;
    private IServiceScopeFactory _serviceProvider;
    public Consumers(IServiceScopeFactory serviceProvider)
    {
        _serviceProvider = serviceProvider;

    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        // For scoped services error
        _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        string topic = "followServiceTopic2";
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
                    while (!cancelToken.IsCancellationRequested)
                    {
                        Console.WriteLine("Starting to Consume");
                        var consumer = consumerBuilder.Consume(cancelToken.Token);
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
        return Task.CompletedTask;
        
    }
    
    public Task StopAsync(CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }
}