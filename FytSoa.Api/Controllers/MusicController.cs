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
        private IListService listService;

        public MusicController(IMusicService musicService, IListService listService, IHubContext<SongHub> hub)
        {
            this.musicService = musicService;
            this.listService = listService;
            this.hub = hub;
        }

        [HttpGet("test")]
        public ApiResult<bool> Test()
        {
            bool flag = musicService.Test();
            return ApiResult<bool>.Success(true);
        }

        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMusicId"></param>
        /// <returns></returns>
        [HttpGet("playMusic")]
        public async Task<ApiResult<string>> PlayMusic(string name, bool isMusicId = false)
        {
            string id = string.Empty;
            if (isMusicId)
            {
                id = name;
            }
            else
            {
                var model = await musicService.GetMusicByName(name);
                if (model != null)
                {
                    id = model.Id;
                }
            }
            if (string.IsNullOrEmpty(id))
            {
                return ApiResult<string>.Success("播放失败");
            }
            else
            {
                await SendPlayMusicMsg(id);
                return ApiResult<string>.Success("播放成功");
            }
        }

        /// <summary>
        /// 一键复制歌单
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("updateListName")]
        public async Task<ApiResult<string>> UpdateListName(string name = "")
        {
            var flag = await listService.UpdateName(name);
            string msg = flag ? "歌单复制成功" : "歌单复制失败";
            return ApiResult<string>.Success(msg);
        }

        /// <summary>
        /// 添加歌曲
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("add")]
        public async Task<ApiResult<string>> AddMusicBySearch([FromQuery] SearchInput input)
        {
            string fansName = input.FansName;
            if (string.IsNullOrEmpty(fansName))
            {
                fansName = "管理员";
            }
            var model = await musicService.AddMusicBySearch(input);
            await SendAddMusicMsg(model, fansName);
            string msg = model != null ? "歌曲添加成功" : "没有找到合适的歌曲";
            return ApiResult<string>.Success(msg);
        }

        /// <summary>
        /// 搜索歌曲列表(网络)
        /// </summary>
        /// <param name="name">歌曲名称或者歌手</param>
        /// <param name="top">显示数量</param>
        /// <returns></returns>
        [HttpGet("search")]
        public ApiResult<List<IMusic>> Index(string name, int top = 10)
        {
            var list = musicService.GetMusics(name, top);
            return ApiResult<List<IMusic>>.Success(list);
        }

        /// <summary>
        /// 修改排序
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="sortId">排序数字(数字越大越靠前)</param>
        /// <returns></returns>
        [HttpGet("updateSortId")]
        public async Task<ApiResult<string>> UpdateSortId(int id, int sortId)
        {
            var list = await musicService.UpdateSortId(id, sortId);
            string msg;
            if (list != null && list.Count > 0)
            {
                msg = "歌曲排序修改成功";
                await SendUpdateMusicSortIdMsg(list);
            }
            else
            {
                msg = "歌曲排序修改失败";
            }
            return ApiResult<string>.Success(msg);
        }

        /// <summary>
        /// 查询歌曲列表(本地)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("getMusicList")]
        public async Task<ApiResult<List<MusicViewModel>>> GetMusicList([FromQuery] GetSearchInput input)
        {
            var list = await musicService.GetMusicList(input);
            return ApiResult<List<MusicViewModel>>.Success(list);
        }

        /// <summary>
        /// 删除歌曲
        /// </summary>
        /// <param name="musicId">歌曲ID</param>
        /// <returns></returns>
        [HttpGet("delete")]
        public async Task<ApiResult<string>> DeleteMusic(string musicId)
        {
            var flag = await musicService.DeleteMusic(musicId);
            string msg = flag ? "歌曲删除成功" : "歌曲删除失败";
            if (flag)
            {
                await SendDeleteMusicMsg(musicId);
            }
            return ApiResult<string>.Success(msg);
        }

        /// <summary>
        /// 添加歌曲列表
        /// </summary>
        /// <param name="list">歌曲列表</param>
        /// <returns></returns>
        [HttpPost("addList")]
        public async Task<ApiResult<string>> AddMusicListBySearch([FromBody] List<SearchInput> list)
        {
            string fansName = string.Empty;
            if (list != null && list.Count > 0)
            {
                fansName = list.FirstOrDefault().FansName;
            }
            if (string.IsNullOrEmpty(fansName))
            {
                fansName = "管理员";
            }
            var musics = await musicService.AddMusicListBySearch(list);
            await SendAddMusicMsg(musics, fansName);
            string msg = musics != null && musics.Count > 0 ? "歌曲添加成功" : "没有找到合适的歌曲";
            return ApiResult<string>.Success(msg);
        }

        /// <summary>
        /// 搜索歌曲(先搜索本地,再网络)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("searchWithDb")]
        public async Task<ApiResult<List<IMusic>>> GetMusicsWithDb([FromQuery]SearchInput input)
        {
            var list = await musicService.GetMusicsWithDb(input);
            return ApiResult<List<IMusic>>.Success(list);
        }

        /// <summary>
        /// 获得音乐列表
        /// </summary>
        /// <param name="name">关键字</param>
        /// <returns></returns>
        [HttpGet("getMusics")]
        public async Task<ApiResult<List<MusicListViewModel>>> GetMusics(string name = "")
        {
            var list = await musicService.GetMusicsWithDb(name);
            return ApiResult<List<MusicListViewModel>>.Success(list);
        }

        private async Task SendAddMusicMsg(IMusic model, string fansName)
        {
            if (model != null && !string.IsNullOrEmpty(model.Id))
            {
                List<IMusic> list = new List<IMusic>();
                list.Add(model);
                await SendAddMusicMsg(list, fansName);
            }
        }

        private async Task SendAddMusicMsg(List<IMusic> list, string fansName)
        {
            if (list != null && list.Count > 0)
            {
                await hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_AddSong, list, fansName);
            }
        }

        private async Task SendDeleteMusicMsg(string musicId)
        {
            await hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_DeleteSong, musicId);
        }

        private async Task SendPlayMusicMsg(string musicId)
        {
            await hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_PlaySong, musicId);
        }

        private async Task SendUpdateMusicSortIdMsg(List<MusicListViewModel> list)
        {
            if (list != null && list.Count > 0)
            {
                var model = list.FirstOrDefault();
                if (model != null)
                {
                    await hub.Clients.All.SendAsync(Core.AppConstant.SONG_HUB_UpdateSongSortId, model);
                }
            }
        }
    }
}