﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using PokerTexas.App_Common;

namespace PokerTexas.App_UserControl
{
    public class WebClientEx : WebClient
    {
        public enum RequestTypeEnum
        {
            Poker,
            PokerWeb,
            FaceBook,
            Nomal
        }
        #region - DECLARE -
        public Exception Error = null;
        public CookieContainer CookieContainer = new CookieContainer();
        public bool AllowAutoRedirect = true;
        public WebRequest Request { get; set; }
        public WebResponse Response { get; set; }
        public string ResponseText { get; set; }
        public Image ResponseImage { get; set; }
        public string X_TUNNEL_VERIFY = string.Empty;
        public string Authorization = string.Empty;
        public RequestTypeEnum RequestType = RequestTypeEnum.Nomal;
        public bool SetAPIV8 = false;
        public bool SetCookieV2 = false;
        public string IpHeader = string.Empty;
        #endregion

        #region - METHOD -
        public WebClientEx()
        {
            this.Encoding = Encoding.UTF8;
        }
        protected override WebRequest GetWebRequest(Uri address)
        {
            try
            {
                Request = base.GetWebRequest(address);
                if (Request is HttpWebRequest)
                {
                    (Request as HttpWebRequest).CookieContainer = CookieContainer;
                    (Request as HttpWebRequest).AllowAutoRedirect = AllowAutoRedirect;
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return Request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            try
            {
                Response = base.GetWebResponse(request);
            }
            catch (WebException ex)
            {
                this.Error = ex;
                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        ResponseText = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return Response;
        }

        public string DoPost(NameValueCollection parameters, string strURL, Dictionary<HttpRequestHeader, string> additionHeader)
        {
            try
            {
                Error = null;
                ResponseText = null;
                addHeader(additionHeader);
                ResponseText = Encoding.Default.GetString(this.UploadValues(strURL, parameters));
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return ResponseText;
        }

        public string DoPost(NameValueCollection parameters, string strURL)
        {
            return this.DoPost(parameters, strURL, null);
        }

        public string DoPost(string parameters, string strURL, Dictionary<HttpRequestHeader, string> additionHeader, string Method)
        {
            try
            {
                Error = null;
                ResponseText = null;
                if (additionHeader.ContainsKey(HttpRequestHeader.ContentType))
                {
                    additionHeader.Remove(HttpRequestHeader.ContentType);
                }
                additionHeader.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
                addHeader(additionHeader);
                ResponseText = this.UploadString(strURL, Method, parameters);
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return ResponseText;
        }

        public string DoPost(string parameters, string strURL)
        {
            Dictionary<HttpRequestHeader, string> additionHeader = new Dictionary<HttpRequestHeader, string>();
            additionHeader.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            return this.DoPost(parameters, strURL, additionHeader, "POST");
        }

        public string DoPost(byte[] parameters, string strURL, Dictionary<HttpRequestHeader, string> additionHeader)
        {
            try
            {
                Error = null;
                ResponseText = null;
                addHeader(additionHeader);
                ResponseText = ResponseText = Encoding.Default.GetString(this.UploadData(strURL, parameters));
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return ResponseText;
        }

        public string DoGet(string strURL, Dictionary<HttpRequestHeader, string> additionHeader)
        {
            try
            {
                Error = null;
                ResponseText = string.Empty;
                addHeader(additionHeader);
                ResponseText = this.DownloadString(strURL);
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return ResponseText;
        }

        public string DoGet(string strURL)
        {
            return this.DoGet(strURL, null);
        }

        public Image GetImage(string strURL)
        {
            try
            {
                Error = null;
                ResponseText = string.Empty;
                ResponseImage = null;
                addHeader(null);
                ResponseText = this.DownloadString(strURL);
                byte[] byteArrayIn = this.DownloadData(strURL);
                MemoryStream ms = new MemoryStream(byteArrayIn);
                ResponseImage = Image.FromStream(ms);
            }
            catch (Exception ex)
            {
                this.Error = ex;
            }
            return ResponseImage;
        }

        private void addHeader(Dictionary<HttpRequestHeader, string> additionHeader)
        {
            this.Headers.Clear();

            if (RequestType == RequestTypeEnum.Poker)
            {
                if (!string.IsNullOrEmpty(X_TUNNEL_VERIFY))
                {
                    this.Headers.Add("X-TUNNEL-VERIFY", X_TUNNEL_VERIFY);
                }

                this.Headers.Add("X-API-VER", "8");
                this.Headers.Add("x-param", "2#0");
            }
            else if (RequestType == RequestTypeEnum.FaceBook)
            {
                if (!string.IsNullOrEmpty(Authorization))
                {
                    this.Headers.Add("Authorization", "OAuth " + Authorization);
                }
                this.Headers.Add(HttpRequestHeader.UserAgent, AppSettings.UserAgentFaceBook);
            }
            else
            {
                this.Headers.Add(HttpRequestHeader.UserAgent, AppSettings.UserAgentBrowser);
                //this.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
                this.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                this.Headers.Add(HttpRequestHeader.AcceptLanguage, "vi-VN,vi;q=0.8,fr-FR;q=0.6,fr;q=0.4,en-US;q=0.2,en;q=0.2");
            }
            if (RequestType == RequestTypeEnum.PokerWeb)
            {
                this.Headers.Add("X-Requested-With", "XMLHttpRequest");
                this.Headers.Remove(HttpRequestHeader.Accept);
                this.Headers.Add(HttpRequestHeader.Accept, "application/json, text/javascript, */*");
            }
            if (SetCookieV2)
            {
                this.Headers.Add("Cookie2", "$Version=1");
            }
            if (!string.IsNullOrEmpty(IpHeader))
            {
                this.Headers.Add("CLIENT-IP", IpHeader);
                this.Headers.Add("X-FORWARDED-FOR", IpHeader);
                this.Headers.Add("X-FORWARDED", IpHeader);
                this.Headers.Add("X-CLUSTER-CLIENT-IP", IpHeader);
                this.Headers.Add("FORWARDED-FOR", IpHeader);
                this.Headers.Add("FORWARDED", IpHeader);
                this.Headers.Add("VIA", IpHeader);
            }
            if (additionHeader != null)
            {
                foreach (KeyValuePair<HttpRequestHeader, string> item in additionHeader)
                {
                    this.Headers.Add(item.Key, item.Value);
                }
            }
        }
        #endregion
    }
}