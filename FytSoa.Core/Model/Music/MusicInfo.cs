using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
namespace FytSoa.Core.Model.Music
{
    [SqlSugar.SugarTable("MusicInfo")]
    public class MusicInfo
    {
        [SqlSugar.SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public string MusicId { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public MusicOrigin Origin { get; set; }

        public string Data { get; set; }

        public DateTime CreateTime { get; set; }

        public IMusic ToMusic()
        {
            string json = Data;
            IMusic music = null;
            if (string.IsNullOrEmpty(json))
            {
                return music;
            }
            try
            {
                switch (Origin)
                {
                    case MusicOrigin.Netease:
                        music = JsonConvert.DeserializeObject<NeteaseMusic>(json);
                        break;
                    case MusicOrigin.Kugou:
                        music = JsonConvert.DeserializeObject<KugouMusic>(json);
                        break;
                    case MusicOrigin.JiuKu:
                        music = JsonConvert.DeserializeObject<JiuKuMusic>(json);
                        break;
                }
            }
            catch (Exception e)
            {
                FytSoa.Common.Logger.Default.Error(e.Message, e);
            }
            return music;
        }

        public MusicInfo()
        {
            CreateTime = DateTime.Now;
        }

        public MusicInfo(IMusic music)
        {
            if (music == null) return;
            MusicId = music.Id;
            Name = music.Name;
            Origin = music.Origin;
            Artists = music.Artists;
            Data = JsonConvert.SerializeObject(music);
            CreateTime = DateTime.Now;
        }
    }
}
