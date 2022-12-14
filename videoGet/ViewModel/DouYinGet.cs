using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace videoGet.ViewModel
{
    public class DouYinGet
    {
        private static readonly Lazy<DouYinGet>
            Lazy = new Lazy<DouYinGet>(() => new DouYinGet());

        public static DouYinGet Instance => Lazy.Value;

        private JObject _json;

        public DouYinGet()
        {

        }

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            var tempUrl = GetLowerUrl(url);
            var tempId = GetId(tempUrl);
            var tempJson = GetJson(tempId);
            FormatJson(tempJson);
        }

        //获取抖音链接
        private string GetLowerUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return "";
            try
            {
                if (!Tools.Helper.ContainChinese(url))
                    return url;
                //需要用户登录分享链接才生效
                return url.Substring(url.IndexOf("https://v.douyin.com", StringComparison.Ordinal), 29);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DouYinGet GetLowerUrl is error: {ex}");
            }

            return "";
        }

        // 得到视频ID 需要传递短链接
        public string GetId(string url)
        {
            if (string.IsNullOrEmpty(url)) 
                return "";
            try
            {
                HttpWebRequest beg = (HttpWebRequest)WebRequest.Create(url);
                beg.AllowAutoRedirect = false;
                WebResponse response = beg.GetResponse();
                string str = response.Headers["Location"];
                response.Dispose();
                return str.Substring(str.IndexOf("video/", StringComparison.Ordinal) + 6, 19);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DouYinGet GetId is error: {ex}");
            }

            return "";
        }

        //得到JSON字符串
        private string GetJson(string id)
        {
            if (string.IsNullOrEmpty(id))
                return "";
            try
            {
                string data = string.Empty;
                HttpWebRequest beg = (HttpWebRequest)WebRequest.Create("https://www.iesdouyin.com/web/api/v2/aweme/iteminfo/?item_ids=" + id);
                using HttpWebResponse ret = (HttpWebResponse)beg.GetResponse();
                using StreamReader readData = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                return readData.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DouYinGet GetJson is error: {ex}");
            }

            return "";
        }

        //填充json
        private void FormatJson(string json)
        {
            if(string.IsNullOrEmpty(json))
                return;
            try
            {
                JObject ob = JObject.Parse(json);
                JArray convert = (JArray)ob["item_list"];
                if (convert != null)
                    _json = JObject.Parse(convert[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DouYinGet FormatJson is error: {ex}");
            }
        }

        //获取音频链接
        public string GetMuiscUrl()
        {
            if (_json == null)
                return string.Empty;
            //0音乐链接 1音乐标题
            return _json["music"]?["play_url"]?["uri"]?.ToString();
        }

        //获取视频标题
        public string GetTitle()
        {
            if (_json == null)
                return string.Empty;
            string title = _json["desc"]?.ToString();
            if (title != null)
            {
                title = title.Replace("#", "@");
                return title;
            }
            return "";
        }

        //获取发布时间
        [Obsolete("TimeZone")]
        public string GetCreateTime()
        {
            if (_json == null)
                return string.Empty;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(Convert.ToInt64(_json["create_time"]));
            var dateTime = $"{startTime.Year}年{startTime.Month}月{startTime.Day}日 {startTime.Hour}:{startTime.Minute}:{startTime.Second}";
            //得到发布的时间
            return dateTime;
        }

        //获取源视频链接
        public string GetVideoUrl()
        {
            if (_json == null)
                return string.Empty;
            string url = _json["video"]?["play_addr"]?["uri"]?.ToString();
            return "https://aweme.snssdk.com/aweme/v1/play/?video_id=" + url;
        }

        //获取封面
        public string GetCover()
        {
            if (_json == null)
                return string.Empty;
            return _json["video"]?["dynamic_cover"]?["url_list"]?[0]?.ToString();
        }

        //图片集处理
        public string[] GetImageUrl()
        {
            if (_json == null)
                return null;
            JArray imgUrl = (JArray)_json["images"];
            if (imgUrl != null)
            {
                string[] image = new string[imgUrl.Count];

                for (int i = 0; i < imgUrl.Count; i++)
                {
                    JArray url = (JArray)imgUrl[i]["url_list"];
                    if (url != null) 
                        image[i] = url[3].ToString();
                }
                return image;
            }
            return null;
        }

        //获取作者名字
        public string GetHostName()
        {
            if (_json == null)
                return string.Empty;
            return _json["author"]?["nickname"]?.ToString();
        }
    }
}
