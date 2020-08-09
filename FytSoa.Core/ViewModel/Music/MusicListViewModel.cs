using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class MusicListViewModel
    {
        public string ListName { get; set; }

        public int Number { get; set; }

        public List<IMusic> MusicList { get; set; }
    }
}
