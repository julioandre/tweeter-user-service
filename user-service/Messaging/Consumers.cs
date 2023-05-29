using System.Diagnostics;
using Confluent.Kafka;
using Newtonsoft.Json;
using user_service.Services;
using NotImplementedException = System.NotImplementedException;

namespace user_service.Messaging;

public class Consumers:IHostedService
{
    private readonly string bootstrapServers = "localhost:9092";
    private IUserService _userService;
    private IProducers _producers;
    private IServiceProvider _serviceProvider;
    public Consumers(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        // For scoped services error
        _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        string topic = "followTopic";
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
                        Debug.WriteLine("Sending ID to followService" + userEntity.Id);
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