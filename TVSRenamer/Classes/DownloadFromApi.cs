using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TVSRenamer {
     public class API {
        public static List<Show> getShows(string name) {
            List<Show> list = new List<Show>();
            name = name.Replace(" ", "+");
            WebRequest wr = WebRequest.Create("http://api.tvmaze.com/search/shows?q=" + name);
             HttpWebResponse response = null;
            try {
                response = (HttpWebResponse)wr.GetResponse();
            } catch (Exception) {
                return list;
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            //JObject jo = JObject.Parse(responseFromServer);
            JArray test = JArray.Parse(responseFromServer);
            foreach (JToken jt in (JToken)test) { 
                JToken value = jt["show"];
                Show s = new Show();
                if (value["externals"]["thetvdb"].ToString() != "") {
                    s.tvdbID = Int32.Parse(value["externals"]["thetvdb"].ToString());                
                    s.id = Int32.Parse(value["id"].ToString());
                    s.name = value["name"].ToString();
                    if(value["rating"]["average"].ToString() != "") {
                        s.rating = float.Parse(value["rating"]["average"].ToString());
                    } else {
                        s.rating = 0;
                    }
                    s.imdb = "www.imdb.com/title/" + value["externals"]["imdb"].ToString() + "/?ref_=nv_sr_1";              
                    if (value["image"].ToString() != "") { 
                        s.image = value["image"]["original"].ToString();
                    }
                    if (value["network"].ToString() != "") {
                        s.station = value["network"]["name"].ToString();
                    }
                    if (value["summary"].ToString() != "") {
                        s.overview = value["summary"].ToString();
                    }
                    foreach (JToken genres in value["genres"]) {
                        s.genres.Add(genres.ToString());
                    }
                    try {
                        s.releaseDate = DateTime.ParseExact(value["premiered"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture).ToString("dd.MM.yyyy");
                    } catch (Exception) {
                        s.releaseDate = "";
                    }               
                    list.Add(s);
                }
            }
            return list;
        }
        private static string getToken() {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.thetvdb.com/login");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            try {
                using (var streamWriter = new StreamWriter(request.GetRequestStream())) {
                    string data = "{\"apikey\": \"0E73922C4887576A\",\"username\": \"Kaharonus\",\"userkey\": \"28E2687478CA3B16\"}";
                    streamWriter.Write(data);
                }
            } catch (WebException) { MessageBox.Show("Connection error"); }
            string text;
            var response = request.GetResponse();
            using (var sr = new StreamReader(response.GetResponseStream())) {
                text = sr.ReadToEnd();
                text = text.Remove(text.IndexOf("\"token\""), "\"token\"".Length);
                text = text.Split('\"', '\"')[1];
                return text;
            }
        }
        public static List<string> GetAliases(Show s) {
            List<string> aliases = new List<string>();
            Regex reg = new Regex(@"\([0-9]{4}\)");
            aliases.Add(s.name);
            Match snMatch = reg.Match(s.name);
            if (snMatch.Success) {
                aliases.Add(reg.Replace(s.name, ""));
            }
            foreach (string alias in getAliasToken(s.tvdbID)) {
                aliases.Add(alias);
                Match regMatch = reg.Match(alias);
                if (regMatch.Success) {
                    aliases.Add(reg.Replace(alias, ""));
                }
            }
            for (int i = 0; i < aliases.Count(); i++) {
                if (aliases[i].Contains(" ")) {
                    aliases.Add(aliases[i].Replace(" ", "."));
                }
            }
            return aliases;
        }
        private static JToken getAliasToken(int id) {
            HttpWebRequest request = getRequest("https://api.thetvdb.com/series/" + id);
            try {
                var response = request.GetResponse();
                using (var sr = new StreamReader(response.GetResponseStream())) {
                    JObject jo = JObject.Parse(sr.ReadToEnd());
                    return jo["data"]["aliases"];
                }
            } catch (WebException) {
                return null;
            }
        }
        private static HttpWebRequest getRequest(string link) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers.Add("Accept-Language", "en");
            request.Headers.Add("Authorization", "Bearer " + API.getToken());
            return request;
        }
        public static List<Episode> RequestEpisodes(Show s) {
            List<Episode> list = new List<Episode>();
            string name = s.name.Replace(" ", "+");
            WebRequest wr = WebRequest.Create("http://api.tvmaze.com/search/shows?q=" + name);
            HttpWebResponse response = null;
            try {
                response = (HttpWebResponse)wr.GetResponse();
            } catch (Exception) {
                return list;
            }
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            JArray jo = JArray.Parse(responseFromServer);
            foreach (JToken jt in jo) {
                Episode e = new Episode();
                e.season = Int32.Parse(jt["season"].ToString());
                e.episode = Int32.Parse(jt["episode"].ToString());
                e.name = jt["name"].ToString();
                list.Add(e);
            }
            return list;
        }
    }
    public class Show {
        public string name;
        public string releaseDate;
        public int id;
        public float rating;
        public string imdb;
        public int tvdbID;
        public string image;
        public string station;
        public string overview;
        public List<string> genres = new List<string>();
        public List<string> aliases = new List<string>();
    }
    public class Episode {
        public string name;
        public int season;
        public int episode;
    }
}



    

