namespace user_service.Messaging;

public interface ITaskHandler
{
    public  Task Handle( CancellationToken stoppingToken);
}