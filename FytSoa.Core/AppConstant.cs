using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core
{
    public class AppConstant
    {
        public const string BLOG_API_URL = "http://blog.ylz1.cn/page/music/api.php";

        public const string KUGOU_API_URL = "https://wwwapi.kugou.com/yy/index.php";

        //public const string NET_API_URL = "http://192.168.1.100:8000";
        public const string NET_API_URL = "http://47.95.194.168:8800";

        public const string NET_API_MUSIC_URL = "https://music.163.com/song/media/outer/url?id={0}.mp3";

        public const string SONG_HUB_OnConnected = "OnConnected";

        public const string SONG_HUB_AddSong = "AddSong";

        public const string SONG_HUB_HeartBeat = "HeartBeat";
    }
}
