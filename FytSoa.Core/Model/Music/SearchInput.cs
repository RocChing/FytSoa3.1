using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FytSoa.Core.Model.Music
{
    public class SearchInput
    {
       
        /// <summary>
        /// 歌曲名称
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 歌手
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// 粉丝名称
        /// </summary>
        public string FansName { get; set; }

        /// <summary>
        /// 音乐ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 搜索前多少条
        /// </summary>
        [Range(1, 100)]
        public int Top { get; set; }

        /// <summary>
        /// 排序ID 越大越靠前
        /// </summary>
        public int SortId { get; set; }

        /// <summary>
        /// 搜索内容
        /// </summary>
        public string Word { get; set; }

        public SearchInput()
        {
            Top = 10;
            SortId = 100;
        }
    }
}
