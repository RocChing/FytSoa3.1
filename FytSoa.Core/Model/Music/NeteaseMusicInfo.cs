using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace FytSoa.Core.Model.Music
{
    public class NeteaseMusicInfo
    {
        public string Id { get; set; }

        public List<string> Artist { get; set; }

        public string Name { get; set; }

        public string Album { get; set; }

        public NeteaseMusic ToNeteaseMusic()
        {
            var m = new NeteaseMusic()
            {
                Album = Album,
                Id = Id,
                Name = Name
            };
            if (Artist != null && Artist.Count > 0)
            {
                m.Artists = string.Join(",", Artist);
            }
            return m;
        }
    }
}
