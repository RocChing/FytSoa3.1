using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;
using System.Linq;

namespace FytSoa.Core.ViewModel.Music
{
    public class NetApiMusicUrlResult : NetApiResult
    {
        public List<MusicUrlInfo> Data { get; set; }

        public bool HasData { get { return Data != null && Data.Count > 0; } }

        public string GetFirstUrl()
        {
            if (HasData)
            {
                return Data.FirstOrDefault().Url;
            }
            return "";
        }
    }
}
