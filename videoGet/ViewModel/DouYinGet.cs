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
            return url.Substring(url.IndexOf("https://v.douyin.com", StringComparison.Ordinal), 29);
        }

        // 得到视频ID 需要传递短链接
        public string GetId(string url)
        {
            HttpWebRequest beg = (HttpWebRequest)WebRequest.Create(url);
            beg.AllowAutoRedirect = false;
            WebResponse response = beg.GetResponse();
            string str = response.Headers["Location"];
            response.Dispose();
            return str.Substring(str.IndexOf("video/", StringComparison.Ordinal) + 6, 19);
           
        }

        //得到JSON字符串
        private string GetJson(string id)
        {
            string data = string.Empty;
            HttpWebRequest beg = (HttpWebRequest)WebRequest.Create("https://www.iesdouyin.com/web/api/v2/aweme/iteminfo/?item_ids=" + id);
            using HttpWebResponse ret = (HttpWebResponse)beg.GetResponse();
            using StreamReader readData = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
            return readData.ReadToEnd();
        }

        //填充json
        private void FormatJson(string json)
        {

            JObject ob = JObject.Parse(json);
            JArray convert = (JArray)ob["item_list"];
            if (convert != null) 
                _json = JObject.Parse(convert[0].ToString());
        }

        //获取音频链接
        public string GetMuiscUrl()
        {
            //0音乐链接 1音乐标题
            return _json["music"]?["play_url"]?["uri"]?.ToString();
        }

        //获取视频标题
        public string GetTitle()
        {
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
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(Convert.ToInt64(_json["create_time"]));
            var dateTime = $"{startTime.Year}年{startTime.Month}月{startTime.Day}日 {startTime.Hour}:{startTime.Minute}:{startTime.Second}";
            //得到发布的时间
            return dateTime;
        }

        //获取源视频链接
        public string GetVideoUrl()
        {
            string url = _json["video"]?["play_addr"]?["uri"]?.ToString();
            return "https://aweme.snssdk.com/aweme/v1/play/?video_id=" + url;
        }

        //获取封面
        public string GetCover()
            //封面
        {
            return _json["video"]?["dynamic_cover"]?["url_list"]?[0]?.ToString();
        }

        //图片集处理
        public string[] GetImageUrl()
        {
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

            return _json["author"]?["nickname"]?.ToString();
        }

        //获取所有信息
        [Obsolete("TimeZone")]
        public string[] GetAll(string txt)
        {
            string[] data;
            txt = GetLowerUrl(txt);
            txt =  GetId(txt);
            txt = GetJson(txt);
            FormatJson(txt);
            //表示为视频
            if (txt.Contains("\"images\":null,")) 
            {
                data = new string[7];
                //0视频标题   1发布时间  2封面   3视频链接  4音乐链接  5作者名称
                data[0] = GetTitle();
                data[1] = GetCreateTime();
                data[2] = GetCover();
                data[3] = GetVideoUrl();
                data[4] = GetMuiscUrl();
                data[5] = GetHostName();
                data[6] = "mp4";
            }
            else
            {
                data = GetImageUrl();
            }
            return data;
        }
    }
}
