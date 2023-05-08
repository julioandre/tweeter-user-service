using user_service.Dto;
using user_service.Models;

namespace user_service.SyncDataServices.Http;

public interface ICommandDataClient
{
    Task SendUserToFollow(string userEntity);
}