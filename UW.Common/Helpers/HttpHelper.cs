using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http.Formatting;
using System.Web.Script.Serialization;
using System.Web;

namespace UW.Common
{
    public static class HttpHelper
    {
        public static string Get(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.Timeout = 10000;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        public static string Post(string url, string data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Timeout = 10000;
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            request.ContentLength = byteData.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(byteData, 0, byteData.Length);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        public static string Post<T>(string url, T data)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsJsonAsync(url, data).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static string Post<T>(string url, T data, int timeoutInSeconds)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            HttpResponseMessage response = client.PostAsJsonAsync(url, data).Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static TResponse Post<TRequst, TResponse>(string url, TRequst data)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsJsonAsync(url, data).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result.FromObject<TResponse>();
        }

        public static string PostFormData(string url, Dictionary<string, string> formData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            StringBuilder postData = new StringBuilder();
            foreach (KeyValuePair<string, string> item in formData)
            {
                postData.AppendFormat("{0}={1}&", item.Key, HttpUtility.UrlEncode(item.Value));
            }
            byte[] data = Encoding.ASCII.GetBytes(postData.ToString().TrimEnd('&'));
            request.ContentLength = data.Length;
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }

        public static string PostFormData(string url, List<FormItemModel> formItems, CookieContainer cookieContainer = null, string refererUrl = null, Encoding encoding = null, int timeOut = 20000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.Timeout = timeOut;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";
            if (!string.IsNullOrEmpty(refererUrl))
            {
                request.Referer = refererUrl;
            }
            if (cookieContainer != null)
            {
                request.CookieContainer = cookieContainer;
            }
            string boundary = "----" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            MemoryStream postStream = new MemoryStream();
            if (formItems != null && formItems.Count > 0)
            {
                string fileFormdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
                string dataFormdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (FormItemModel item in formItems)
                {
                    string formdata = null;
                    formdata = ((!item.IsFile) ? string.Format(dataFormdataTemplate, item.Key, item.Value) : string.Format(fileFormdataTemplate, item.Key, item.FileName));
                    byte[] formdataBytes = null;
                    formdataBytes = ((postStream.Length != 0) ? Encoding.UTF8.GetBytes(formdata) : Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2)));
                    postStream.Write(formdataBytes, 0, formdataBytes.Length);
                    if (item.FileContent == null || item.FileContent.Length <= 0)
                    {
                        continue;
                    }
                    using (Stream stream = item.FileContent)
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = 0;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            postStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
                byte[] footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                postStream.Write(footer, 0, footer.Length);
            }
            else
            {
                request.ContentType = "application/x-www-form-urlencoded";
            }
            request.ContentLength = postStream.Length;
            if (postStream != null)
            {
                postStream.Position = 0L;
                Stream requestStream = request.GetRequestStream();
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
                postStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (cookieContainer != null)
            {
                response.Cookies = cookieContainer.GetCookies(response.ResponseUri);
            }
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader myStreamReader = new StreamReader(responseStream, encoding ?? Encoding.UTF8))
                {
                    return myStreamReader.ReadToEnd();
                }
            }
        }
    }

    public class FormItemModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public bool IsFile
        {
            get
            {
                if (FileContent == null || FileContent.Length == 0)
                {
                    return false;
                }
                if (FileContent != null && FileContent.Length > 0 && string.IsNullOrWhiteSpace(FileName))
                {
                    throw new Exception("上传文件时 FileName 属性值不能为空");
                }
                return true;
            }
        }

        public string FileName { get; set; }

        public Stream FileContent { get; set; }
    }

}
