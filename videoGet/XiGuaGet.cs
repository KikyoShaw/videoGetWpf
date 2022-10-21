using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace videoGet
{
    public class XiGuaGet
    {
        private static readonly Lazy<XiGuaGet>
            Lazy = new Lazy<XiGuaGet>(() => new XiGuaGet());

        public static XiGuaGet Instance => Lazy.Value;

        public XiGuaGet()
        {

        }

        private HtmlDocument _doc;
        private JObject _json;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            if (url.Trim().Length == 29) GetHtml(ref url);
            //得到长链接
            GetHtml(ref url);
            FormatJson(url);
        }

        //得到html
        private void GetHtml(ref string url)
        {
            HttpWebRequest beg = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse ret;
            beg.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            beg.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36";
            beg.Headers.Add("Cookie", "ixigua-a-s=1; support_webp=true; support_avif=true; __ac_nonce=063454b6b00d4f78b8c1d; __ac_signature=_02B4Z6wo00f01f4IbSQAAIDCNrSkOsNVka3-KGmAABzJw2Jh8-UbwMpJADtyfKsBlG8uSNRlG8WRe0WJiBnnELkHGl2E80Qf.kjxNadHrUn3v1W-GR26riqXDqj.K.pu2jq57uPdJk8Z3oyK9c; _tea_utm_cache_1300=undefined; MONITOR_WEB_ID=7124914227890439694; ttwid=1%7CszSdFxHrf6xg5cThiKwJhsMNpw4Kpimnyepj1FIscYc%7C1665485824%7C1d015107dbdf704a70f6fc74722077d67f482d0b2d594a0652d360fe81baa93d; msToken=9JMXTpKWpn1A4K3t7lk7zDJXp1wxyaYuTlpCHyBvk1Z8o_TEQ7Sx-cOiniYgCyWBGGyghf97aS1kZncgNsE_mITorQuoHn8o4-vsNbmGF_m5ZSXtUvrUFalMzFqFs94EZA==; tt_scid=0csR7YE-y3ussN5mJn0gntW9jOIrdQODNrv1YmNbOgtAtRZNjp1xD8xrTC.Jn.Qa8e09");
            if (url.Trim().Length == 29)
            {
                beg.AllowAutoRedirect = false;
                ret = (HttpWebResponse)beg.GetResponse();
                url = ret.Headers["location"];
            }
            else
            {
                ret = (HttpWebResponse)beg.GetResponse();
                StreamReader read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                url = read.ReadToEnd();
                read.Dispose();
            }
            ret.Dispose();
        }

        private void FormatHtml(string str)
        {
            _doc = new HtmlDocument();
            _doc.LoadHtml(str);
        }

        //初始html
        private void FormatJson(string str)
        {
            _doc = new HtmlDocument();
            _doc.LoadHtml(str);
            str = _doc.DocumentNode.SelectSingleNode("//body/script[@id='SSR_HYDRATED_DATA']").InnerText.Replace("window._SSR_HYDRATED_DATA=", "");
            _json = JObject.Parse(str);
        }

        //360分辨率
        private string Get360Mp4()
        {
            return FormatBase64(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["videoResource"]?["normal"]?["video_list"]?["video_1"]?["main_url"]?.ToString());
        }

        //480分辨率
        public string Get480Mp4()
        {
            try
            {
                return FormatBase64(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["videoResource"]?["normal"]?["video_list"]?["video_2"]?["main_url"]?.ToString());
            }
            catch
            {
                return string.Empty;
            }

        }

        //720分辨率
        public string Get720Mp4()
        {
            try
            {
                return FormatBase64(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["videoResource"]?["normal"]?["video_list"]?["video_3"]?["main_url"]?.ToString());
            }
            catch
            {
                return string.Empty;
            }
        }

        //1080分辨率
        public string Get1080Mp4()
        {
            try
            {
                return FormatBase64(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["videoResource"]?["normal"]?["video_list"]?["video_4"]?["main_url"]?.ToString());
            }
            catch
            {
                return string.Empty;
            }
            
        }

        //4K
        public string Get2160()
        {
            try
            {
                return FormatBase64(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["videoResource"]?["normal"]?["video_list"]?["video_5"]?["main_url"]?.ToString());
            }
            catch
            {
                return string.Empty;
            }
        }
        public string GetHeightMp4()
        {

            if (Get2160() != string.Empty) return Get2160(); //4k
            if (Get1080Mp4() != string.Empty) return Get1080Mp4();//1080
            if (Get720Mp4() != string.Empty) return Get720Mp4(); //720
            if (Get480Mp4() != string.Empty) return Get480Mp4(); //480

            //得到分辨率最高的视频
            return Get360Mp4();
        }

        //作者头像
        public string GetAouthorHeadImage()
        {
            return Regex.Unescape(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["user_info"]?["avatar_url"]?.ToString() ?? string.Empty);
        }

        //作者名称
        public string GetAouthorName()
        {
            return _json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["user_info"]?["name"]?.ToString();
        }

        //作品标题
        public string GetTitle()
        {
            return _json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["title"]?.ToString();
        }

        //作品封面
        public string GetCover()
        {
            return Regex.Unescape(_json["anyVideo"]?["gidInformation"]?["packerData"]?["video"]?["poster_url"]?.ToString() ?? string.Empty);
        }

        //Base64解密 获得视频的方法都需要调用他
        public string FormatBase64(string str)
        {
            byte[] array = Convert.FromBase64String(str);
            return str = Encoding.UTF8.GetString(array);
        }


    }

}
