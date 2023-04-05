using System.Text.Json;
using System.Text.Json.Serialization;
using Unwired.Redis.Extensions;
using Unwired.Redis.Interfaces;
using Unwired.Redis.Resources;

namespace Unwired.Redis.Implementations;

public class URedis : IURedis
{
    private readonly IRedisFactory _redisFactory;
    private readonly JsonSerializerOptions _serializerSettings;
    public URedis(IRedisFactory redisFactory)
    {       
        _redisFactory = redisFactory;
        _serializerSettings = new JsonSerializerOptions() { ReferenceHandler = ReferenceHandler.Preserve };
    }

    public async Task<(bool sucess, string codeError, string error)> SaveAsync1(string key, string value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
                return (true, null, null);

            if (string.IsNullOrEmpty(value))
                value = "";

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);
            var success = await redisDb.StringSetAsync(key, value, expiresInMinutes.ToTimeSpan());
            if (success)
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
    public async Task<(bool sucess, string codeError, string error)> SaveAsync(string key, object value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
                return (true, null, null);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var jsonValue = "";
            if (value != null)
                jsonValue = Serialize(value);

            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);
            var success = false;

            success = await redisDb.StringSetAsync(key, jsonValue, expiresInMinutes.ToTimeSpan());
            if (success)
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }


    }    
    public (bool sucess, string codeError, string error) Save(string key, string value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
                return (true, null, null);

            if (string.IsNullOrEmpty(value))
                value = "";

            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var success = redisDb.StringSet(key, value, expiresInMinutes.ToTimeSpan());
            if (success)
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
    public (bool sucess, string codeError, string error) Save(string key, object value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
                return (true, null, null);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var jsonValue = "";
            if (value != null)
                jsonValue = Serialize(value);

            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);

            var success = redisDb.StringSet(key, jsonValue, expiresInMinutes.ToTimeSpan());
            if (success)
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
    public async Task<(bool sucess, string codeError, string error)>  BulkSaveAsync(Dictionary<string, string> values, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {            
            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);
            var transction = redisDb.CreateTransaction();

            if (!string.IsNullOrEmpty(prefixKey))           
                prefixKey += separatorKeys;
            
            foreach (var current in values)
                await redisDb.StringSetAsync($"{prefixKey}{current.Key}", current.Value ?? "", expiresInMinutes.ToTimeSpan());

            if (await transction.ExecuteAsync())
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
    public async Task<(bool sucess, string codeError, string error)> BulkSaveAsync<Entity>(ICollection<Entity> values, string[] KeyFieldName, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);
            var transction = redisDb.CreateTransaction();            
            bool result = true;

            foreach (var current in values)
            {
                var recordId = string.Empty;
                foreach (var currentKey in KeyFieldName)
                    recordId += $"{separatorKeys}{current.GetType().GetProperty(currentKey).GetValue(current)}";

                result = await redisDb.StringSetAsync($"{prefixKey}{recordId}", Serialize(current) ?? "", expiresInMinutes.ToTimeSpan());
                if (!result)
                    break;                
            }

            if (result && await transction.ExecuteAsync())
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }                   
    public async Task<(TResult result, string codeError, string error)> GetKeyAsync<TResult>(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var db = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))            
                key = $"{prefixKey}{separatorKeys}{key}";
            
            var value = (await db.StringGetAsync($"{key}"));

            if (value.HasValue && refreshExpiration)
                db.KeyExpire(key, expiresInMinutes.ToTimeSpan());

            return (DeSerialize<TResult>(value), null, null);
        }
        catch (Exception exception)
        {
            return (default(TResult), nameof(CustomMessage.InternalError), exception.Message);
        }        
    }
    public (TResult result, string codeError, string error) GetKey<TResult>(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var db = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var value =  db.StringGet($"{key}");

            if (value.HasValue && refreshExpiration)
                db.KeyExpire(key, expiresInMinutes.ToTimeSpan());

            return (DeSerialize<TResult>(value), null, null);
        }
        catch (Exception exception)
        {
            return (default(TResult), nameof(CustomMessage.InternalError), exception.Message);
        }        
    }
    public async Task<(bool sucess, string codeError, string error)> DeleteAsync(string key, string prefixKey = "", string separatorKeys = "|", int database = -1)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();            
            var redisDb = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))                                
                key = $"{prefixKey}{separatorKeys}{key}";

            if (await redisDb.KeyDeleteAsync(key))
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
    public (bool sucess, string codeError, string error) Delete(string key, string prefixKey = "", string separatorKeys = "|", int database = -1)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var redisDb = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            if (redisDb.KeyDelete(key))
                return (true, null, null);

            return (false, nameof(CustomMessage.FailedToSaveRecord), CustomMessage.FailedToSaveRecord);
        }
        catch (Exception exception)
        {
            return (false, nameof(CustomMessage.InternalError), exception.Message);
        }
    }
        
    private string Serialize<TObject>(TObject value)
        => JsonSerializer.Serialize(value, _serializerSettings);
    private TResult DeSerialize<TResult>(string value)
        => JsonSerializer.Deserialize<TResult>(value ?? "", _serializerSettings);
    private async Task<string> GetStringKeyAsync(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var db = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var value = (await db.StringGetAsync($"{key}"));

            if (value.HasValue && refreshExpiration)
                db.KeyExpire(key, expiresInMinutes.ToTimeSpan());

            return value;

        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        return default;
    }
    private string GetStringKey(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true)
    {
        try
        {
            using var redis = _redisFactory.CreateInstance();
            var db = redis.GetDatabase(database);

            if (!string.IsNullOrEmpty(prefixKey))
                key = $"{prefixKey}{separatorKeys}{key}";

            var value = db.StringGet($"{key}");

            if (value.HasValue && refreshExpiration)
                db.KeyExpire(key, expiresInMinutes.ToTimeSpan());

            return value;

        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

        return default;
    }


}
