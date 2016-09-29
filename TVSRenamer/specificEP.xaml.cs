using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for specificEP.xaml
    /// </summary>
    public partial class specificEP : Window {
        string tkn;
        public specificEP(string token) {
            tkn = token;
            InitializeComponent();
        }
        string path;
        string ext;
        string info;
        string ID;
        private void textBox_TextChanged(object sender, TextChangedEventArgs e) {
        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e) {
            Regex regex = new Regex(@"^\d{1,2}$");
            if (!regex.IsMatch(textBox2.Text)) {
                MessageBox.Show("Enter valid number!");
                delete(1);
            }
            enableStart();
        }
        private void delete(int i) {
            textBox2.TextChanged -= textBox2_TextChanged;
            textBox3.TextChanged -= textBox3_TextChanged;
            if (i == 1) { textBox2.Text = null; } else { textBox3.Text = null; }
            textBox2.TextChanged += textBox2_TextChanged;
            textBox3.TextChanged += textBox3_TextChanged;
        }
        private void selectFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Supported Formats (*.mkv,*.srt,*.m4v,*.avi,*.mp4,*.mov,*.sub,*.wmv,*.flv,*.idx)|*.mkv;*.srt;*.m4v;*.avi;*.mp4;*.mov;*.sub;*.wmv;*.flv;*.idx";
            ofd.Multiselect = false;
            ofd.ShowDialog();
            path = ofd.FileName;
            textBox.Text = path;
            ext = Path.GetExtension(path);
            enableStart();
        }

        private void findShow_Click(object sender, RoutedEventArgs e) {
                string showNameNoSpaces = textBox_Copy.Text.Replace(" ", "+");
                string getName = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?name=" + showNameNoSpaces, tkn, 0);
                info = getName;
                if (info == "Error") {
                    MessageBox.Show("Re-check your data or make sure that you are connected to the internet.");
                } else { 
                var w = new SelectShow(info);
                w.ShowDialog();
                ID = w.Return;
                enableStart();
                }
        }

        private void textBox3_TextChanged(object sender, TextChangedEventArgs e) {
            Regex regex = new Regex(@"^\d{1,2}$");
            if (!regex.IsMatch(textBox3.Text)) {
                MessageBox.Show("Enter valid number!");
                delete(4);
            }
            enableStart();
        }
        private void enableStart(){
            if (textBox2.Text != "" && textBox3.Text != "" && ID != null &&File.Exists(textBox.Text)) { button2.IsEnabled = true; } else { button2.IsEnabled = false; }
        }
       
    private void button2_Click(object sender, RoutedEventArgs e) {
            int season = Int32.Parse(textBox2.Text);
            int episode = Int32.Parse(textBox3.Text);
            string name = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + ID + "/episodes/query?airedSeason=" + season + "&airedEpisode=" + episode,tkn,0);
            string showName = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + ID,tkn,0);
            JObject parsed = JObject.Parse(name);
            JObject parsedSN = JObject.Parse(info);
            name = parsed["data"][0]["episodeName"].ToString();
            showName = parsedSN["data"][0]["seriesName"].ToString();
            string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            foreach (char znak in invalid) {
                name = name.Replace(znak.ToString(), "");
            }
            if (season < 10) {
                if (episode < 10) { name = showName + " - S0" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { name = showName + " - S0" + season + "E" + episode + " - " + name; }
            } else if (season < 10) {
                if (episode < 10) { name = showName + " - S" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { name = showName + " - S" + season + "E" + episode + " - " + name; ; }
            }
            try {
                File.Move(path, Path.GetDirectoryName(path) + name + Path.GetExtension(path));
                MessageBox.Show("File was removed!");
            } catch (IOException) { MessageBox.Show("Something went wrong\nAre you sure file " + path + " isn't being used?"); }
        }
    }
}
