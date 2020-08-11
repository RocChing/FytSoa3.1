using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.ViewModel.Music
{
    public class NetApiResult
    {
        public int Code { get; set; }

        public bool Success { get { return Code == 200; } }
    }
}
