using FytSoa.Core.Model.Music;
using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.ViewModel.Music
{
    public class NetApiMusicInfo
    {
        public List<SongInfo> Songs { get; set; }

        public bool HasMore { get; set; }

        public int SongCount { get; set; }

        public bool HasData { get { return Songs != null && Songs.Count > 0; } }
    }
}
