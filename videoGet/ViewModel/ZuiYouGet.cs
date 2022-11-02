using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace videoGet.ViewModel
{
    public class ZuiYouGet
    {
        private static readonly Lazy<ZuiYouGet>
            Lazy = new Lazy<ZuiYouGet>(() => new ZuiYouGet());

        public static ZuiYouGet Instance => Lazy.Value;

        public ZuiYouGet()
        {

        }

        private JObject _json;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            var tempId = GetId(url);
            FormatJson(tempId);
        }

        //获取视频id
        private string GetId(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            try
            {
                return url.Substring(url.IndexOf("pid=", StringComparison.Ordinal) + 4, 9);
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }

            return "";
        }

        //获取json
        private void FormatJson(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;
            try
            {
                var beg = WebRequest.Create("https://share.xiaochuankeji.cn/planck/share/post/detail");
                beg.Method = "POST";
                var array = Encoding.UTF8.GetBytes("{\"pid\":" + id + "}");
                beg.ContentLength = array.Length;
                var st = beg.GetRequestStream();
                st.Write(array, 0, array.Length);
                var ret = beg.GetResponse();
                var read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                var json = read.ReadToEnd();
                _json = JObject.Parse(json);
                //解析JSON
                st.Dispose();
                ret.Dispose();
                read.Dispose();
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine($"PiPiXiaGet FormatJson is error: {ex}");
            }
        }

        //作者名
        public string GetAuthorName()
        {
            if (_json == null)
                return string.Empty;

            return _json["data"]?["post"]?["member"]?["name"]?.ToString();
        }

        //视频标题
        public string GetTitle()
        {
            if (_json == null)
                return string.Empty;

            return _json["data"]?["post"]?["content"]?.ToString();
        }


        public string GetMp4()
        {
            if (_json == null)
                return string.Empty;

            string mp4Id = _json["data"]?["post"]?["imgs"]?[0]?["id"]?.ToString();

            if (string.IsNullOrEmpty(mp4Id))
                return string.Empty;

            return _json["data"]?["post"]?["videos"]?[mp4Id]?["url"]?.ToString();
        }//得到原视频


        public string GetCover(string size)
        {
            if (_json == null)
                return string.Empty;

            string mp4Id = _json["data"]?["post"]?["imgs"]?[0]?["id"]?.ToString();

            if (string.IsNullOrEmpty(mp4Id))
                return string.Empty;

            return $"http://tbfile.izuiyou.com/img/frame/id/{mp4Id}?w={size}&delogo=0";
        }
    }
}
