using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class MusicListViewModel2
    {
        public int ListId { get; set; }

        public int SortId { get; set; }

        public int Id { get; set; }

        public MusicInfo Music { get; set; }
    }
}
