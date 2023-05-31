using System.Diagnostics;
using Confluent.Kafka;
using Newtonsoft.Json;
using user_service.Services;
using NotImplementedException = System.NotImplementedException;

namespace user_service.Messaging;

public class Consumers:BackgroundService
{
    
    private IUserService _userService;
    private IProducers _producers;
    private IServiceScopeFactory _serviceProvider;
    private ITaskHandler _handler;
    
    public Consumers(IServiceScopeFactory serviceProvider)
    {
        _serviceProvider = serviceProvider;
        

    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        using var scope = _serviceProvider.CreateScope();
        // // For scoped services error
         _handler = scope.ServiceProvider.GetRequiredService<ITaskHandler>();
         await Task.Run(() => _handler.Handle(stoppingToken));
         // string topic = "followServiceTopic";
         // string groupId = "test_group";
         // var config = new ConsumerConfig {GroupId = groupId, BootstrapServers = bootstrapServers, AutoOffsetReset = AutoOffsetReset.Earliest};
         // try
         // {
         //     using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
         //     {
         //         consumerBuilder.Subscribe(topic);
         //         
         //         try
         //         {
         //             while (!stoppingToken.IsCancellationRequested)
         //             {
         // Console.WriteLine("Starting to Consume");
         // await Task.Delay(100,stoppingToken);
         // var consumer = consumerBuilder.Consume(stoppingToken);
         // var orderRequest = JsonConvert.DeserializeObject<string>(consumer.Message.Value);
         // Console.WriteLine("Consuming");
         // //var userEntity = _userService.GetUser(orderRequest);
         //                 // Console.WriteLine("Sending ID to followService" + orderRequest);
         //             }
         //             
         //         }catch (OperationCanceledException) {
         //             consumerBuilder.Close();
         //         }
         //     }
         //
         // }catch (Exception ex) {
         //     System.Diagnostics.Debug.WriteLine(ex.Message);
         // }
         //
         //





    }

  
}