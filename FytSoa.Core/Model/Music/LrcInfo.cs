using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FytSoa.Core.Model.Music
{
    public class LrcInfo
    {
        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; }

        public double Total { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        public List<LrcItemInfo> Words { get; set; }

        public static LrcInfo Parse(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                LrcInfo lrc = new LrcInfo();
                List<LrcItemInfo> items = new List<LrcItemInfo>();
                string[] lines = content.Split(new char[] { '\r', '\n' });
                foreach (var line in lines)
                {
                    if (line.StartsWith("[ti:"))
                    {
                        lrc.Title = SplitInfo(line);
                    }
                    else if (line.StartsWith("[ar:"))
                    {
                        lrc.Artist = SplitInfo(line);
                    }
                    else if (line.StartsWith("[al:"))
                    {
                        lrc.Album = SplitInfo(line);
                    }
                    else if (line.StartsWith("[by:"))
                    {
                        lrc.LrcBy = SplitInfo(line);
                    }
                    else if (line.StartsWith("[offset:"))
                    {
                        lrc.Offset = SplitInfo(line);
                    }
                    else if (line.StartsWith("[total:"))
                    {
                        double t = 0;
                        string total = SplitInfo(line);
                        double.TryParse(total, out t);
                        lrc.Total = t;
                    }
                    else
                    {
                        try
                        {
                            Regex regexword = new Regex(@".*\](.*)");
                            Match mcw = regexword.Match(line);
                            string word = mcw.Groups[1].Value;
                            Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                            MatchCollection mct = regextime.Matches(line);
                            foreach (Match item in mct)
                            {
                                double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                                items.Add(new LrcItemInfo(time, word));
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
                lrc.Words = items.OrderBy(m => m.Time).ToList();
                return lrc;
            }
            catch (Exception e)
            {
                FytSoa.Common.Logger.Default.Error(e.Message, e);
                return null;
            }
        }

        /// <summary>
        /// 处理信息(私有方法)
        /// </summary>
        /// <param name="line"></param>
        /// <returns>返回基础信息</returns>
        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }
    }
}
