using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.IO;
using System.Runtime.Serialization;

namespace IDCodePrinter
{
    public class PostDataAPI
    {
        CookieContainer cookie = new CookieContainer();
        /// <summary>
        /// 推送数据
        /// </summary>
        public string HttpPost(string Url, string postDataStr)
        {
            string retString = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.CookieContainer = cookie;
                Stream myRequestStream = request.GetRequestStream();
                StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                myStreamWriter.Write(postDataStr);
                myStreamWriter.Close();

                HttpWebResponse response = null;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (WebException ex)
                {
                    response = (HttpWebResponse)ex.Response;
                    //LogR.Logger.Error(ex, "request.GetResponse");
                    //throw ex;
                }
                response.Cookies = cookie.GetCookies(response.ResponseUri);
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();
            }
            catch (Exception ex)
            {
                //LogR.Logger.Error(ex, "HttpPost");
                throw ex;
            }
            return retString;
        }

        public static string BPNum = "2";//1 2
        public static string BPType = "PHEV";//PHEV BEV
    }
}
