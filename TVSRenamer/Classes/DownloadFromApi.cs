using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
            return list;
        }
     }
    public class Show {
        public string name;
        public string releaseDate;
        public int id;
        public float rating;
        public string imdb;
        public string image;
        public string station;
        public string overview;
        public List<string> genres = new List<string>();
    }
}
    

