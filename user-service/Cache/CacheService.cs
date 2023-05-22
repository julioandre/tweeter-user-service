using StackExchange.Redis;

namespace user_service.Cache;

public class CacheService:ICacheService
{
    private IDatabase _db;

    public CacheService()
    {
        ConfigureRedis();
    }
    private void ConfigureRedis() {
        // _db = ConnectionHelper.Connection.GetDatabase();
    }
    public T GetData<T>(string key)
    {
        throw new NotImplementedException();
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        throw new NotImplementedException();
    }

    public object RemoveData(string key)
    {
        throw new NotImplementedException();
    }
}