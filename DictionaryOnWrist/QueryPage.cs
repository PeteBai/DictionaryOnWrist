using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tizen;
using Tizen.Applications;
using Tizen.Network.Connection;
using Tizen.Wearable.CircularUI.Forms;
using Xamarin.Forms;

namespace DictionaryOnWrist
{
    class QueryPage : CirclePage
    {
        bool isDev = true;
        PopupEntry pe = new PopupEntry()
        {
            //Text=" ",
            IsSpellCheckEnabled = true,
            IsTextPredictionEnabled = true,
            HorizontalTextAlignment = TextAlignment.Center,
            Placeholder = "点我输入英语单词"
        };
        Button b = new Button() { ImageSource = new FileImageSource() { File = "Search.png" }, BackgroundColor = Color.Transparent };
        // The root page of your application
        public QueryPage()
        {
            searchBtnClicked += searchBtnClickedFunc;
            b.Clicked += searchBtnClicked;
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Orientation = StackOrientation.Vertical,
                Children = {
                    new Label(){Text="请输入要查询的单词", HorizontalTextAlignment=TextAlignment.Center, FontSize=8},
                    pe,
                    new Label(){Text="  "},
                    b,
                }
            };
        }
        private event EventHandler searchBtnClicked;
        private void searchBtnClickedFunc(Object sender, EventArgs e)
        {
            //Toast.DisplayIconText("查询中", "CloudSearch.png", 1000);
            try
            {
                Task<String> task = Get(pe.Text, "zh");
                //Navigation.PushModalAsync(new ResultPage("等等"));
                string taskRet = task.Result;
                JsonResponse res = resolveJson(taskRet);
                if (res.error_code != -1 || res.trans_result.Count <= 0)
                {
                    string err = res.error_code.ToString();
                    if(!isDev)
                    {
                        Navigation.PushModalAsync(new ResultPage("出现错误，请重试。"));
                    }
                    else
                    {
                        Navigation.PushModalAsync(new ResultPage(err));
                    }
                    return;
                }
                //判断用户输入的是什么语言
                if (String.Equals(res.from, "zh"))
                {
                    Task<String> task2 = Get(pe.Text, "en");
                    res = resolveJson(task2.Result);
                }
                    //res = resolveJson(Post(pe.Text, "en"));
                string disp = res.trans_result[0].dst;
                Navigation.PushModalAsync(new ResultPage(disp));
            }
            catch(Exception ex)
            {
                if(!isDev)
                {
                    Toast.DisplayIconText("出现了一些问题，请重试。", "Error.png");
                }
                else
                {
                    Toast.DisplayText(ex.Message);
                }
                return;
            }
            
            
        }
        public async Task<String> Get(string query, string lang)
        {
            // 原文
            string q = query;
            // 源语言
            string from = "auto";
            // 目标语言
            string to = lang;
            // 改成您的APP ID
            string appId = "20200906000560123";
            Random rd = new Random();
            string salt = rd.Next(100000).ToString();
            // 改成您的密钥
            string secretKey = "X3vp5mhO6pTTVZe7ePpD";
            string sign = EncryptString(appId + q + salt + secretKey);
            string url = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            ConnectionItem currentConnection = ConnectionManager.CurrentConnection;
            if (currentConnection.Type == ConnectionType.Ethernet)
            {
                string proxyAddr = ConnectionManager.GetProxy(AddressFamily.IPv4);
                WebProxy myproxy = new WebProxy(proxyAddr, true);
                request.Proxy = myproxy;
                //Toast.DisplayText("Proxy");
            }

            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            request.UserAgent = null;
            request.Timeout = 6000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
        public JsonResponse resolveJson(string str)
        {
            JsonResponse jr = JsonConvert.DeserializeObject<JsonResponse>(str);
            return jr;
        }
    }
}
