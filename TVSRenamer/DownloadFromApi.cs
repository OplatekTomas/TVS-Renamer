using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TVSRenamer {
     public static class DownloadFromApi {
        public static string apiGet(string url, string token, int start) {
            
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers.Add("Accept-Language", "en");
                request.Headers.Add("Authorization", "Bearer " + token);
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    return sr.ReadToEnd();
                }
            } catch (WebException) {
                switch (start) {
                    case 0:
                        MessageBox.Show("Please re-check the data you entered and that you are connected to the internet!");
                        return "Error";
                    case 1:
                        return null;
                    default:
                        return "Error";
                }
            }
        }
        
        public static string getToken() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/login");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            try {
                using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                    string data = "{\"apikey\": \"0E73922C4887576A\",\"username\": \"Kaharonus\",\"userkey\": \"28E2687478CA3B16\"}";
                    streamWriter.Write(data);
                }
            } catch (WebException) { return null; }
            string text;
            var response = request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream())) {
                text = sr.ReadToEnd();
                text = text.Remove(text.IndexOf("\"token\""), "\"token\"".Length);
                text = text.Split('\"', '\"')[1];
                return text;
            }
        }
    }
}
    

