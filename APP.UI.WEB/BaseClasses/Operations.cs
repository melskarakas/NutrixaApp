using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ORM.Business;
using ORM.Models;
using ORM.Models.Models.CustomModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace APP.UI.WEB.BaseClasses
{
    public static class Operations
    {
        private static string WebApiUrl = "http://localhost:58613/"; //project local
        private static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static string GetSessionValue(string key)
        {
            return _httpContextAccessor?.HttpContext?.Session.GetString(key);
        }

        public static void SetSessionValue(string key, string value)
        {
            _httpContextAccessor?.HttpContext?.Session.SetString(key, value);
        }
        public static async Task<string> Authenticate(string UserName, string Password)
        {
            try
            {
                string Uri = WebApiUrl + "Users/authenticate";
                var loginData = new AuthenticateRequest
                {
                    Username = UserName,
                    Password = Password,
                    IsPasswordMd5 = true
                };
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(Uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();

                        // Token'ı JSON string'den manuel olarak çekelim
                        var json = JObject.Parse(responseString);
                        string token = json["token"]?.ToString(); // JSON içerisindeki 'Token' alanını alıyoruz

                        return token;
                    }

                    return ""; // Eğer yetkilendirme başarısız olursa null döndür
                }

            }
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                if (!string.IsNullOrEmpty(resp))
                {
                    dynamic obj = (dynamic)JsonConvert.DeserializeObject(resp);
                    var messageFromServer = obj.StatusMessage;

                    return "";
                    throw new Exception(ex.Message + " Detail : " + messageFromServer);
                }

                return "";
                throw new Exception(ex.Message);

            }

        }
        public static T GET<T>(string FunctionName)
        {
            try
            {
                string Result = "";
                string Uri = WebApiUrl + FunctionName;
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri);
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Token", GetSessionValue("token"));
                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    }

                    Stream stream = httpWebResponse.GetResponseStream();
                    using (StreamReader read = new StreamReader(stream))
                    {
                        Result = read.ReadToEnd();
                    }
                }
                T value = (T)JsonConvert.DeserializeObject(Result, typeof(T));
                return value;
            }
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = (dynamic)JsonConvert.DeserializeObject(resp);
                var messageFromServer = obj.StatusMessage;

                throw new Exception(ex.Message + " Detail : " + messageFromServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string POST<T>(string functionName, T BodyObject)
        {
            try
            {
                string Result = "";
                string Uri = WebApiUrl + functionName;
                string Body = JsonConvert.SerializeObject(BodyObject);
                string token = GetSessionValue("token");
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri);
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Token", GetSessionValue("token"));
                if (Body.Length > 0)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = Body;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    }

                    Stream stream = httpWebResponse.GetResponseStream();
                    using (StreamReader read = new StreamReader(stream))
                    {
                        Result = read.ReadToEnd();
                    }
                }

                return Result;
            }
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = (dynamic)JsonConvert.DeserializeObject(resp);
                var messageFromServer = obj.StatusMessage;

                throw new Exception(ex.Message + " Detail : " + messageFromServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool PUT<T>(string functionName, T BodyObject)
        {
            try
            {
                string Result = "";
                string Uri = WebApiUrl + functionName;
                string Body = JsonConvert.SerializeObject(BodyObject);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri);
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("Token", GetSessionValue("token"));

                if (Body.Length > 0)
                {
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = Body;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }
                }

                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    }

                    Stream stream = httpWebResponse.GetResponseStream();
                    using (StreamReader read = new StreamReader(stream))
                    {
                        Result = read.ReadToEnd();
                    }
                }

                return Convert.ToBoolean(Result);
            }
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = (dynamic)JsonConvert.DeserializeObject(resp);
                var messageFromServer = obj.StatusMessage;

                throw new Exception(ex.Message + " Detail : " + messageFromServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static bool DELETE(string functionName)
        {
            try
            {
                string Result = "";
                string Uri = WebApiUrl + functionName;

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Uri);
                httpWebRequest.KeepAlive = true;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "DELETE";
                httpWebRequest.Headers.Add("Token", GetSessionValue("token"));

                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    }

                    Stream stream = httpWebResponse.GetResponseStream();
                    using (StreamReader read = new StreamReader(stream))
                    {
                        Result = read.ReadToEnd();
                    }
                }

                return Convert.ToBoolean(Result);
            }
            catch (WebException ex)
            {
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = (dynamic)JsonConvert.DeserializeObject(resp);
                var messageFromServer = obj.StatusMessage;

                throw new Exception(ex.Message + " Detail : " + messageFromServer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
