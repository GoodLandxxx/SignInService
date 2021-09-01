using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SignInService.Net
{
    public  class HttpTools
    {
          
        
        private readonly int _timeout ;
        private readonly string _userAgent ;
        public HttpTools()
        {
            var temp = int.Parse(ConfigurationManager.AppSettings["RequestTimeout"]); 
             _timeout = temp == 0 ? 5000: temp;
            _userAgent = ConfigurationManager.AppSettings["UserAgent"];
           
        }
        public  bool GetHttpResult(string url)
        {

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = _timeout  ;
            request.UserAgent = _userAgent;
            try
            {
                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return false;

        }

    }
}
