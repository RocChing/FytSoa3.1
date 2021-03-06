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
using SqlSugar;
using FytSoa.Core.ViewModel.Music;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using MySql.Data.Common;

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

        public async Task<List<MusicListViewModel>> UpdateSortId(int id, int sortId)
        {
            bool flag = await musicListService.UpdateSortId(id, sortId);
            if (flag)
            {
                return await GetMusicsWithDb(GetCurrentListName());
            }
            return null;
        }

        public async Task<IMusic> GetMusicByName(string name)
        {
            var model = await this.GetModelAsync(m => m.Name.Contains(name));
            if (model != null)
            {
                return model.ToMusic();
            }
            return null;
        }

        public async Task<List<MusicViewModel>> GetMusicList(GetSearchInput input)
        {
            string listName = input.ListName;
            if (string.IsNullOrEmpty(listName))
            {
                listName = GetCurrentListName();
            }
            var list = await Db.Queryable<MusicListInfo, ListInfo, MusicInfo>((ml, l, m) => new JoinQueryInfos(JoinType.Left, ml.ListId == l.Id, JoinType.Left, ml.MusicId == m.MusicId))
                  .WhereIF(!string.IsNullOrEmpty(listName), (ml, l, m) => l.Name == listName)
                  .WhereIF(!string.IsNullOrEmpty(input.Name), (ml, l, m) => m.Name.Contains(input.Name))
                  .WhereIF(!string.IsNullOrEmpty(input.Author), (ml, l, m) => m.Artists.Contains(input.Author))
                  .OrderBy((ml, l, m) => ml.SortId, OrderByType.Desc)
                  .OrderBy((ml, l, m) => ml.Id)
                  .Select((ml, l, m) => new { ml, l, m })
                  .ToListAsync();
            var resLIst = new List<MusicViewModel>();
            foreach (var item in list)
            {
                resLIst.Add(new MusicViewModel(item.m, item.ml.SortId));
            }
            return resLIst;
        }

        public async Task<bool> DeleteMusic(string musicId)
        {
            try
            {
                await musicListService.DeleteAsync(m => m.MusicId == musicId);
                await this.DeleteAsync(m => m.MusicId == musicId);
                string listName = GetCurrentListName();
                await listService.SubNumber(listName);
                return true;
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return false;
            }
        }

        public async Task<List<MusicListViewModel>> GetMusicsWithDb(string name)
        {
            List<MusicListViewModel> viewModel2s = new List<MusicListViewModel>();
            var list = await Db.Queryable<ListInfo>().WhereIF(!string.IsNullOrEmpty(name), m => m.Name == name).OrderBy(m => m.Id, OrderByType.Desc).ToListAsync();

            var list2 = await Db.Queryable<MusicListInfo, MusicInfo>((ml, m) => new JoinQueryInfos(JoinType.Left, ml.MusicId == m.MusicId))
                .Select((ml, m) => new MusicListViewModel2()
                {
                    ListId = ml.ListId,
                    SortId = ml.SortId,
                    Id = ml.Id,
                    Music = m
                })
                .ToListAsync();
            if (list != null && list.Count > 0)
            {
                foreach (var item in list)
                {
                    MusicListViewModel mm = new MusicListViewModel();
                    mm.ListName = item.Name;
                    mm.Number = item.Number;
                    mm.MusicList = new List<IMusic>();

                    if (list2 != null && list2.Count > 0)
                    {
                        var list3 = list2.Where(m => m.ListId == item.Id).OrderByDescending(m => m.SortId).ThenBy(m => m.Id).ToList();
                        if (list3 != null && list3.Count > 0)
                        {
                            foreach (var music in list3)
                            {
                                if (music.Music != null)
                                {
                                    mm.MusicList.Add(music.Music.ToMusic());
                                }
                            }
                        }
                    }
                    viewModel2s.Add(mm);
                }
            }
            return viewModel2s;
        }

        public List<IMusic> GetMusics(string kw, int top = 10)
        {
            if (top < 1)
            {
                top = 10;
            }

            return GetJiuKuMusics(kw, top);

            List<IMusic> list = new List<IMusic>();
            try
            {
                //string url = AppConstant.BLOG_API_URL;
                //byte[] commit = Encoding.UTF8.GetBytes($"types=search&count={top}&source=kugou&pages=1&name={kw}");
                //byte[] data = HttpWebClient.Post(url, commit);
                //string json = Encoding.UTF8.GetString(data);
                //var kgList = JsonConvert.DeserializeObject<KugouMusicInfo[]>(json);
                //if (kgList != null && kgList.Length > 0)
                //{
                //    foreach (var item in kgList)
                //    {
                //        list.Add(item.ToKugouMusic());
                //    }
                //}

                string url = $"{AppConstant.NET_API_URL}/search?&limit={top}&keywords={kw}";

                byte[] data2 = HttpWebClient.Get(url);
                string json2 = Encoding.UTF8.GetString(data2);
                Logger.Default.Debug(json2);
                var res = JsonConvert.DeserializeObject<NetApiMusicResult>(json2);
                if (res != null && res.Success)
                {
                    var ress = res.Result;
                    if (ress != null && ress.HasData)
                    {
                        foreach (var item in ress.Songs)
                        {
                            list.Add(item.ToMusic());
                        }
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

        public List<IMusic> GetJiuKuMusics(string kw, int top = 10)
        {
            List<IMusic> list = new List<IMusic>();
            try
            {
                string url = $"http://baidu.9ku.com/song/?key={kw}";
                var bytes = HttpWebClient.Get(url);
                string html = Encoding.UTF8.GetString(bytes);
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);
                var root = document.DocumentNode;
                var lis = root.SelectNodes("//div[@class='songList']/ul/li");
                if (lis != null && lis.Count > 0)
                {
                    foreach (var li in lis)
                    {
                        JiuKuMusic info = new JiuKuMusic();
                        var song = li.SelectSingleNode("a[@class='songName']");
                        if (song != null)
                        {
                            info.Name = song.InnerText;
                            string href = song.Attributes["href"].Value;
                            if (!string.IsNullOrEmpty(href))
                            {
                                string str = href.Substring(href.LastIndexOf("/") + 1);
                                info.Id = str.Substring(0, str.IndexOf("."));
                            }
                        }
                        var singer = li.SelectSingleNode("a[@class='singerName']");
                        if (singer != null)
                        {
                            info.Artists = singer.InnerText.Trim();
                        }
                        var album = li.SelectSingleNode("a[@class='albumName']");
                        if (album != null)
                        {
                            info.Album = album.InnerText.Trim();
                        }
                        list.Add(info);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
            }
            return list;
        }

        public async Task<List<IMusic>> GetMusicsWithDb(SearchInput input)
        {
            string name = input.Name;
            string author = input.Author;
            string key = name;

            List<MusicInfo> dbList = null;
            List<IMusic> list = new List<IMusic>();

            bool flag = true;
            if (string.IsNullOrEmpty(name))
            {
                key = author;
                flag = false;
            }

            if (string.IsNullOrEmpty(author))
            {
                flag = false;
            }

            if (flag)
            {
                dbList = await this.GetListAsync(m => m.Name == name && m.Artists.Contains(author), m => m.Id, DbOrderEnum.Desc);
            }
            else
            {
                dbList = await Db.Queryable<MusicInfo>()
                   .WhereIF(!string.IsNullOrEmpty(key), m => m.Name == key || m.Artists.Contains(key))
                   .OrderBy(m => m.Id, OrderByType.Desc)
                   .ToListAsync();
            }

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

            string word = input.Word;
            if (string.IsNullOrEmpty(word))
            {
                word = name;
            }
            return GetMusics(word, input.Top);
        }

        public async Task<List<IMusic>> AddMusicListBySearch(List<SearchInput> list)
        {
            if (list == null || list.Count < 1)
            {
                return null;
            }
            List<IMusic> musics = new List<IMusic>();
            foreach (var item in list)
            {
                var m = await AddMusicBySearch(item);
                if (musics != null)
                {
                    musics.Add(m);
                }
            }
            return musics;
        }

        public async Task<IMusic> AddMusicBySearch(SearchInput input)
        {
            var list = await GetMusicsWithDb(input);
            if (list != null && list.Count > 0)
            {
                bool findMusic = true;
                IMusic music = null;

                bool flag = !string.IsNullOrEmpty(input.Id);
                if (flag)
                {
                    music = list.FirstOrDefault(m => m.Id == input.Id);
                }

                if (music == null)
                {
                    music = list.FirstOrDefault(m =>
                    {
                        bool flag = true;
                        string name = input.Name;
                        string author = input.Author;
                        string key = name;
                        if (!string.IsNullOrEmpty(name))
                        {
                            flag = flag && m.Name == name;
                        }
                        if (!string.IsNullOrEmpty(author))
                        {
                            flag = flag && m.Artists.Contains(author);
                        }
                        return flag;
                    });
                }

                if (music == null)
                {
                    Logger.Default.Warn("没有找到合适的歌曲!");
                    return null;
                }

                if (music != null)
                {
                    Logger.Default.Debug(JsonConvert.SerializeObject(music));
                    if (string.IsNullOrEmpty(music.ConverUrl))
                    {
                        music.ConverUrl = GetCoverUrl(music);
                        Logger.Default.Debug("ConverUrl url:" + music.ConverUrl);
                    }

                    if (string.IsNullOrEmpty(music.MusicUrl))
                    {
                        music.MusicUrl = GetMusicUrl(music);
                        Logger.Default.Debug("MusicUrl url:" + music.MusicUrl);
                        if (string.IsNullOrEmpty(music.MusicUrl))
                        {
                            findMusic = false;
                        }
                    }

                    if (music.LrcInfo == null)
                    {
                        music.LrcInfo = GetLyricInfo(music);
                        Logger.Default.Debug("LrcInfo json:" + JsonConvert.SerializeObject(music.LrcInfo));
                        if (music.LrcInfo == null)
                        {
                            findMusic = false;
                        }
                    }

                    if (music.ArtistInfo == null || !music.ArtistInfo.HasData())
                    {
                        music.ArtistInfo = GetArtistInfo(music);
                    }

                    if (string.IsNullOrEmpty(music.ConverUrl))
                    {
                        music.ConverUrl = music.ArtistInfo.GetFirstBgImgUrl();
                    }
                }

                if (findMusic)
                {
                    var musicInfo = new MusicInfo(music);
                    await SaveMusicInfo(musicInfo);

                    string listName = GetCurrentListName();
                    int listId = await listService.Insert(listName);

                    bool flag4 = await musicListService.Insert(new MusicListInfo(listId, music.Id, input.SortId));
                    return flag4 ? music : null;
                }
                else
                {
                    Logger.Default.Warn("没有找到合适的歌曲!");
                }
            }
            return null;
        }

        private ArtistInfo GetArtistInfo(IMusic music)
        {
            ArtistInfo info = new ArtistInfo();
            try
            {
                string artists = music.Artists;
                string[] strs = artists.Split(',', '|', ' ', '、');
                string url = $"https://cn.bing.com/images/async?q={strs[0]}&first=0&count=10&scenario=ImageBasicHover&datsrc=I&layout=RowBased&mmasync=1&qft=+filterui:imagesize-custom_1200_700+filterui:photo-photo+filterui:aspect-wide";
                var bytes = HttpWebClient.Get(url);
                string html = Encoding.UTF8.GetString(bytes);
                Regex reg = new Regex("m=\"(.*?)\"", RegexOptions.IgnoreCase);
                var matches = reg.Matches(html);
                if (matches != null && matches.Count > 0)
                {
                    foreach (Match item in matches)
                    {
                        string json = item.Groups[1].Value;
                        if (string.IsNullOrEmpty(json))
                        {
                            continue;
                        }
                        string bgUrl = string.Empty;
                        try
                        {
                            json = Utils.HtmlDecode(json);
                            var obj = JObject.Parse(json);
                            obj.TryGetValue("murl", out JToken t);
                            if (t != null)
                            {
                                bgUrl = t.ToString();
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.Default.Error(e.Message, e);
                        }
                        info.AddBgImgURL(bgUrl);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
            }
            return info;
        }

        private string GetCoverUrl(IMusic music)
        {
            try
            {
                var origin = music.Origin;
                if (origin == MusicOrigin.Netease)
                {
                    string pic_id = ((NeteaseMusic)music).PicId;
                    string pwd = Utils.EncryptNeteaseId(pic_id);
                    return $"https://p1.music.126.net/{pwd}/{pic_id}.jpg";//?param=300y300
                }
                else if (origin == MusicOrigin.JiuKu)
                {
                    return null;
                }
                string url = $"{AppConstant.KUGOU_API_URL}?r=play/getdata&hash={music.Id}";
                CookieCollection cookies = new CookieCollection();
                cookies.Add(new Cookie("kg_mid", "emmmm"));
                byte[] data = HttpWebClient.Get(url, "", ref cookies);
                string json = Encoding.UTF8.GetString(data);
                Logger.Default.Info("the conver json value is :" + json);
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
                var origin = music.Origin;
                if (origin == MusicOrigin.Netease)
                {
                    string url = $"{AppConstant.NET_API_URL}/lyric?id={music.Id}";
                    byte[] data = HttpWebClient.Get(url);
                    string json = Encoding.UTF8.GetString(data);
                    var res = JsonConvert.DeserializeObject<NetApiLyricResult>(json);
                    if (res != null && res.Success)
                    {
                        if (res.Lrc != null)
                        {
                            return LrcInfo.Parse(res.Lrc.Lyric);
                        }
                    }
                    return null;
                }
                else if (origin == MusicOrigin.JiuKu)
                {
                    string url = $"http://www.9ku.com/play/{music.Id}.htm";
                    var bytes = HttpWebClient.Get(url);
                    string html = Encoding.UTF8.GetString(bytes);
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);
                    var root = document.DocumentNode;
                    var textarea = root.SelectSingleNode("//div[@id='lrctext']/textarea");
                    if (textarea != null)
                    {
                        return LrcInfo.Parse(textarea.InnerText);
                    }
                    return null;
                }
                else
                {
                    string type = GetOriginName(music.Origin);
                    string url = AppConstant.BLOG_API_URL;
                    byte[] commit = Encoding.UTF8.GetBytes($"types=lyric&id={music.Id}&source={type}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    JObject json = JObject.Parse(Encoding.UTF8.GetString(data));
                    string content = json["lyric"].ToString();
                    return LrcInfo.Parse(content);
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return null;
            }
        }

        public string GetMusicUrl(IMusic music)
        {
            try
            {
                var origin = music.Origin;
                if (origin == MusicOrigin.Netease)
                {
                    return string.Format(AppConstant.NET_API_MUSIC_URL, music.Id);
                    string url = $"{AppConstant.NET_API_URL}/song/url?id={music.Id}";
                    byte[] data = HttpWebClient.Get(url);
                    string json = Encoding.UTF8.GetString(data);
                    Logger.Default.Info($"the url json value is:{json}");
                    var res = JsonConvert.DeserializeObject<NetApiMusicUrlResult>(json);
                    if (res != null && res.Success)
                    {
                        return res.GetFirstUrl();
                    }
                    return "";
                }
                else if (origin == MusicOrigin.JiuKu)
                {
                    decimal mid = decimal.Parse(music.Id);
                    int id = (int)Math.Floor(mid / 1000) + 1;
                    string url = $"http://www.9ku.com/html/playjs/{id}/{music.Id}.js";
                    var bytes = HttpWebClient.Get(url);
                    string json = Encoding.UTF8.GetString(bytes);
                    if (!string.IsNullOrEmpty(json))
                    {
                        json = json.Substring(1, json.Length - 2);
                    }

                    var obj = JObject.Parse(json);
                    Logger.Default.Info("the json value is :" + json);
                    return obj["wma"].ToString();
                }
                else
                {
                    string type = GetOriginName(music.Origin);
                    string url = AppConstant.BLOG_API_URL;
                    byte[] commit = Encoding.UTF8.GetBytes($"types=url&id={music.Id}&source={type}");
                    byte[] data = HttpWebClient.Post(url, commit);
                    string json = Encoding.UTF8.GetString(data);
                    Logger.Default.Info($"the url json value is:{json}");
                    JObject obj = JObject.Parse(json);
                    return obj["url"].ToString();
                }
            }
            catch (Exception e)
            {
                Logger.Default.Error(e.Message, e);
                return null;
            }
        }

        private async Task SaveMusicInfo(MusicInfo info)
        {
            var model = await this.GetModelAsync(m => m.MusicId == info.MusicId);
            if (model != null && model.Id > 0)
            {
                await UpdateAsync(info);
                return;
            }
            await this.AddAsync(info);
        }

        private string GetOriginName(MusicOrigin origin)
        {
            string type = "";
            switch (origin)
            {
                case MusicOrigin.Kugou:
                    type = "kugou";
                    break;
                case MusicOrigin.Netease:
                    type = "netease";
                    break;
            }
            return type;
        }

        private string GetCurrentListName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
