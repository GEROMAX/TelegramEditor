using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCommon
{
    public class YahooFinanceLoginParam : LoginParam
    {
        public YahooFinanceLoginParam() : base()
        {
        }

        public string StartUrl { get; set; }
        public string LoginUrl { get; set; }
    }

    public class YahooFinanceLoginHelper : WebServiceLoginHelperBase
    {
        public YahooFinanceLoginHelper(WebClient client) : base(client)
        {
        }

        public override void Login<YahooFinanceLoginParam>(YahooFinanceLoginParam param)
        {
            //ヤフーのログインは一筋縄ではいかない。。。
            client.Encoding = Encoding.GetEncoding("UTF-8");
            client.Proxy = new WebProxy("http://172.22.0.100:12800");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E; Tablet PC 2.0)");
            client.DownloadString(param["StartUrl"]);
            System.Threading.Thread.Sleep(1000);
            var firstSource = client.DownloadString(param["LoginUrl"]);
            //var loginUrl = Regex.Match(firstSource, "action=\"(?<action>[^\"]+)").Groups["action"];

            var nameValues = Regex.Matches(firstSource, "<input type=\"hidden\" name=\"(?<name>[^\"]+)\" value=\"(?<value>[^\"]+)\">");
            var loginInfo = new NameValueCollection();
            foreach (Match item in nameValues)
            {
                loginInfo.Add(item.Groups["name"].Value, item.Groups["value"].Value);
            }
            loginInfo.Add("login", param.UserName);
            loginInfo.Add("passwd", param.Password);
            loginInfo.Add(".persistent", "y");
            System.Threading.Thread.Sleep(1000);
            var ret = Encoding.GetEncoding("UTF-8").GetString(client.UploadValues(param["LoginUrl"], loginInfo));
            System.Threading.Thread.Sleep(1000);
        }
    }
}
