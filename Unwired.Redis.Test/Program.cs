using Unwired.Redis.Configuration;
using Unwired.Redis.Test;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
        services.AddURedis(configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
