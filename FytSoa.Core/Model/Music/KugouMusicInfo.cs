using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class KugouMusicInfo
    {
        public string Id { get; set; }

        public List<string> Artist { get; set; }

        public string Name { get; set; }

        public string Album { get; set; }

        public KugouMusic ToKugouMusic()
        {
            var m = new KugouMusic()
            {
                Album = Album,
                Id = Id,
                Name = Name
            };
            if (Artist != null && Artist.Count>0)
            {
                m.Artists = string.Join(",", Artist);
            }
            return m;
        }
    }
}
