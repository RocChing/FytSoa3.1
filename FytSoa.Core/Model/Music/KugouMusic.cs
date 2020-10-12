using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class KugouMusic : IMusic
    {
        public string Id { get; set; }

        public string Artists { get; set; }

        public string Name { get; set; }

        public string Album { get; set; }

        public string SourceName { get { return "酷狗"; } }

        public MusicOrigin Origin { get { return MusicOrigin.Kugou; } }

        public string ConverUrl { get; set; }

        public LrcInfo LrcInfo { get; set; }

        public string MusicUrl { get; set; }

        public ArtistInfo ArtistInfo { get; set; }

        public KugouMusic() { }
    }
}
