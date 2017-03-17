﻿using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Paydock_dotnet_sdk.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.Text;

namespace Paydock_dotnet_sdk.Services
{
    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    /// <summary>
    /// Helper class to handle calling the API and basic serialisation
    /// </summary>
    public class ServiceHelper : IServiceHelper
    {
        static ServiceHelper()
        {
            // set to TLS1.2
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.DefaultConnectionLimit = 9999;
        }

        /// <summary>
        /// Call the API, throws ResponseException on any errors
        /// </summary>
        /// <param name="url">relative URL to call (eg charge or notification/templates)</param>
        /// <param name="method">HTTP method to call</param>
        /// <param name="json">Data to send, will be ignored for some HTTP methods</param>
        /// <returns>the response string</returns>
        public string CallPaydock(string url, HttpMethod method, string json)
        {
            var url = Config.BaseUrl() + url;
            var request = HttpWebRequest.Create((string)url);

            request.ContentType = "application/json";
            request.Headers.Add("x-user-secret-key", Config.SecretKey);
            request.Method = method.ToString();

            string result = "";

            var webRequest = WebRequest.Create((string)url);
            webRequest.Method = method.ToString();
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("x-user-secret-key", Config.SecretKey);

            if (method == HttpMethod.POST || method == HttpMethod.PUT)
            {
                using (var stream = request.GetRequestStream())
                {
                    var data = Encoding.UTF8.GetBytes(json);
                    stream.Write(data, 0, json.Length);
                }
            }

            WebResponse response = null;
            try
            {
                response = request.GetResponse();
            }

            catch (WebException ex)
            {
                ConvertException(ex);
            }

            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            
            return result;
        }
        
        /// <summary>
        /// Parses and converts exception & response into common format
        /// </summary>
        /// <param name="exception">Exception to convert</param>
        private void ConvertException(WebException exception)
        {
            using (var reader = new StreamReader(exception.Response.GetResponseStream()))
            {
                var result = reader.ReadToEnd();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(result, new ExpandoObjectConverter());
                var errorResponse = new ErrorResponse()
                {
                    Status = Convert.ToInt32(obj.status),
                    ExtendedInformation = obj,
                    JsonResponse = result
                };

                var json = JObject.Parse(result);
                if (json["error"]["message"].Count() == 0)
                {
                    errorResponse.ErrorMessage = (string)json["error"]["message"];
                }
                else if (json["error"]["message"]["message"].Count() == 0)
                {
                    errorResponse.ErrorMessage = (string)json["error"]["message"]["message"];
                }
                
                throw new ResponseException(errorResponse, exception.Status.ToString());
            }
        }
    }
}
