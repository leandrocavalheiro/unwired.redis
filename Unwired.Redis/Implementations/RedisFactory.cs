using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Unwired.Redis.Interfaces;

namespace Avalia.Caching.Service.Implementations;

public class RedisFactory : IRedisFactory
{
    private readonly IConfiguration _configuration;
    public RedisFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public ConnectionMultiplexer CreateInstance()
    {
        var password = _configuration.GetSection("Redis:Password").Value;
        var hosts = _configuration.GetSection("Redis:Server").Value?.Split(',') ?? new string[] { "localhost:6379" };
        var ssl = _configuration.GetSection("Redis:SSL").Value?.ToLower().Equals("true") ?? false;        

        var redisOptions = new ConfigurationOptions();

        foreach (var host in hosts)
        {
            var endpoint = host.Split(":");
            redisOptions.EndPoints.Add(endpoint[0], Convert.ToInt32(endpoint[1]));
        }

        if (!string.IsNullOrEmpty(password))
            redisOptions.Password = $"{password}";

        redisOptions.AbortOnConnectFail = false;
        redisOptions.Ssl = ssl;
        redisOptions.AllowAdmin = true;
        redisOptions.SyncTimeout = 5000;        

        return ConnectionMultiplexer.Connect(redisOptions);
        
    }
    
}