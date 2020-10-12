using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class ArtistInfo
    {
        public ArtistInfo()
        {
            BgImgUrls = new List<string>();
        }
        /// <summary>
        /// 背景图片
        /// </summary>
        public List<string> BgImgUrls { get; set; }

        public void AddBgImgURL(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                BgImgUrls.Add(url);
            }
        }

        public string GetFirstBgImgUrl()
        {
            if (HasData())
            {
                return BgImgUrls[0];
            }
            return string.Empty;
        }

        public bool HasData()
        {
            return BgImgUrls != null && BgImgUrls.Count > 0;
        }
    }
}
