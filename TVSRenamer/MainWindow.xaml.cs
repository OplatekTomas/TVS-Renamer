using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using TVSRenamer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window {
        string settings1 = Properties.Settings.Default["Lokace1"].ToString();
        string settings2 = Properties.Settings.Default["Lokace2"].ToString();
        string settings3 = Properties.Settings.Default["Lokace3"].ToString();
        string location;
        string showName;
        string TVID;
        int subFolders;
        const int namesCount = 50;
        string[,,] names = new string[namesCount, namesCount, 4];
        string token = Properties.Settings.Default["Token"].ToString();
        string[] fileExtension = new string[10] { ".mkv", ".srt", ".m4v", ".avi", ".mp4", ".mov", ".sub", ".wmv", ".flv", ".idx" };
        string info;
        string defLoc;
        List<string> isShowFile = new List<string>();
        FolderBrowserDialog fbd = new FolderBrowserDialog();


        private void generateSearch() {
            for (int season = 1; season <= namesCount; season++) {
                for (int episode = 1; episode <= namesCount; episode++) {
                    if (season < 10) {
                        if (episode < 10) { runGen("0", "0", season, episode); }
                        if (episode >= 10) { runGen("0", "", season, episode); }
                    } else if (season >= 10) {
                        if (episode < 10) { runGen("", "0", season, episode); }
                        if (episode >= 10) { runGen("", "", season, episode); }
                    }

                }
            }
        }
        private void runGen(string seasonNumb, string episodeNumb, int season, int episode) {
            names[season - 1, episode - 1, 0] = "S" + seasonNumb + season + "E" + episodeNumb + episode;
            names[season - 1, episode - 1, 1] = "S" + seasonNumb + season + ".E" + episodeNumb + episode;
            names[season - 1, episode - 1, 2] = "S" + seasonNumb + season + " E" + episodeNumb + episode;
            names[season - 1, episode - 1, 3] = seasonNumb + season + "x" + episodeNumb + episode;
        }
        private string checkNameExists() {
            string showNameNoSpaces = showName.Replace(" ", "+");
            string getName = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?name=" + showNameNoSpaces, token, 0);
            info = getName;
            if (info == "Error") {
                return "Error!";
            }
            var w = new SelectShow(info);
            w.ShowDialog();
            return w.Return;
        }
        private string checkNameExistsIMDb() {
            string getName = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?imdbId=" + showName, token, 0);
            string name;
            if (info == "error") {
                return "Error!";
            }
            JObject parsed = JObject.Parse(getName);
            TVID = parsed["data"][0]["id"].ToString();
            info = DownloadFromApi.apiGet("https://api.thetvdb.com/series" + TVID, token, 0);
            name = parsed["data"][0]["seriesName"].ToString();
            return name;
        }
        private void button_Click_1(object sender, RoutedEventArgs e) {
            fbd.ShowDialog();
            TVLoc.Text = fbd.SelectedPath;
            Name.Text = System.IO.Path.GetFileName(fbd.SelectedPath);
            location = fbd.SelectedPath;
            showName = System.IO.Path.GetFileName(fbd.SelectedPath);
        }

        private void InfoBtn_Click(object sender, RoutedEventArgs e) {
            var window = new Window1 { Owner = this };
            window.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            var window = new OknoSettings { Owner = this };
            window.ShowDialog();

            try { settings1 = TVSRenamer.Properties.Settings.Default["Lokace1"].ToString(); } catch (NullReferenceException) { settings1 = null; }
            try { settings2 = TVSRenamer.Properties.Settings.Default["Lokace2"].ToString(); } catch (NullReferenceException) { settings2 = null; }
            try { settings3 = TVSRenamer.Properties.Settings.Default["Lokace3"].ToString(); } catch (NullReferenceException) { settings3 = null; }
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            textShowName.Foreground = Brushes.Black;
            if (checkBox.IsChecked == true) {
                textShowName.Text = checkNameExistsIMDb();
            } else { textShowName.Text = getFromID(); }
            if (textShowName.Text != "Error!") {
                Start.IsEnabled = true;
                showName = textShowName.Text;
            } else { textShowName.Foreground = Brushes.Tomato; textShowName.Text = "TV show was not found!"; }

        }
        private string getFromID() {
            TVID = checkNameExists();
            if (TVID != "Error!") {
                info = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + TVID, token, 0);
                if (info == "Error") {
                    return "Error!";
                }
                JObject tvShowName = JObject.Parse(info);
                return tvShowName["data"]["seriesName"].ToString();
            } else { return "Error!"; }
        }
        private void Name_TextChanged(object sender, TextChangedEventArgs e) {
            showName = Name.Text;
            Start.IsEnabled = false;
            if (showName != null & Directory.Exists(location)) {
                button1.IsEnabled = true;
            } else { button1.IsEnabled = false; }
        }
        private void TVLoc_TextChanged(object sender, TextChangedEventArgs e) {
            location = TVLoc.Text;
            Start.IsEnabled = false;
            if (Directory.Exists(location) & showName != null) {
                button1.IsEnabled = true;
            } else { button1.IsEnabled = false; 
            }
        }
        private void search() {
            List<string> files = new List<string>();
            try {
                files.AddRange(Directory.GetFiles(settings1, "*.*", SearchOption.AllDirectories));
            } catch (ArgumentException) { }
            try {
                files.AddRange(Directory.GetFiles(settings2, "*.*", SearchOption.AllDirectories));
            } catch (ArgumentException) { }
            try {
                files.AddRange(Directory.GetFiles(settings3, "*.*", SearchOption.AllDirectories));
            } catch (ArgumentException) { }
            moveNew(files.ToArray());
        }
        private void moveNew(string[] files) {
            List<string> names = new List<string>();
            names.Add(showName);
            if (showName != showName.Replace(' ', '.')) { names.Add(showName.Replace(' ', '.')); }
            for (int n = 0; n < getNumberAliases(); n++) {
                names.Add(getAliases(n));
                if (getAliases(n) != getAliases(n).Replace(' ', '.')) { names.Add(getAliases(n).Replace(' ', '.')); }
            }
            if (Path.GetFileName(location) != showName) {
                defLoc = location;
                Directory.CreateDirectory(location + "\\" + showName);
                location = location + "\\" + showName;
            }
            for (int file = 0; file < files.Length; file++) {
                for (int i = 0; i < names.Count(); i++) {
                    if (files[file].IndexOf(names[i], StringComparison.OrdinalIgnoreCase) >= 0) {
                        isShowFile.Add(files[file]);
                    }
                }
            }
        }
 
        private string getAliases(int number) {
            string getName = info;
            JObject test = JObject.Parse(getName);
            try {
                var alias = test["data"]["aliases"][number];
                return alias.ToString();
            } catch (ArgumentOutOfRangeException) {
                return null;
            }

        }

        private int getNumberAliases() {
            string getName = info;
            JObject test = JObject.Parse(getName);
            return test["data"]["aliases"].Count();
        }


        private void renameNew(string[] files) {
            string epName;
            for (int file = 0; file < files.Count(); file++) {
                for (int season = 0; season < namesCount; season++) {
                    for (int episode = 0; episode < namesCount; episode++) {
                        for (int variability = 0; variability < 3; variability++) {
                            for (int ext = 0; ext < 10; ext++) {
                                if (files[file].IndexOf(names[season, episode, variability], StringComparison.OrdinalIgnoreCase) >= 0 && files[file].IndexOf(fileExtension[ext], StringComparison.OrdinalIgnoreCase) >= 0) {
                                    epName = getName(season + 1, episode + 1);
                                    if (!files[file].Contains(epName) && Path.GetDirectoryName(files[file]) != defLoc + "Season???") {
                                        if (Int32.Parse(Properties.Settings.Default["Danger"].ToString()) == 1) {
                                            try { File.Move(files[file], pathMove(season, fileExtension[ext], epName)); } catch (IOException) { MessageBox.Show("Check if file is not being used!"); }
                                        } else { File.Move(files[file], pathMove(season, fileExtension[ext], epName)); }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private string pathMove(int season, string extension, string epName) {
            string path = null;
            if (season < 10) {
                path = location + "\\" + "Season " + "0" + (season + 1) + "\\" + epName + extension;
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }
            if (season >= 10) {
                path = location + "\\" + "Season " + (season + 1) + "\\" + epName + extension;
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }


            return path;
        }

        private string getName(int season, int episode) {
            string name = null;
            string final = null;
            string info = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + TVID + "/episodes/query?airedSeason=" + season + "&airedEpisode=" + episode, token, 0);
            JObject parse = JObject.Parse(info);
            name = parse["data"][0]["episodeName"].ToString();
            string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            foreach (char znak in invalid) {
                name = name.Replace(znak.ToString(), "");
            }
            if (season < 10) {
                new System.IO.FileInfo(location + "\\" + "Season 0" + season).Directory.Create();
                if (episode < 10) { final = showName + " - S0" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { final = showName + " - S0" + season + "E" + episode + " - " + name; }
            } else if (season < 10) {
                new System.IO.FileInfo(location + "\\" + "Season 0" + season).Directory.Create();
                if (episode < 10) { final = showName + " - S" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { final = showName + " - S" + season + "E" + episode + " - " + name; ; }
            }
            if (!File.Exists(final)) { return final; } else {
                int filenumber = 1;
                do {
                    final = final + "_" + filenumber;
                    filenumber++;
                } while (File.Exists(final) == false);
                return final;
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e) {
            defLoc = location;
            generateSearch();
            bool loaded=true;
            if (settings1 != null || settings2 != null || settings3 != null || settings1 != "" || settings2 != "" || settings3 != "") {
                search();
                List<String> isShowFileNoDuplicates = isShowFile.Distinct().ToList();
                renameNew(isShowFileNoDuplicates.ToArray());
            }
            List<string> files = new List<string>();
            try {
                files = System.IO.Directory.GetFiles(defLoc, "*.*", System.IO.SearchOption.AllDirectories).ToList<string>();
            } catch (Exception) { loaded = false; }
            if (loaded) {
                if (Properties.Settings.Default.folder == 1) {
                    renameNew(files.ToArray());
                } else {
                    isShowFile = null;
                    moveNew(files.ToArray());
                    List<String> isShowFileNoDuplicates = isShowFile.Distinct().ToList();
                    renameNew(isShowFileNoDuplicates.ToArray());
                }
            } else { MessageBox.Show("Please make sure the folder isn't empty!"); }       
            MessageBox.Show("Files that were found were probably renamed!", "Done");
        }


        private void ComboBox_Loaded(object sender, RoutedEventArgs e) {
            List<string> data = new List<string>();
            data.Add("Downloads folder");
            data.Add("TV Show folder");
            var comboBox = sender as System.Windows.Controls.ComboBox;
            comboBox.ItemsSource = data;
            int setOption = Int32.Parse(TVSRenamer.Properties.Settings.Default["dbdown"].ToString());           
            comboBox.SelectedIndex = setOption;

        }

        private void checkBox_Checked(object sender, RoutedEventArgs e) {
            
        }
        private void specific_Click(object sender, RoutedEventArgs e) {
            var window = new specificEP(token) { Owner = this };
            window.ShowDialog();
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            string info = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?name=The%20100", token, 1);
            if (info == null) {
                token = DownloadFromApi.getToken();
                if (token == null) {
                    MessageBox.Show("You are either not connected to the internet or API is not responding. TVS Renamer will now close");
                    this.Close();
                } else {
                    Properties.Settings.Default["Token"] = token;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}