using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class JiuKuMusic : IMusic
    {
        public string Id { get; set; }

        public string Artists { get; set; }

        public string Name { get; set; }

        public string Album { get; set; }

        public string SourceName { get { return "9酷"; } }

        public MusicOrigin Origin { get { return MusicOrigin.JiuKu; } }

        public string ConverUrl { get; set; }

        public LrcInfo LrcInfo { get; set; }

        public string MusicUrl { get; set; }

        public ArtistInfo ArtistInfo { get; set; }

        public JiuKuMusic() { }
    }
}
