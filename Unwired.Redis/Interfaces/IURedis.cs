using Avalia.Caching.Service.Implementations;
using Unwired.Redis.Extensions;
using Unwired.Redis.Resources;

namespace Unwired.Redis.Interfaces;

public interface IURedis
{
    //Task<(bool sucess, string codeError, string error)> SaveAsync(string key, string value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);
    Task<(bool sucess, string codeError, string error)> SaveAsync(string key, object value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);
    (bool sucess, string codeError, string error) Save(string key, string value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);
    (bool sucess, string codeError, string error) Save(string key, object value, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);
    Task<(bool sucess, string codeError, string error)> BulkSaveAsync(Dictionary<string, string> values, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);
    Task<(bool sucess, string codeError, string error)> BulkSaveAsync<Entity>(ICollection<Entity> values, string[] KeyFieldName, string prefixKey = "", uint? expiresInMinutes = 1440, string separatorKeys = "|", int database = -1);    
    Task<(TResult result, string codeError, string error)> GetKeyAsync<TResult>(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true);
    (TResult result, string codeError, string error) GetKey<TResult>(string key = "*", string prefixKey = "", uint? expiresInMinutes = 1440, int database = -1, string separatorKeys = "|", bool refreshExpiration = true);
    Task<(bool sucess, string codeError, string error)> DeleteAsync(string key, string prefixKey = "", string separatorKeys = "|", int database = -1);
    (bool sucess, string codeError, string error) Delete(string key, string prefixKey = "", string separatorKeys = "|", int database = -1);
}