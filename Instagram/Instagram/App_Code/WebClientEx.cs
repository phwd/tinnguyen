﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Configuration;

public class WebClientEx : WebClient
{
    #region - DECLARE -
    public Exception Error = null;
    public CookieContainer CookieContainer = new CookieContainer();
    public bool AllowAutoRedirect = true;
    public WebRequest Request { get; set; }
    public WebResponse Response { get; set; }
    public string ResponseText { get; set; }
    public Image ResponseImage { get; set; }
    public string UserAgent = "Instagram 6.5.0 Android (17/4.2.2; 240dpi; 480x800; samsung; SGH-T959; SGH-T959; aries; en_US)";
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
        return this.DoPost(parameters, strURL, null, "POST");
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

    private void addHeader(Dictionary<HttpRequestHeader, string> additionHeader)
    {
        this.Headers.Clear();
        this.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip");
        this.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US");
        this.Headers.Add(HttpRequestHeader.UserAgent, this.UserAgent);
        //this.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded; charset=UTF-8");
        this.Headers.Add("X-IG-Connection-Type", "WIFI");
        this.Headers.Add("Cookie2", "$Version=1");
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