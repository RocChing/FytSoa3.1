using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FytSoa.Core.Model.Music;
using FytSoa.Service.Interfaces.Music;
using FytSoa.Common;

namespace FytSoa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : Controller
    {
        private IMusicService musicService;
        public MusicController(IMusicService musicService)
        {
            this.musicService = musicService;
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
        public async Task<ApiResult<bool>> AddMusicBySearch([FromBody] SearchInput input)
        {
            bool flag = await musicService.AddMusicBySearch(input);
            return ApiResult<bool>.Success(flag);
        }

        [HttpPost("addList")]
        public async Task<ApiResult<bool>> AddMusicListBySearch([FromBody] List<SearchInput> list)
        {
            bool flag = await musicService.AddMusicListBySearch(list);
            return ApiResult<bool>.Success(flag);
        }
    }
}