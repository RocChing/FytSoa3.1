using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class MusicViewModel
    {
        public int SortId { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public string Url { get; set; }

        public string Album { get; set; }

        public string ConverUrl { get; set; }

        public LrcInfo LrcInfo { get; set; }

        public ArtistInfo ArtistInfo { get; set; }

        public MusicViewModel()
        { }

        public MusicViewModel(MusicInfo info, int sortId)
        {
            if (info != null)
            {
                Id = info.MusicId;
                Name = info.Name;
                Artists = info.Artists;
                SortId = sortId;
                var music = info.ToMusic();
                if (music != null)
                {
                    Url = music.MusicUrl;
                    LrcInfo = music.LrcInfo;
                    ArtistInfo = music.ArtistInfo;
                    Album = music.Album;
                    ConverUrl = music.ConverUrl;
                }
            }
        }

        public MusicViewModel(IMusic info, int sortId)
        {
            if (info != null)
            {
                Id = info.Id;
                Name = info.Name;
                Artists = info.Artists;
                SortId = sortId;

                Url = info.MusicUrl;
                LrcInfo = info.LrcInfo;
                ArtistInfo = info.ArtistInfo;
                Album = info.Album;
                ConverUrl = info.ConverUrl;
            }
        }
    }
}
