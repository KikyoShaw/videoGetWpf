using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

namespace videoGet.ViewModel
{
    public class KuaiShouGet
    {
        private static readonly Lazy<KuaiShouGet>
            Lazy = new Lazy<KuaiShouGet>(() => new KuaiShouGet());

        public static KuaiShouGet Instance => Lazy.Value;

        public KuaiShouGet()
        {

        }

        private JObject _json;
        private JObject _jsonComment;

        //构造函数 初始化json
        public void InitUrl(string url)
        {
            if(string.IsNullOrEmpty(url))
                return;
            string lower = GetLowerUrl(url);
            string furlong = GetLongUrl(lower);
            Dictionary<string, string> dic = SplitData(furlong);
            if(dic == null)
                return;
            GetJson(furlong, dic["fid"], dic["shareToken"], dic["shareObjectId"], dic["shareId"], dic["photoId"]);
        }

        //获取快手链接
        private string GetLowerUrl(string url)
        {
            return url.Substring(url.IndexOf("https://v.kuaishou.com", StringComparison.Ordinal), 29);
        }

        //获取长链接
        public string GetLongUrl(string url)
        {
            try
            {
                HttpWebRequest beg = (HttpWebRequest)WebRequest.Create(url);
                beg.AllowAutoRedirect = false;
                beg.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                beg.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36";
                WebResponse response = null;
                //try
                //{
                response = beg.GetResponse();
                //}
                //catch (WebException e) {
                //    //if (e.Message.Contains("302"))
                //        //response = e.Result;
                //}

                response.Dispose();
                return response.Headers["Location"];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KuaiShouGet GetLongUrl is error: {ex}");
            }
            return "";
        }

        //分解键值对
        private Dictionary<string, string> SplitData(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;
            try
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                url = url.Substring(url.IndexOf("photo", StringComparison.Ordinal) + 22);
                string[] array = url.Split('&');
                foreach (var t in array)
                {
                    string[] s = t.Split('=');
                    dic.Add(s[0], s[1]);
                }
                return dic;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KuaiShouGet SplitData is error: {ex}");
            }

            return null;
        }

        //得到Json
        public void GetJson(string sources, string fid, string shareToken, string shareObjectId, string shareId, string photoId)
        {
            try
            {
                string postData = $@"
                   {{""fid"":""{fid}"",""shareToken"":""{shareToken}"",  ""shareObjectId"":""{shareObjectId}"",
                   ""shareMethod"":""TOKEN"",
                  ""shareId"":""{shareId}"",
                    ""shareResourceType"":""PHOTO_OTHER"",
                 ""shareChannel"":""share_copylink"",
                       ""kpn"":""KUAISHOU"",
                        ""subBiz"":""BROWSE_SLIDE_PHOTO"",
                        ""env"":""SHARE_VIEWER_ENV_TX_TRICK"",
                      ""h5Domain"":""v.m.chenzhongtech.com"",
                        ""photoId"":""{photoId}"",
                      ""isLongVideo"":false}} ";
                /*
                 * 下面是固定值不需要改变
                 *
                 * shareMethod
                 * shareResourceType:
                 *  shareChannel
                 *  kpn
                 *  subBiz
                 *  env
                 *  h5Domain  //无参数
                 *  isLongVideo
                 */
                byte[] array = Encoding.UTF8.GetBytes(postData);
                HttpWebRequest beg = (HttpWebRequest)WebRequest.Create("https://m.gifshow.com/rest/wd/photo/info?kpn=KUAISHOU&captchaToken=");
                beg.Method = "POST";
                beg.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36";
                beg.Referer = sources;
                beg.Headers.Add("Cookie", "_did=web_518883321649BCF8; did=web_94a05b1178c34af3b97bc4d644d8c963");
                beg.ContentType = "application/json";
                beg.ContentLength = array.Length;
                Stream st = beg.GetRequestStream();
                st.Write(array, 0, array.Length);
                HttpWebResponse ret = (HttpWebResponse)beg.GetResponse();
                StreamReader read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);

                string url = read.ReadToEnd();
                st.Dispose();
                ret.Dispose();
                read.Dispose();
                _json = JObject.Parse(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KuaiShouGet GetJson is error: {ex}");
            }
        }

        //得到视频
        public string GetMp4()
        {
            if (_json == null)
                return string.Empty;
            return _json["photo"]?["mainMvUrls"]?[0]?["url"]?.ToString();
        }

        //得到背景音乐
        public string GetMp3()
        {
            if (_json == null)
                return string.Empty;
            return _json["photo"]?["music"]?["audioUrls"]?[0]?["url"]?.ToString();
        }

        //得到视频标题
        public string GetTitle() 
        {
            if (_json == null)
                return string.Empty; 
            return _json["shareInfo"]?["shareTitle"]?.ToString();
        }

        //得到作者名称
        public string GetAuthorName() 
        {
            if (_json == null)
                return string.Empty; return _json["photo"]?["userName"]?.ToString();
        }

        //得到作者头像
        public string GetHeadeImage() 
        {
            if (_json == null)
                return string.Empty; 
            return _json["photo"]?["headUrl"]?.ToString();
        }

        //得到发布时间
        public string GetReleaseTime()
        {
            if (_json == null)
                return string.Empty;
            DateTime dt = new DateTime(new DateTime(1970, 1, 1, 8, 0, 0).Ticks + Convert.ToInt64(_json["photo"]?["timestamp"]) * 10000);
            return $"{dt.Year}年{dt.Month}月{dt.Day}日 {dt.Hour}时:{dt.Minute}分{dt.Second}秒";
        }

        //得到账号ID
        public string GetUserID()
        {
            if (_json == null)
                return string.Empty;
            return _json["photo"]?["kwaiId"]?.ToString();
        }

        //得到视频封面
        public string GetCover()
        {
            if (_json == null)
                return string.Empty;
            return _json["photo"]?["webpCoverUrls"]?[0]?["url"]?.ToString();
        }

        //得到粉丝数量
        public string GetFanCount()
        {
            if (_json == null)
                return string.Empty;
            return _json["counts"]?["fanCount"]?.ToString();
        }

        //得到关注数量
        public string GetAttentionCount()
        {
            if (_json == null)
                return string.Empty;
            return _json["counts"]?["followCount"]?.ToString();
        }

        //得到作品数量
        public string GetVideoCount()
        {
            if (_json == null)
                return string.Empty;
            return _json["counts"]?["photoCount"]?.ToString();
        }
        
        public string[] FanList()
        {
            string[] list = { GetFanCount(), GetAttentionCount(), GetVideoCount() };
            //0粉丝数量  1关注数量  2作品数量
            return list;
        }

        //得到评论的JSON 需要获得评论信息  必须调用一下他
        public void Comment(string photoId)
        {
            try
            {
                byte[] array = Encoding.UTF8.GetBytes($"{{\"photoId\":\"{photoId}\",\"count\":1000}}");
                HttpWebRequest beg = (HttpWebRequest)WebRequest.Create("https://v.m.chenzhongtech.com/rest/wd/photo/comment/list");
                beg.Method = "POST";
                beg.ContentType = "application/json";
                beg.UserAgent = "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Mobile Safari/537.36";
                beg.Headers.Add("Cookie", "did=web_94a05b1178c34af3b97bc4d644d8c963; didv=1663392836000");
                beg.ContentLength = array.Length;
                beg.Referer = "https://v.m.chenzhongtech.com/fw/photo/3xf2fu9w2qbbc62?fid=1538398485&cc=share_copylink&followRefer=151&shareMethod=TOKEN&docId=9&kpn=KUAISHOU&subBiz=BROWSE_SLIDE_PHOTO&photoId=3xf2fu9w2qbbc62&shareId=17161594948599&shareToken=X-aeKODbI6NAE1zO&shareResourceType=PHOTO_OTHER&userId=3xeh9u683m77smw&shareType=1&et=1_i%2F2000048841256313987_h3104&shareMode=APP&originShareId=17161594948599&appType=21&shareObjectId=5211227800906946534&shareUrlOpened=0×tamp=1665131446191";
                Stream st = beg.GetRequestStream();
                st.Write(array, 0, array.Length);
                HttpWebResponse ret = (HttpWebResponse)beg.GetResponse();
                StreamReader read = new StreamReader(ret.GetResponseStream(), Encoding.UTF8);
                _jsonComment = JObject.Parse(read.ReadToEnd());
                st.Dispose();
                ret.Dispose();
                read.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"KuaiShouGet Comment is error: {ex}");
            }
        }

        //得到评论总数
        public int GetCommentCount()
        {
            if (_jsonComment == null)
                return 0;
            return Convert.ToInt32(_jsonComment["commentCount"]);
        }

        //部分快手评论
        public DataTable GetCommentTable()
        {
            if (_jsonComment == null)
                return null;

            JArray root = (JArray)_jsonComment["rootComments"];
            DataTable commentTable = new DataTable();

            commentTable.Columns.Add("headurl");//头像
            commentTable.Columns.Add("author_name");//名称
            commentTable.Columns.Add("content");//内容
            commentTable.Columns.Add("authorArea");//ip归属地
            commentTable.Columns.Add("time");//ip归属地

            if (root != null)
            {
                foreach (var t in root)
                {
                    commentTable.Rows.Add(t["headurl"], t["author_name"], t["content"], t["authorArea"], t["time"]);
                }
            }

            return commentTable;
        }
        
    }
}
