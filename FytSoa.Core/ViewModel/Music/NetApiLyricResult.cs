using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FytSoa.Core.Model.Music;

namespace FytSoa.Core.ViewModel.Music
{
    public class NetApiLyricResult : NetApiResult
    {
        public NetLyricInfo Lrc { get; set; }

        public NetLyricInfo Klyric { get; set; }
    }
}
