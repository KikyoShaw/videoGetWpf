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

        HtmlDocument Doc;
        JObject Json;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            //var tempUrl = GetUrl(url);
            GetHtml(url);
            FromatJson();
        }

        //截取链接,得到短链接
        private string GetUrl(string txt)
        {
            return txt.Substring(txt.IndexOf(">>", StringComparison.Ordinal) + 2);
        }

        //得到html
        public void GetHtml(string url)
        {
            var beg = (HttpWebRequest)WebRequest.Create(url);
            beg.AllowAutoRedirect = false;
            beg.UserAgent = "Mozilla/5.0 (Linux; Android 6.0.1; Moto G (4)) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36";
            var ret = (HttpWebResponse)beg.GetResponse();
            var read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
            string text = read.ReadToEnd();
            Doc = new HtmlDocument();
            Doc.LoadHtml(text);
        }

        //初始化JSON
        public void FromatJson()
        {
            string json = Doc.DocumentNode.SelectSingleNode("//body/script[1]").InnerText.Substring(30).Replace("; } catch (err) { console.error('[Vise] fail to read initState.'); }", "");
            Json = JObject.Parse(json);
        }

        //发布时间
        [Obsolete("TimeZone")]
        public string GetCreateTime()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return startTime.AddSeconds(Convert.ToInt64(Json["feedsList"]?[0]?["poster"]?["createtime"])).ToString("yyy-MM-dd hh:mm:ss");
        }

        //作者名字
        public string GetAuthorName()
        {
            return Json["feedsList"]?[0]?["poster"]?["nick"]?.ToString();
        }

        //作者头像
        public string GetAuthorHeadImage()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["poster"]?["avatar"]?.ToString() ?? string.Empty);
        }

        //高质量封面 1080P
        public string GetHeightCover()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["images"]?[0]?["url"]?.ToString() ?? string.Empty);
        }

        //低质量封面 200
        public string GetLowerCover()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["images"]?[1]?["url"]?.ToString() ?? string.Empty);
        }

        //点赞数量
        public string GetLinkCount()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["dingCount"]?.ToString() ?? string.Empty);
        }

        //评论数量
        public string GetCommentCount()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["totalCommentNum"]?.ToString() ?? string.Empty);
        }

        //播放数量
        public string GetPlayNum()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["playNum"]?.ToString() ?? string.Empty);
        }

        //源视频
        public string GetMp4()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["videoUrl"]?.ToString() ?? string.Empty);
        }

        //视频标题
        public string GetTitle()
        {
            return Regex.Unescape(Json["feedsList"]?[0]?["shareInfo"]?["bodyMap"]?["0"]?["title"]?.ToString() ?? string.Empty);
        }

    }
}
