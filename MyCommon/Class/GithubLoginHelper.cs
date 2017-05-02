using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyCommon
{
    public class GithubLoginHelper : WebServiceLoginHelperBase
    {
        public GithubLoginHelper(WebClient client) : base(client)
        {
        }

        public override void Login<LoginParam>(LoginParam loginParam = null)
        {
            //GitHubはユーザーエージェントが設定されていないとエラーが帰ってくる
            this.client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.2; .NET4.0C; .NET4.0E; Tablet PC 2.0)");
        }
    }
}
