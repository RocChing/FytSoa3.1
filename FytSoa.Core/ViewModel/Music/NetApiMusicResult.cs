using System;
using System.Collections.Generic;
using System.Text;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class NetApiMusicResult : NetApiResult
    {
        public NetApiMusicInfo Result { get; set; }
    }
}
