using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public interface IMusic
    {
        /// <summary>
        /// 音乐链接ID
        /// </summary>
        string Id { get; set; }

        ///// <summary>
        ///// 封面id，只有网易云需要，其他音乐默认为ID
        ///// </summary>
        //string CoverId { get; set; }

        /// <summary>
        /// 歌手列表
        /// </summary>
        string Artists { get; set; }

        /// <summary>
        /// 歌曲名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 音乐专辑名称
        /// </summary>
        string Album { get; set; }

        /// <summary>
        /// 音乐来源名称
        /// </summary>
        string SourceName { get; }

        /// <summary>
        /// 音乐来源
        /// </summary>
        MusicOrigin Origin { get; }

        /// <summary>
        /// 专辑名称
        /// </summary>
        string ConverUrl { get; set; }

        /// <summary>
        /// 歌词信息
        /// </summary>
        LrcInfo LrcInfo { get; set; }

        /// <summary>
        /// 歌曲地址
        /// </summary>
        string MusicUrl { get; set; }

        /// <summary>
        /// 歌手信息
        /// </summary>
        ArtistInfo ArtistInfo { get; set; }
    }
}
