using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace videoGet.ViewModel
{
    public class WeiShiGet
    {
        private static readonly Lazy<WeiShiGet>
            Lazy = new Lazy<WeiShiGet>(() => new WeiShiGet());

        public static WeiShiGet Instance => Lazy.Value;

        public WeiShiGet()
        {

        }

        JObject _json;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            //var tempUrl = GetUrl(url);
            GetHtml(url);
        }

        //截取链接,得到短链接
        private string GetUrl(string txt)
        {
            return txt.Substring(txt.IndexOf(">>", StringComparison.Ordinal) + 2);
        }

        //初始化json
        public void GetHtml(string url)
        {
            if(string.IsNullOrEmpty(url))
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

        //发布时间
        [Obsolete("TimeZone")]
        public string GetCreateTime()
        {
            if (_json == null)
                return string.Empty;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddSeconds(Convert.ToInt64(_json["feedsList"]?[0]?["poster"]?["createtime"])).ToString("yyy-MM-dd hh:mm:ss");
        }

        //作者名字
        public string GetAuthorName()
        {
            if (_json == null)
                return string.Empty;
            return _json["feedsList"]?[0]?["poster"]?["nick"]?.ToString();
        }

        //作者头像
        public string GetAuthorHeadImage()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["poster"]?["avatar"]?.ToString() ?? string.Empty);
        }

        //高质量封面 1080P
        public string GetHeightCover()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["images"]?[0]?["url"]?.ToString() ?? string.Empty);
        }

        //低质量封面 200
        public string GetLowerCover()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["images"]?[1]?["url"]?.ToString() ?? string.Empty);
        }

        //点赞数量
        public string GetLinkCount()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["dingCount"]?.ToString() ?? string.Empty);
        }

        //评论数量
        public string GetCommentCount()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["totalCommentNum"]?.ToString() ?? string.Empty);
        }

        //播放数量
        public string GetPlayNum()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["playNum"]?.ToString() ?? string.Empty);
        }

        //源视频
        public string GetMp4()
        {
            if (_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["videoUrl"]?.ToString() ?? string.Empty);
        }

        //视频标题
        public string GetTitle()
        {
            if(_json == null)
                return string.Empty;
            return Regex.Unescape(_json["feedsList"]?[0]?["shareInfo"]?["bodyMap"]?["0"]?["title"]?.ToString() ?? string.Empty);
        }

    }
}
