using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    [SqlSugar.SugarTable("SearchInfo")]
    public class SearchInfo
    {
        [SqlSugar.SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        public string SongName { get; set; }

        public string Artists { get; set; }

        public string FansName { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
