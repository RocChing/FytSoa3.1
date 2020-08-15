using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    [SqlSugar.SugarTable("MusicListInfo")]
    public class MusicListInfo
    {
        [SqlSugar.SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public int ListId { get; set; }
        public string MusicId { get; set; }

        public int SortId { get; set; }

        //[SqlSugar.SugarColumn(IsIgnore = true)]
        //public ListInfo List { get; set; }

        //[SqlSugar.SugarColumn(IsIgnore = true)]
        //public MusicInfo Music { get; set; }

        public MusicListInfo()
        {
            SortId = 1000;
        }

        public MusicListInfo(int listId, string musicId, int sortId)
        {
            ListId = listId;
            MusicId = musicId;
            SortId = sortId;
        }
    }
}
