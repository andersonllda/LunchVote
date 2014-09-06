using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;

namespace HMV.Core.Framework.Helper
{
    public class JsonHelper
    {

        public static string objectToJson(object objeto)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Serialize(objeto);
        }

        public static string listObjectToJson(object objeto)
        {
            StringBuilder sb = new StringBuilder(); 
            JavaScriptSerializer js = new JavaScriptSerializer();
            js.Serialize(objeto,sb);
            return sb.ToString();
        }

        public static object jsonToObject<T>(string jSon)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            return js.Deserialize<T>(jSon);
        }

        public static T urlJsonToObject<T>(string url)
        {
            WebClient client = new WebClient(); 
            byte[] data = client.DownloadData(url);
            string jsonString = System.Text.Encoding.GetEncoding("utf-8").GetString(data);
             
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //return jss.Deserialize<T>(jsonString);
            return JsonDeserialize<T>(jsonString);
        }

        // post, put e delete
        public static T urlJsonRestToObject<T>(string method,string url, object objPost)
        {
            WebClient client = new WebClient();
            client.Headers["Content-type"] = "application/json";

            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(objPost.GetType());
            serializer.WriteObject(stream, objPost);

            byte[] data = client.UploadData(url, method, stream.ToArray());

            string jsonString = System.Text.Encoding.GetEncoding("utf-8").GetString(data);
            //JavaScriptSerializer jss = new JavaScriptSerializer();
            //return jss.Deserialize<T>(jsonString);
            return JsonDeserialize<T>(jsonString);
        }

        private static T JsonDeserialize<T>(string jsonString)
        {
            //Convert "yyyy-MM-dd HH:mm:ss" String as "\/Date(1319266795390-0300)\/"
            
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(@"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}");
           
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;

            DateTime dt = DateTime.Parse(m.Groups[0].Value);

            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}-0300)\\/", ts.TotalMilliseconds);
            return result;
        }

    }
}
