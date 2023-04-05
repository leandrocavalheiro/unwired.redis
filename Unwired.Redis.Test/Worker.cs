using Unwired.Redis.Interfaces;

namespace Unwired.Redis.Test
{
    public class Worker : BackgroundService
    {
        private readonly IURedis _unwiredRedis;
        public Worker(IURedis unwiredRedis)
        {
            _unwiredRedis = unwiredRedis;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await TestSaveAsync(stoppingToken);
            await TestDeleteAsync(stoppingToken);
            await TestGetAsync(stoppingToken);            
        }

        private async Task TestSaveAsync(CancellationToken stoppingToken)
        {

            await _unwiredRedis.SaveAsync("SaveAsync1", "1");
            await _unwiredRedis.SaveAsync("SaveAsync2", "2");
            await _unwiredRedis.SaveAsync("SaveAsync3", "3");
            await _unwiredRedis.SaveAsync("SaveAsync4", "4");

            

            await Task.CompletedTask;
        }
        private async Task TestGetAsync(CancellationToken stoppingToken)
        {

            var (result1, _, _) = await _unwiredRedis.GetKeyAsync<string>("SaveAsync1");
            Console.WriteLine($"SaveAsync1: {result1}");

            var (result2, _, _)  = await _unwiredRedis.GetKeyAsync<string>("SaveAsync2");
            Console.WriteLine($"SaveAsync2: {result2}");

            var (result3, _, _)  = await _unwiredRedis.GetKeyAsync<string>("SaveAsync3");
            Console.WriteLine($"SaveAsync3: {result3}");

            var (result4, _, _)  = await _unwiredRedis.GetKeyAsync<string>("SaveAsync4");
            Console.WriteLine($"SaveAsync4: {result4}");

            await Task.CompletedTask;
        }
        private async Task TestDeleteAsync(CancellationToken stoppingToken)
        {            
            await _unwiredRedis.DeleteAsync("SaveAsync3");            
            await Task.CompletedTask;
        }
    }
}