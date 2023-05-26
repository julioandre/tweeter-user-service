namespace user_service.Messaging;

public interface IProducers
{
    Task<bool> SendId(string topic, string Id);
}