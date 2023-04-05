using StackExchange.Redis;

namespace Unwired.Redis.Interfaces;

public interface IRedisFactory
{
    ConnectionMultiplexer CreateInstance();    
}
