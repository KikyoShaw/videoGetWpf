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
    public class PipiGet
    {
        private static readonly Lazy<PipiGet>
            Lazy = new Lazy<PipiGet>(() => new PipiGet());

        public static PipiGet Instance => Lazy.Value;

        public PipiGet()
        {

        }

        private JObject _json = null;
        private string Id { get; set; } = "";

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            var tempId = GetId(url);
            FormatJson(tempId);
            //初始化id
            GetCoverId();
        }

        //获取视频id
        private string GetId(string str)
        {
            if(string.IsNullOrEmpty(str))
                return "";
            try
            {
                return str.Substring(30, 12);
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
                var array = Encoding.UTF8.GetBytes("{\"pid\":" + id + ",\"type\":\"post\"}");
                var beg = WebRequest.Create("https://h5.pipigx.com/ppapi/share/fetch_content");
                beg.Method = "POST";
                beg.ContentLength = array.Length;
                var st = beg.GetRequestStream();
                st.Write(array, 0, array.Length);
                var ret = beg.GetResponse();
                var str = new StreamReader(ret.GetResponseStream()).ReadToEnd();

                _json = JObject.Parse(str);
                ret.Dispose();
            }
            catch /*(Exception ex)*/
            {
                //Console.WriteLine($"PiPiXiaGet FormatJson is error: {ex}");
            }
        }

        //获取视频
        public string GetMp4()
        {
            if (_json == null)
                return "";
            try
            {
                return _json["data"]?["post"]?["videos"]?[Id]?["url"]?.ToString();
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }

            return "";
        }

        //获取封面Id
        public void GetCoverId()
        {
            if (_json == null)
                return;

            try
            {
                Id = _json["data"]?["post"]?["videos"]?.ToString().Substring(6, 10);
            }
            catch /*(Exception e)*/
            {
                //Console.WriteLine(e);
                //throw;
            }
        }

        //视频标题
        public string GetTitle()
        {
            if (_json == null)
                return "";
            return _json["data"]?["post"]?["content"]?.ToString();
        }

    }
}
