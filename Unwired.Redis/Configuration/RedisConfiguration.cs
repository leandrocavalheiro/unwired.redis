using Avalia.Caching.Service.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Unwired.Redis.Implementations;
using Unwired.Redis.Interfaces;

namespace Unwired.Redis.Configuration;

public static class RedisConfiguration
{
    public static void AddURedis(this IServiceCollection services, IConfiguration configuration)
    {

        /*var password = configuration.GetSection("Redis:Password").Value;
        var hosts = configuration.GetSection("Redis:EndPoint").Value?.Split(',') ?? new string[] { "localhost:6379" };
        var ssl = configuration.GetSection("Redis:SSL").Value?.ToLower().Equals("true") ?? false;

        var redisOptions = new ConfigurationOptions();


        foreach (var host in hosts)
        {
            var endpoint = host.Split(":");
            redisOptions.EndPoints.Add(endpoint[0], Convert.ToInt32(endpoint[1]));
        }

        if (!string.IsNullOrEmpty(password))
            redisOptions.Password = $"{password}";

        redisOptions.AbortOnConnectFail = false;
        redisOptions.Ssl = ssl;*/

        //var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
        /*services.AddTransient<IDatabase>(cfg =>
        {
            //IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisOptions);
            return multiplexer.GetDatabase();
        });*/

        services.AddTransient<IRedisFactory, RedisFactory>();
        services.AddTransient<IURedis, URedis>();


    }
}
