namespace user_service.Messaging;

public interface IConsumers:IHostedService
{
    Task GetIdAsync(string topic, CancellationToken cancellation);
}