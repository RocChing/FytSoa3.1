using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class GetSearchInput
    {
        /// <summary>
        /// 列表名称
        /// </summary>
        public string ListName { get; set; }

        /// <summary>
        /// 歌曲名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 歌手
        /// </summary>
        public string Author { get; set; }
    }
}
