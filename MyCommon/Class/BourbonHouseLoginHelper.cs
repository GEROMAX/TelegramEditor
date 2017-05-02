using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyCommon
{
    public class BourbonHouseLoginHelper : WebServiceLoginHelperBase
    {
        public BourbonHouseLoginHelper(WebClient client) : base(client)
        {
        }

        public override void Login<LoginParam>(LoginParam param)
        {
            client.Encoding = Encoding.GetEncoding("UTF-8");
            //Basic認証のユーザー名とパスワード設定
            client.Credentials = new NetworkCredential(param.UserName, param.Password);
            //自己署名証明書を強制的に検証OKとするための検証メソッド追加
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(OnRemoteCertificateValidationCallback);
        }

        private bool OnRemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //チェックしないで常々OK
            return true;
        }
    }
}
