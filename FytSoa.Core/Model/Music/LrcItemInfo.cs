using System;
using System.Collections.Generic;
using System.Text;

namespace FytSoa.Core.Model.Music
{
    public class LrcItemInfo
    {
        public double Time { get; set; }

        public string Text { get; set; }

        public LrcItemInfo() { }

        public LrcItemInfo(double time, string text)
        {
            Time = time;
            Text = text;
        }
    }
}
