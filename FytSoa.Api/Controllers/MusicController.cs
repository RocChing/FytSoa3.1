using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FytSoa.Core.Model.Music;
using FytSoa.Service.Interfaces.Music;
using FytSoa.Common;
using FytSoa.Core.ViewModel.Music;
using Microsoft.AspNetCore.SignalR;
using System.Timers;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : Controller
    {
        private IHubContext<SongHub> hub;
        private IMusicService musicService;

        public MusicController(IMusicService musicService, IHubContext<SongHub> hub)
        {
            this.musicService = musicService;
            this.hub = hub;
        }

        [HttpGet("getPicStr")]
        public string test()
        {
            string picId = "6630055115561496";
            //u7KrT_jRkmxUFPApQi_L2w==
            //u7KrT_jRkmxUFPApQi_L2w==
            return Utils.EncryptNeteaseId(picId);
        }

        [HttpGet("getMusics")]
        public async Task<ApiResult<List<MusicListViewModel>>> GetMusics(string name = "")
        {
            var list = await musicService.GetMusicsWithDb(name);
            return ApiResult<List<MusicListViewModel>>.Success(list);
        }

        [HttpGet("search")]
        public ApiResult<List<IMusic>> Index(string name)
        {
            var list = musicService.GetMusics(name);
            return ApiResult<List<IMusic>>.Success(list);
        }

        [HttpGet("searchWithDb")]
        public async Task<ApiResult<List<IMusic>>> GetMusicsWithDb([FromQuery]SearchInput input)
        {
            var list = await musicService.GetMusicsWithDb(input);
            return ApiResult<List<IMusic>>.Success(list);
        }

        [HttpPost("add")]
        public async Task<ApiResult<string>> AddMusicBySearch([FromBody] SearchInput input)
        {
            var model = await musicService.AddMusicBySearch(input);
            await SendHubMsg(model);
            string msg = model != null ? "歌曲添加成功" : "没有找到合适的歌曲";
            return ApiResult<string>.Success(msg);
        }

        [HttpPost("addList")]
        public async Task<ApiResult<string>> AddMusicListBySearch([FromBody] List<SearchInput> list)
        {
            var musics = await musicService.AddMusicListBySearch(list);
            await SendHubMsg(musics);
            string msg = musics != null && musics.Count > 0 ? "歌曲添加成功" : "没有找到合适的歌曲";
            return ApiResult<string>.Success(msg);
        }

        private async Task SendHubMsg(IMusic model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Id))
            {
                List<IMusic> list = new List<IMusic>();
                list.Add(model);
                await SendHubMsg(list);
            }
        }

        private async Task SendHubMsg(List<IMusic> list)
        {
            if (list != null && list.Count > 0)
            {
                await hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_AddSong, list);
            }
        }
    }
}