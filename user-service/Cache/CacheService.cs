using Newtonsoft.Json;
using StackExchange.Redis;
using user_service.Config;
using user_service.Models;

namespace user_service.Cache;

public class CacheService:ICacheService
{
    private IDatabase _db;

    public CacheService()
    {
        ConfigureRedis();
    }
    private void ConfigureRedis() {
        _db = ConnectionHelper.Connection.GetDatabase();
    }
    public UserEntity GetData<UserEntity>(string key)
    {
        var value = _db.StringGet(key);
        if (!string.IsNullOrEmpty(value)) {
            return JsonConvert.DeserializeObject <UserEntity> (value);
        }
        return default;
    }

    public bool SetData<UserEntity>(string key, UserEntity value, TimeSpan expirationTime)
    {
        if (CheckData(key) == true)
        {
            var isSet = _db.StringSet(key, JsonConvert.SerializeObject(value), expirationTime);
            return isSet; 
        }
        return false;
     
    }

    public object RemoveData(string key)
    {
        bool _isKeyExist = _db.KeyExists(key);
        if (_isKeyExist == true) {
            return _db.KeyDelete(key);
        }
        return false;
    }

    public bool CheckData(string key)
    {
        var _data = _db.StringGet(key);
        return _data.IsNullOrEmpty;
    }
}