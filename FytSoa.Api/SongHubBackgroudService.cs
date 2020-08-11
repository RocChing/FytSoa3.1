using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace FytSoa.Api
{
    public class SongHubBackgroudService : BackgroundService
    {
        private System.Timers.Timer timer;
        private IHubContext<SongHub> hub;
        public SongHubBackgroudService(IHubContext<SongHub> hub)
        {
            this.hub = hub;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            InitTimer();
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ClearTimer();
            return base.StopAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        private void InitTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void ClearTimer()
        {
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_HeartBeat, 1);
        }
    }
}
