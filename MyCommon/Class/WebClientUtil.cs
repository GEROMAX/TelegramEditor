using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Reflection;

namespace MyCommon
{
    /// <summary>
    /// 拡張WebClientクラス
    /// </summary>
    public class UnivarsalWebClient : WebClient
    {
        private CookieContainer cookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                //CookieContainerを追加
                ((HttpWebRequest)request).CookieContainer = this.cookieContainer;
            }
            return request;
        }
    }

    public class LoginParam
    {
        public LoginParam()
        {
        }

        public string UserName { get; set; }
        public string Password { get; set; }
        public string this[string propertyName]
        {
            get
            {
                var pi = this.getPropertyInfo(propertyName);
                if (null == pi)
                {
                    return string.Empty;
                }

                return (string)pi.GetValue(this);
            }
            set
            {
                var pi = this.getPropertyInfo(propertyName);
                if (null == pi)
                {
                    return;
                }

                pi.SetValue(this, value);
            }
        }
        private PropertyInfo getPropertyInfo(string propertyName)
        {
            return this.GetType().GetProperty(propertyName);
        }
    }

    public abstract class WebServiceLoginHelperBase
    {
        public WebClient client { get; private set; }

        public WebServiceLoginHelperBase(WebClient client)
        {
            this.client = client;
        }

        public abstract void Login<T>(T param) where T : LoginParam, new();
    }
}
