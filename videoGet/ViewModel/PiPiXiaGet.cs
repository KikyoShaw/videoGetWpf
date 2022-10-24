using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace videoGet.ViewModel
{
    public class PiPiXiaGet
    {
        private static readonly Lazy<PiPiXiaGet>
            Lazy = new Lazy<PiPiXiaGet>(() => new PiPiXiaGet());

        public static PiPiXiaGet Instance => Lazy.Value;

        public PiPiXiaGet()
        {

        }

        JObject _json;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            //GetHtml(url);
            var longUrl = GetLong(url);
            var tempId = GetMp4Id(longUrl);
            FormatJson(tempId);
        }

        public void GetHtml(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            try
            {
                var beg = (HttpWebRequest)WebRequest.Create(url);
                beg.AllowAutoRedirect = false;
                beg.UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Moto G (4)) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36";
                var ret = (HttpWebResponse)beg.GetResponse();
                var read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                string text = read.ReadToEnd();
                var doc = new HtmlDocument();
                doc.LoadHtml(text);
                string json = doc.DocumentNode.SelectSingleNode("//body/script[1]").InnerText.Substring(30).Replace("; } catch (err) { console.error('[Vise] fail to read initState.'); }", "");
                _json = JObject.Parse(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WeiShiGet GetHtml is error: {ex}");
            }
        }

        //得到长链接
        private string GetLong(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";

            string longUrl = "";
            try
            {
                var beg = (HttpWebRequest)WebRequest.Create(url);
                beg.AllowAutoRedirect = false;
                using HttpWebResponse ret = (HttpWebResponse)beg.GetResponse();
                longUrl = ret.Headers["location"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PiPiXiaGet GetLong is error: {ex}");
            }
            return longUrl;
        }

        //得到视频ID
        private string GetMp4Id(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            try
            {
                return url.Substring(26, 19);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PiPiXiaGet GetMp4Id is error: {ex}");
                return "";
            }
        }

        //得到JSON
        private void FormatJson(string mp4Id)
        {
            if (string.IsNullOrEmpty(mp4Id))
                return;
            try
            {
                var beg = (HttpWebRequest)WebRequest.Create($"https://ib-hl.snssdk.com/bds/cell/detail/?version_code=4.2.7&app_name=super&device_id=1988391945639406&channel=App%20Store&resolution=1170*2532&aid=1319&last_channel=App%20Store&last_update_version_code=42691&recommend_disable=0&update_version_code=42780&ac=WIFI&os_version=15.5&device_platform=iphone&iid=805274026772804&device_type=iPhone%2012&cell_id={mp4Id}&cell_type=1&api_version=1");
                var ret = (HttpWebResponse)beg.GetResponse();
                var read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                string json = read.ReadToEnd();
                //序列化
                _json = JObject.Parse(json);
                read.Dispose();
                ret.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PiPiXiaGet FormatJson is error: {ex}");
            }
        }

        //得到发布时间
        [Obsolete("TimeZone")]
        public string GetCreateTime()
        {
            if (_json == null)
                return string.Empty;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddSeconds(Convert.ToInt64(_json["data"]?["data"]?["display_time"])).ToString("yyy-MM-dd hh:mm:ss");
        }

        //得到原视频
        public string GetMp4()
        {
            if (_json == null)
                return string.Empty;
            return _json["data"]?["data"]?["item"]?["video"]?["video_high"]?["url_list"]?[0]?["url"]?.ToString();
        }

        //视频标题
        public string GetTitle()
        {
            if (_json == null)
                return string.Empty;
            return _json["data"]?["data"]?["item"]?["content"]?.ToString();
        }

        //得到高清封面图 720
        public string GetHeightCover()
        {
            if (_json == null)
                return string.Empty;
            return "https://p9-ppx.byteimg.com/img/" + _json["data"]?["data"]?["item"]?["cover"]?["uri"]?.ToString() + "~1280x720.jpg";
        }

        //创作者名称
        public string GetAouthorName()
        {
            if (_json == null)
                return string.Empty;
            return _json["data"]?["data"]?["item"]?["author"]?["name"]?.ToString();
        }

        //作者头像 jpg类型
        public string GetAuthorHeadImage()
        {
            if (_json == null)
                return string.Empty;
            return "https://p3-ppx.byteimg.com/img/" + _json["data"]?["data"]?["item"]?["author"]?["avatar"]?["uri"]?.ToString() + "~tplv-ppx-avatar:200:200:q100.jpg";
        }
    }
}
