using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FytSoa.Core.Model.Music
{
    public class SongInfo
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int Duration { get; set; }

        public List<SingerInfo> Artists { get; set; }

        public AlbumInfo Album { get; set; }

        public IMusic ToMusic()
        {
            NeteaseMusic m = new NeteaseMusic()
            {
                Id = Id,
                Name = Name
            };
            if (Album != null)
            {
                m.Album = Album.Name;
                m.PicId = Album.PicId;
            }
            if (Artists != null && Artists.Count > 0)
            {
                m.Artists = string.Join(",", Artists.Select(m => m.Name));
            }
            return m;
        }
    }
}
