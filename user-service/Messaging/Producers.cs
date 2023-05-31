using System.Diagnostics;
using System.Net;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace user_service.Messaging;

public class Producers:IProducers
{
    private readonly string bootstrapServers = "localhost:9092";
    public async Task<bool> SendId(string topic, string Id)
    {
        ProducerConfig config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers, ClientId = Dns.GetHostName()
        };
        try
        {
            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = Id });
                Debug.WriteLine($"Delivery Timestamp:{result.Timestamp.UtcDateTime}");
                return await Task.FromResult(true);

            }
        }catch (Exception ex) {
            Console.WriteLine($"Error occured: {ex.Message}");
        }
        return await Task.FromResult(false);
    }
}