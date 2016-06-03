using DeJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTHttpRequest
    {
        public class Options
        {
            public string contentType = "application/json";
            public string content;
            public string verb = "POST";
            public string url;
        }

        static string GetResponseContent(System.Net.HttpWebResponse response)
        {
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string result = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            return result;
        }

        static public System.Net.HttpStatusCode SyncRequest(Options options, out string result)
        {
            System.Net.HttpStatusCode code = System.Net.HttpStatusCode.Ambiguous;
            result = "";
            try
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(options.url);
                request.Method = options.verb;
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(options.content);
                request.ContentType = options.contentType;
                request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                result = GetResponseContent(response);
                code = response.StatusCode;
                response.Close();
            }
            catch (System.Net.WebException ex)
            {
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)ex.Response;
                code = response.StatusCode;
                result = GetResponseContent(response);
                response.Close();
            }
            return code;
        }
    }
}
