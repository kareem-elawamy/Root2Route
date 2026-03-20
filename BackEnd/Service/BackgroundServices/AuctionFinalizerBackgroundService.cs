using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Service.Services.AuctionService;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.BackgroundServices
{
    public class AuctionFinalizerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuctionFinalizerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var auctionService = scope.ServiceProvider.GetRequiredService<IAuctionService>();
                    await auctionService.FinalizeExpiredAuctionsAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
