using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class NeteaseMusic : IMusic
    {
        public string Id { get; set; }

        public string Artists { get; set; }

        public string Name { get; set; }

        public string Album { get; set; }

        public string SourceName { get { return "网易"; } }

        public MusicOrigin Origin { get { return MusicOrigin.Netease; } }

        public string ConverUrl { get; set; }

        public LrcInfo LrcInfo { get; set; }

        public string MusicUrl { get; set; }

        public string PicId { get; set; }
    }
}
