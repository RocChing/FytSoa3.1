﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FytSoa.Core.Model.Music;
using FytSoa.Service.Interfaces.Music;
using FytSoa.Service.Repository;
using FytSoa.Common;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;
using FytSoa.Core;

namespace FytSoa.Service.Implements.Music
{
    [IocService]
    public class MusicService : BaseService<MusicInfo>, IMusicService
    {
        private IListService listService;
        private IMusicListService musicListService;
        public MusicService(IListService listService, IMusicListService musicListService)
        {
            this.listService = listService;
            this.musicListService = musicListService;
        }

        public List<IMusic> GetMusics(string kw, int top = 10)
        {
            if (top < 1)
            {
                top = 10;
            }
            List<IMusic> list = new List<IMusic>();
            try
            {
                string url = AppConstant.BLOG_API_URL;
                byte[] commit = Encoding.UTF8.GetBytes($"types=search&count={top}&source=kugou&pages=1&name={kw}");
                byte[] data = HttpWebClient.Post(url, commit);
                string json = Encoding.UTF8.GetString(data);
                var kgList = JsonConvert.DeserializeObject<KugouMusicInfo[]>(json);
                if (kgList != null && kgList.Length > 0)
                {
                    foreach (var item in kgList)
                    {
                        list.Add(item.ToKugouMusic());
                    }
                }
                return list;
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return list;
            }
        }

        public async Task<List<IMusic>> GetMusicsWithDb(SearchInput input)
        {
            string name = input.Name;
            string author = input.Author;

            List<IMusic> list = new List<IMusic>();
            var dbList = await this.GetListAsync(m => m.Name == name && m.Artists.Contains(author), m => m.Id, DbOrderEnum.Desc);
            if (dbList != null && dbList.Count > 0)
            {
                foreach (var item in dbList)
                {
                    IMusic m = item.ToMusic();
                    if (m != null && !string.IsNullOrEmpty(m.Id))
                    {
                        list.Add(m);
                    }
                }
            }
            if (list.Count > 0)
            {
                return list;
            }
            return GetMusics(name, input.Top);
        }

        public async Task<bool> AddMusicListBySearch(List<SearchInput> list)
        {
            if (list == null || list.Count < 1)
            {
                return false;
            }
            foreach (var item in list)
            {
                await AddMusicBySearch(item);
            }
            return true;
        }

        public async Task<bool> AddMusicBySearch(SearchInput input)
        {
            var list = await GetMusicsWithDb(input);
            Logger.Default.Info("the list json is:" + JsonConvert.SerializeObject(list));
            if (list != null && list.Count > 0)
            {
                bool addMusic = false;
                IMusic music = null;
                foreach (var item in list)
                {
                    addMusic = false;
                    if (item.Name == input.Name && item.Artists.Contains(input.Author))
                    {
                        if (string.IsNullOrEmpty(item.ConverUrl))
                        {
                            addMusic = true;
                            item.ConverUrl = GetCoverUrl(item);
                            if (string.IsNullOrEmpty(item.ConverUrl)) continue;
                        }

                        if (string.IsNullOrEmpty(item.MusicUrl))
                        {
                            addMusic = true;
                            item.MusicUrl = GetMusicUrl(item);
                            if (string.IsNullOrEmpty(item.MusicUrl)) continue;
                        }

                        if (item.LrcInfo == null)
                        {
                            addMusic = true;
                            item.LrcInfo = GetLyricInfo(item);
                            if (item.LrcInfo == null) continue;
                        }

                        music = item;
                        break;
                    }
                }

                if (music != null)
                {
                    if (addMusic)
                    {
                        var musicInfo = new MusicInfo(music);
                        await this.AddMusicInfo(musicInfo);
                    }
                    string listName = DateTime.Now.ToString("yyyy-MM-dd");
                    int listId = await listService.Insert(listName);
                    await musicListService.Insert(new MusicListInfo(listId, music.Id));
                }
                else
                {
                    Logger.Default.Warn("没有找到合适的歌曲!");
                }
                return true;
            }
            return false;
        }

        private string GetCoverUrl(IMusic music)
        {
            try
            {
                string url = $"{AppConstant.KUGOU_API_URL}?r=play/getdata&hash={music.Id}";
                CookieCollection cookies = new CookieCollection();
                cookies.Add(new Cookie("kg_mid", "emmmm"));
                byte[] data = HttpWebClient.Get(url, "", ref cookies);
                string json = Encoding.UTF8.GetString(data);
                Logger.Default.Info("the json is :" + json);
                return JObject.Parse(json)["data"]["img"].ToString();
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return string.Empty;
            }
        }

        public LrcInfo GetLyricInfo(IMusic music)
        {
            try
            {
                string url = AppConstant.BLOG_API_URL;
                byte[] commit = Encoding.UTF8.GetBytes($"types=lyric&id={music.Id}&source=kugou");
                byte[] data = HttpWebClient.Post(url, commit);
                JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
                string content = json["lyric"].ToString();
                return LrcInfo.Parse(content);
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return null;
            }
        }

        public string GetMusicUrl(IMusic music)
        {
            string url = AppConstant.BLOG_API_URL;
            byte[] commit = Encoding.UTF8.GetBytes($"types=url&id={music.Id}&source=kugou");
            byte[] data = HttpWebClient.Post(url, commit);
            string json = Encoding.UTF8.GetString(data);
            Logger.Default.Info($"the json value is:{json}");
            JObject obj = JObject.Parse(json);
            return obj["url"].ToString();
        }

        private async Task AddMusicInfo(MusicInfo info)
        {
            var model = await this.GetModelAsync(m => m.MusicId == info.MusicId);
            if (model != null && model.Id > 0)
            {
                return;
            }
            await this.AddAsync(info);
        }
    }
}
