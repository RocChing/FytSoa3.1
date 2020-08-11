using System;
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

        public async Task<List<MusicListViewModel>> GetMusicsWithDb(string name)
        {
            List<MusicListViewModel> viewModel2s = new List<MusicListViewModel>();
            var list = await Db.Queryable<ListInfo>().WhereIF(!string.IsNullOrEmpty(name), m => m.Name == name).OrderBy(m => m.Id, OrderByType.Desc).ToListAsync();
            //var list = await listService.GetListAsync(it => true, it => it.Id, DbOrderEnum.Desc);

            var list2 = await Db.Queryable<MusicListInfo>().Mapper(it => it.Music, it => it.MusicId).ToListAsync();
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
                        var list3 = list2.FindAll(m => m.ListId == item.Id);
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
            //Logger.Default.Info("the list json is:" + JsonConvert.SerializeObject(list));
            if (list != null && list.Count > 0)
            {
                bool addMusic = false;
                IMusic music = null;
                foreach (var item in list)
                {
                    addMusic = false;
                    if (item.Name == input.Name && item.Artists.Contains(input.Author))
                    {
                        Logger.Default.Debug(JsonConvert.SerializeObject(item));
                        if (string.IsNullOrEmpty(item.ConverUrl))
                        {
                            addMusic = true;
                            item.ConverUrl = GetCoverUrl(item);
                            Logger.Default.Debug("ConverUrl url:" + item.ConverUrl);
                            if (string.IsNullOrEmpty(item.ConverUrl)) continue;
                        }

                        if (string.IsNullOrEmpty(item.MusicUrl))
                        {
                            addMusic = true;
                            item.MusicUrl = GetMusicUrl(item);
                            Logger.Default.Debug("MusicUrl url:" + item.MusicUrl);
                            if (string.IsNullOrEmpty(item.MusicUrl)) continue;
                        }

                        if (item.LrcInfo == null)
                        {
                            addMusic = true;
                            item.LrcInfo = GetLyricInfo(item);
                            Logger.Default.Debug("LrcInfo json:" + JsonConvert.SerializeObject(item.LrcInfo));
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
                    bool flag = await musicListService.Insert(new MusicListInfo(listId, music.Id));
                    return flag ? music : null;
                }
                else
                {
                    Logger.Default.Warn("没有找到合适的歌曲!");
                }
            }
            return null;
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

        private async Task AddMusicInfo(MusicInfo info)
        {
            var model = await this.GetModelAsync(m => m.MusicId == info.MusicId);
            if (model != null && model.Id > 0)
            {
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
    }
}
