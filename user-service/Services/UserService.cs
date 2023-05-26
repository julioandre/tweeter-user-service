using user_service.Cache;
using user_service.Data;
using user_service.Models;

namespace user_service.Services;

public class UserService:IUserService
{
    private ICacheService _cacheService;
    private ApplicationDbContext _dbContext;

    public UserService(ICacheService cacheService, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _cacheService = cacheService;
    }
    public UserEntity GetUser(string username, bool includeCache = false)
    {
        UserEntity user;
        if (includeCache)
        {
            try
            {
                user = _cacheService.GetData<UserEntity>(username);
                if (user != null)
                {
                    return user;
                }
                
            }
            catch (Exception exception)
            {
                
            }
        }
           
        user = _dbContext.Users.FirstOrDefault(u => u.UserName == username);
            
        if (user != null)
        {
            try
            {
                _cacheService.SetData(user.UserName, user, TimeSpan.FromDays(2));
            }
            catch (Exception e)
            {
                   
            }
              

        }

        return user;
    }
}