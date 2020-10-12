using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FytSoa.Common
{
    public class Utils
    {
        ///<summary>
        ///恢复html中的特殊字符
        ///</summary>
        ///<paramname="theString">需要恢复的文本。</param>
        ///<returns>恢复好的文本。</returns>
        public static string HtmlDecode(string theString)
        {
            theString = theString.Replace("&gt;", ">");
            theString = theString.Replace("&lt;", "<");
            theString = theString.Replace("&nbsp;", " ");
            theString = theString.Replace("&quot;", "\"");
            theString = theString.Replace("&#39;", "\'");
            theString = theString.Replace("<br/>", "\n");
            return theString;
        }

        /// <summary>
        /// 字符串转数组
        /// </summary>
        /// <param name="str">如1,2,3,4,5</param>
        /// <returns></returns>
        public static List<string> StrToListString(string str)
        {
            var list = new List<string>();
            if (!str.Contains(","))
            {
                list.Add(str);
                return list;
            }
            var slist = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in slist)
            {
                list.Add(item);
            }
            return list;
        }

        public static string EncryptNeteaseId(string id)
        {
            if (id == null) { return string.Empty; }
            string god = "3go8&$8*3*3h0k(2)2";
            string[] magic = new string[god.Length];
            for (int i = 0; i < god.Length; i++)
            {
                magic[i] = god.Substring(i, 1);
            }

            string[] sid = new string[id.Length];
            for (int i = 0; i < id.Length; i++)
            {
                sid[i] = id.Substring(i, 1);
            }

            for (int i = 0; i < sid.Length; i++)
            {
                int key1 = new ASCIIEncoding().GetBytes(sid[i])[0];
                int key2 = new ASCIIEncoding().GetBytes(magic[i % magic.Length])[0];
                sid[i] = ((char)(key1 ^ key2)).ToString();
            }
            byte[] md5 = new MD5CryptoServiceProvider().ComputeHash(Encoding.Default.GetBytes(string.Join("", sid)));
            string result = Convert.ToBase64String(md5);

            result = result.Replace("+", "-");
            result = result.Replace("/", "_");
            return result;
        }
    }
}
