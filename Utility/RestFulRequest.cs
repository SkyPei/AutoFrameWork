using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Reflection;

namespace ApiFrameWork.Utility
{
    public static class RestFulRequest
    {
        public static void SetRequestMethod(this HttpWebRequest request, RestFulRequestMethod method)
        {
            request.Method = method.ToString();

        }

        public static List<T> Clone<T>(this List<T> list)
        {
            string content = JsonConvert.SerializeObject(list);

            return JsonConvert.DeserializeObject<List<T>>(content);

        }
        private static void Log(this HttpWebRequest request, string content = "")
        {

            Log.Log log = ApiFrameWork.Log.Log.GetLog();
            log.Info("---------------------Request-------------------\r\n");
            log.Info($"{request.Method.ToUpper()} {request.RequestUri} HTTP/{request.ProtocolVersion}\r\n");
            if (request.KeepAlive)
            {
                log.Info($"Connection: Keep-Alive\r\n");
            }
            else
            {
                log.Info($"Connection: close\r\n");
            }


            log.Info($"Host: {request.Host}\r\n");

            if (request.Headers.Count > 0)
            {
                foreach (var key in request.Headers.AllKeys)
                {
                    log.Info($"{key}: {request.Headers[key]}\r\n");
                }
            }
            if (request.CookieContainer != null && request.CookieContainer.Count > 0)
            {
                log.Info("Cookie:");
                var items = request.CookieContainer.GetCookies(new System.Uri(request.RequestUri.AbsoluteUri));
                string cookies = string.Empty;
                foreach (var item in items)
                {
                    cookies = cookies + $" {item.ToString()};";
                }
                cookies = cookies.Substring(0, cookies.Length - 1);
                log.Info($"{cookies}\r\n");

            }
            log.Info("\r\n");
            if (request.ContentLength > 0 && "GET" != request.Method.ToUpper())
            {

                log.Info(content);
                log.Info("\r\n");
            }


        }

        private static void Log(this HttpWebResponse response)
        {
            Log.Log log = ApiFrameWork.Log.Log.GetLog();

            log.Info("---------------------Response-------------------\r\n");
            log.Info($"HTTP/{response.ProtocolVersion} {((int)response.StatusCode).ToString()} {response.StatusDescription}\r\n");
            if (response.Headers.Count > 0)
            {
                foreach (var key in response.Headers.AllKeys)
                {
                    log.Info($"{key}: {response.Headers[key]}\r\n");
                }
            }
            log.Info("\r\n");

            string content = response.GetResponseBody();
            if (!string.IsNullOrEmpty(content))
            {
                if (response.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.Formatting = Formatting.Indented;
                        var jsoncontent = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(content), settings);
                        log.Info($"{jsoncontent}\r\n");
                    }
                    catch (System.Exception e)
                    {
                        log.Info($"{content}\r\n");
                    }
                }
                else
                {
                    log.Info($"{content}\r\n");
                }

            }


        }
        public static HttpWebResponse Send(this HttpWebRequest request)
        {
            if (string.IsNullOrEmpty(request.UserAgent))
            {
                request.UserAgent = "ApiFrameWork/1.0.0(.net Core 2.1)";
            }
            HttpWebResponse response = null;
            request.Log();
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.WebException e)
            {
                if (e.Response != null)
                {
                    response = e.Response as HttpWebResponse;

                }
                else
                {
                    throw e;

                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            response.Log();
            return response;

        }
        public static HttpWebResponse Send(this HttpWebRequest request, string content, Encoding encoding)
        {

            if (string.IsNullOrEmpty(request.UserAgent))
            {
                request.UserAgent = "ApiFrameWork/1.0.0(.net Core 2.1)";
            }
            HttpWebResponse response = null;
            if ("Get" != request.Method)
            {
                if (!string.IsNullOrEmpty(request.ContentType) && !request.ContentType.Contains(";"))
                {
                    request.ContentType += $";charset={encoding.BodyName}";
                }
                byte[] bytes = encoding.GetBytes(content);
                request.ContentLength = bytes.Length;

                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(bytes, 0, bytes.Length);

                }
            }
            request.Log(content);
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (System.Net.WebException e)
            {
                if (e.Response != null)
                {
                    response = e.Response as HttpWebResponse;

                }
                else
                {

                    throw e;

                }
            }
            catch (System.Exception e)
            {
                throw e;
            }
            response.Log();
            return response;
        }
        public static HttpWebResponse Send(this HttpWebRequest request, string content)
        {
            return Send(request, content, Encoding.UTF8);
        }

        public static Type Typen(this string str)
        {
            Type type = null;

            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asse in assemblyArray)
            {
                type = asse.GetType(str);
                if (type != null)
                {
                    return type;
                }
            }

            return type;
        }
        public static string GetResponseBody(this HttpWebResponse response)
        {

            string output = string.Empty;
            ApiFrameWork.Log.ResponseContent contents = ApiFrameWork.Log.ResponseContent.GetInstance();
            if (contents.List.Keys.Contains(response))
            {
                return contents.List[response];
            }
            else
            {

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    output = reader.ReadToEnd();
                    contents.List.Add(response, output);

                }
            }




            return output;

        }


    }


    public enum RestFulRequestMethod
    {
        Get, Post, Put, Delete, Head, Trace, Options
    }


}