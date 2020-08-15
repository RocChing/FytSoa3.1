using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class MusicViewModel
    {
        public int Id { get; set; }

        public int SortId { get; set; }

        public string MusicId { get; set; }

        public string Name { get; set; }

        public string Artists { get; set; }

        public MusicViewModel()
        { }

        public MusicViewModel(MusicInfo info, int sortId)
        {
            if (info != null)
            {
                MusicId = info.MusicId;
                Name = info.Name;
                Artists = info.Artists;
                SortId = sortId;
            }
        }
    }
}
