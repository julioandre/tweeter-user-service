using KafkaFlow;
using KafkaFlow.TypedHandler;

namespace user_service.Messaging;

public class TaskHandler1:IMessageHandler<string>
{
    private readonly ILogger<string> _logger;

    public TaskHandler1(ILogger<string> logger)
    {
        _logger = logger;
    }
    public Task Handle(IMessageContext context, string message)
    {
        _logger.LogInformation(message);
        return Task.CompletedTask;
    }
}