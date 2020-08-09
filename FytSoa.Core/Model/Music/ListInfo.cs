using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    [SqlSugar.SugarTable("ListInfo")]
    public class ListInfo
    {
        [SqlSugar.SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }

        public int Number { get; set; }

        public int AllowDel { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public ListInfo()
        {
            AllowDel = 1;
        }

        public ListInfo(string name)
        {
            AllowDel = 1;
            Name = name;
        }
    }
}
