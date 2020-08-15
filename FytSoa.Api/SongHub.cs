using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using FytSoa.Service.Interfaces;
using FytSoa.Service.Interfaces.Music;
using FytSoa.Core.ViewModel.Music;
using FytSoa.Common;

namespace FytSoa.Api
{
    public class SongHub : Hub
    {
        private IMusicService musicService;
        public SongHub(IMusicService musicService)
        {
            this.musicService = musicService;
        }

        public override async Task OnConnectedAsync()
        {
            string name = DateTime.Now.ToString("yyyy-MM-dd");
            var list = await musicService.GetMusicsWithDb(name);
            MusicListViewModel model = null;
            if (list != null && list.Count > 0)
            {
                model = list.FirstOrDefault();
            }
            string connectionId = Context.ConnectionId;
            await Clients.Client(connectionId).SendAsync(Core.AppConstant.SONG_HUB_OnConnected, model);
        }
    }
}
