using user_service.Models;

namespace user_service.Services;

public interface IUserService
{
    UserEntity GetUser(string username, bool includeCache = false);
}