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
        string[,,] names = new string[namesCount, namesCount, 200];
        string token = Properties.Settings.Default["Token"].ToString();
        string[] fileExtension = new string[10] { ".mkv", ".srt", ".m4v", ".avi", ".mp4", ".mov", ".sub", ".wmv", ".flv", ".idx" };
        string info;
        string defLoc;
        FolderBrowserDialog fbd = new FolderBrowserDialog();
        private void generateSearch() {
            for (int season = 1; season <= namesCount; season++) {
                for (int episode = 1; episode <= namesCount; episode++) {
                    if (season < 10) {
                        if (episode < 10) { runGen("0", "0", season, episode); }
                        if (episode >= 10) { runGen("0", "", season, episode); }
                    } else if (season < 10) {
                        if (episode < 10) { runGen("", "0", season, episode); }
                        if (episode >= 10) { runGen("", "", season, episode); }
                    }

                }
            }
        }
        private void runGen(string seasonNumb, string episodeNumb, int season, int episode) {
            string showNameNoSpaces;
            showNameNoSpaces = showName.Replace(" ", ".");
            names[season - 1, episode - 1, 0] = "S" + seasonNumb + season + "E" + episodeNumb + episode;
            names[season - 1, episode - 1, 1] = "S" + seasonNumb + season + ".E" + episodeNumb + episode;
            names[season - 1, episode - 1, 2] = "S" + seasonNumb + season + " E" + episodeNumb + episode;
            names[season - 1, episode - 1, 3] = seasonNumb + season + "x" + episodeNumb + episode;
            names[season - 1, episode - 1, 4] = showName + " S" + seasonNumb + season + "E" + episodeNumb + episode;
            names[season - 1, episode - 1, 5] = showName + " S" + seasonNumb + season + ".E" + episodeNumb + episode;
            names[season - 1, episode - 1, 6] = showName + " - S" + seasonNumb + season + "E" + episodeNumb + episode;
            names[season - 1, episode - 1, 7] = showName + " - S" + seasonNumb + season + ".E" + episodeNumb + episode;
            names[season - 1, episode - 1, 8] = showName + " - S" + seasonNumb + season + " E" + episodeNumb + episode;
            names[season - 1, episode - 1, 9] = showName + " S" + seasonNumb + season + " E" + episodeNumb + episode;
            names[season - 1, episode - 1, 10] = showName + " " + seasonNumb + season + "x" + episodeNumb + episode;
            names[season - 1, episode - 1, 11] = showName + " - " + seasonNumb + season + "x" + episodeNumb + episode;
            names[season - 1, episode - 1, 12] = showNameNoSpaces + ".S" + seasonNumb + +season + "E" + episodeNumb + episode;
            names[season - 1, episode - 1, 13] = showNameNoSpaces + ".S" + seasonNumb + +season + ".E" + episodeNumb + episode;
            names[season - 1, episode - 1, 14] = showNameNoSpaces + "." + seasonNumb + season + "x" + episodeNumb + episode;
            for (int namesCount = 0; namesCount < getNumberAliases(); namesCount++) {
                names[season - 1, episode - 1, 5 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " S" + seasonNumb + season + "E" + episodeNumb + episode;
                names[season - 1, episode - 1, 6 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " S" + seasonNumb + season + ".E" + episodeNumb + episode;
                names[season - 1, episode - 1, 7 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount).Replace(" ", ".") + ".S" + seasonNumb + season + "E" + episodeNumb + episode;
                names[season - 1, episode - 1, 8 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount).Replace(" ", ".") + ".S" + seasonNumb + season + ".E" + episodeNumb + episode;
                names[season - 1, episode - 1, 9 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " " + seasonNumb + season + "x" + episodeNumb + episode;
                names[season - 1, episode - 1, 10 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount).Replace(" ", ".") + "." + seasonNumb + season + "x" + episodeNumb + episode;
                names[season - 1, episode - 1, 11 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " S" + seasonNumb + season + " E" + episodeNumb + episode;
                names[season - 1, episode - 1, 12 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " - S" + seasonNumb + season + "E" + episodeNumb + episode;
                names[season - 1, episode - 1, 13 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " - S" + seasonNumb + season + " E" + episodeNumb + episode;
                names[season - 1, episode - 1, 14 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " - S" + seasonNumb + season + ".E" + episodeNumb + episode;
                names[season - 1, episode - 1, 15 + namesCount + (11 * (namesCount + 1))] = getAliases(namesCount) + " - " + seasonNumb + season + "x" + episodeNumb + episode;

            }

        }
        
       
        private string checkNameExists() {
            string showNameNoSpaces = showName.Replace(" ", "+");
            string getName = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?name=" + showNameNoSpaces, token,0);
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

        private string ownedSeasons() {
            string downloadedSeasons = null;
            try {
                int[] cislaSerii = new int[50];
                subFolders = System.IO.Directory.GetDirectories(location, "Season???").Length;
                string[] lokaceNazev = new string[50];
                string[] lokace = new string[100];
                lokace = Directory.GetDirectories(location, "Season???");
                for (int i = 0; i < subFolders; i++) {
                    lokaceNazev[i] = System.IO.Path.GetFileName(lokace[i]);
                }
                for (int i = 0; i < subFolders; i++) {
                    cislaSerii[i] = Int32.Parse(lokaceNazev[i].Substring(lokaceNazev[i].Length - Math.Min(2, lokaceNazev[i].Length)));
                    downloadedSeasons += cislaSerii[i].ToString() + ", ";
                }
                textOwnedSeasons.Foreground = Brushes.Black;
                return downloadedSeasons.Remove(downloadedSeasons.Length - 2);
            } catch (NullReferenceException) {
                textOwnedSeasons.Foreground = Brushes.Tomato;
                if (textShowName.Text == "Error!") {
                    Start.IsEnabled = false;}
                return "No seasons were detected"; } 
            catch (ArgumentException) {
                textOwnedSeasons.Foreground = Brushes.Tomato;
                Start.IsEnabled = false;
                return "No seasons were detected"; }

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
                ownedSeasons();
            } else { textShowName.Foreground = Brushes.Tomato; textShowName.Text = "TV show was not found!"; }
            
        }
        private string getFromID() {
            TVID=checkNameExists();
            if (TVID != "Error!") {
                info = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + TVID, token, 0);
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
            } else { button1.IsEnabled = false; }
        }
        private bool search() {
            int f1L = 0;
            int f2L = 0;
            int f3L = 0;
            bool time = false;
            try {
                f1L = System.IO.Directory.GetFiles(settings1, "*.*", System.IO.SearchOption.AllDirectories).Length;
            } catch (ArgumentException) { }
            try {
                f2L = System.IO.Directory.GetFiles(settings2, "*.*", System.IO.SearchOption.AllDirectories).Length;
            } catch (ArgumentException) { }
            try {
                f3L = System.IO.Directory.GetFiles(settings3, "*.*", System.IO.SearchOption.AllDirectories).Length;
            } catch (ArgumentException) { }
            if (f1L > 10000) { time = true; MessageBox.Show("Number of files in folder " + settings1 + " are higher then max limit (10 000)"); }
            if (f2L > 10000) { time = true; MessageBox.Show("Number of files in folder " + settings2 + " are higher then max limit (10 000)"); }
            if (f3L > 10000) { time = true; MessageBox.Show("Number of files in folder " + settings3 + " are higher then max limit (10 000)"); }
            if (time == false) {
                string[] folder1 = new string[f1L];
                string[] folder2 = new string[f2L];
                string[] folder3 = new string[f3L];
                try {
                    folder1 = System.IO.Directory.GetFiles(settings1, "*.*", System.IO.SearchOption.AllDirectories);
                } catch (ArgumentException) { }
                try {
                    folder2 = System.IO.Directory.GetFiles(settings2, "*.*", System.IO.SearchOption.AllDirectories);
                } catch (ArgumentException) { }
                try {
                    folder3 = System.IO.Directory.GetFiles(settings3, "*.*", System.IO.SearchOption.AllDirectories);
                } catch (ArgumentException) { }
                for (int files = 0; files < 3; files++) {
                    if (files == 0) move(folder1);
                    if (files == 1) move(folder2);
                    if (files == 2) move(folder3);
                }
                return true;
            } else { return false; }
        }

        private void move(string[] files) {
            int lenght = files.Length;
            int number = 0;
            if (Path.GetFileName(location) != showName) {
                defLoc = location;
                Directory.CreateDirectory(location + "\\" + showName);
                location = location + "\\" + showName;               
            }
            for (int file = 0; file < lenght; file++) {
                number = 0;
                for (int season = 0; season < namesCount; season++) {
                    for (int episode = 0; episode < namesCount; episode++) {
                        for (int variance = 4; variance < 200; variance++) {
                            if (names[season, episode, variance] != null) {
                                for (int ext = 0; ext < fileExtension.Count(); ext++) {
                                    if (files[file].IndexOf(names[season, episode, variance], StringComparison.OrdinalIgnoreCase) >= 0 & files[file].Contains(fileExtension[ext])) {
                                        for (int i = 0; ; i++) {
                                            if (Int32.Parse(Properties.Settings.Default["Danger"].ToString()) == 1) {
                                                try {
                                                    if (File.Exists(path(season, episode, files[file], number))) {
                                                        number++;
                                                        if (!File.Exists(path(season, episode, files[file], number))) {
                                                            File.Move(files[file], path(season, episode, files[file], number));
                                                            number = 0;
                                                            break;
                                                        }
                                                    } else { File.Move(files[file], path(season, episode, files[file], number)); break; }
                                                } catch (IOException) { MessageBox.Show("File " + files[file] + " is currently in use and it WILL BE SKIPPED!"); break; }
                                            } else {
                                                if (File.Exists(path(season, episode, files[file], number))) {
                                                    number++;
                                                    if (!File.Exists(path(season, episode, files[file], number))) {
                                                        File.Move(files[file], path(season, episode, files[file], number));
                                                        number = 0;
                                                        break;
                                                    }
                                                } else { File.Move(files[file], path(season, episode, files[file], number)); break; }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private string path(int season, int episode, string name, int number) {
            string path = null;
            if (season < 10) {
                path = location + "\\" + "Season " + "0" + (season + 1) + "\\" + number + System.IO.Path.GetFileName(name);
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }
            if (season >= 10) {
                path = location + "\\" + "Season " + (season + 1) + "\\" + number + System.IO.Path.GetFileName(name);
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }
            return path;
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

        private void rename() {
            string[] folders = new string[50];
            string[] files = new string[200];
            string name;
            folders = Directory.GetDirectories(location, "Season???");
            for (int lenght = 0; lenght < folders.Length; lenght++) {
                files = System.IO.Directory.GetFiles(folders[lenght], ".", SearchOption.AllDirectories);
                for (int file = 0; file < files.Count(); file++) {
                    for (int season = 0; season < namesCount; season++) {
                        for (int episode = 0; episode < namesCount; episode++) {
                            for (int variance = 0; variance <= 10; variance++) {
                                if (names[season, episode, variance] != null) {
                                    for (int ext = 0; ext < fileExtension.Count(); ext++) {
                                        if (files[file].IndexOf(names[season, episode, variance], StringComparison.OrdinalIgnoreCase) >= 0 & files[file].Contains(fileExtension[ext])) {
                                            name = getName(season + 1, episode + 1, showName);
                                            if (Path.GetFileNameWithoutExtension(files[file]) != name) {
                                                if (File.Exists(pathMove(season, name) + fileExtension[ext])) {
                                                    for (int i = 1; ; i++) {
                                                        if (!File.Exists(Path.GetDirectoryName(files[file]) + "\\" + name + "_" + i + fileExtension[ext])) {
                                                            try {
                                                                File.Move(files[file], pathMove(season, name) + "_" + i + fileExtension[ext]);
                                                            } catch (FileNotFoundException) { }
                                                            break;
                                                        }
                                                    }
                                                } else { File.Move(files[file], pathMove(season, name) + fileExtension[ext]); break; }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private string pathMove(int season, string name) {
            string path = null;
            if (season < 10) {
                path = location + "\\" + "Season " + "0" + (season + 1) + "\\" + name;
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }
            if (season >= 10) {
                path = location + "\\" + "Season " + (season + 1) + "\\" + name;
                if (Directory.Exists(Path.GetDirectoryName(path)) == false) {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
            }
            return path;
        }

        private string getName(int season, int episode, string showName) {
            string name = null;
            string final = null;
            string info = DownloadFromApi.apiGet("https://api.thetvdb.com/series/" + TVID + "/episodes/query?airedSeason=" + season + "&airedEpisode=" + episode, token,0);
            JObject parse = JObject.Parse(info);
            name = parse["data"][0]["episodeName"].ToString();
            string invalid = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
            foreach (char znak in invalid) {
                name = name.Replace(znak.ToString(), "");
            }
            if (season < 10) {
                if (episode < 10) { final = showName + " - S0" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { final = showName + " - S0" + season + "E" + episode + " - " + name; }
            } else if (season < 10) {
                if (episode < 10) { final = showName + " - S" + season + "E0" + episode + " - " + name; }
                if (episode >= 10) { final = showName + " - S" + season + "E" + episode + " - " + name; ; }
            }
            return final;
        }
        //DOPSAT!!! KLINGI JE KOKOT

        private void Start_Click(object sender, RoutedEventArgs e) {
            defLoc = location;
            generateSearch();
            bool success = true; 
            if (settings1 != null || settings2 != null || settings3 != null || settings1 != "" || settings2 != "" || settings3 != "") {
                success = search();
            }
            if (success == true) {
                int numberofFiles = 0;
                if (Int32.Parse(Properties.Settings.Default["dbdown"].ToString()) == 1) { rename(); } else {
                    try {
                        numberofFiles = System.IO.Directory.GetFiles(defLoc, "*.*", System.IO.SearchOption.AllDirectories).Length;
                    } catch (ArgumentException) { }
                    string[] files = new string[numberofFiles];
                    try {
                        files = System.IO.Directory.GetFiles(defLoc, "*.*", System.IO.SearchOption.AllDirectories);
                    } catch (ArgumentException) { }
                    move(files);
                    rename();
                    MessageBox.Show("Files that were found were probably renamed!", "Done");
                }
            } else { MessageBox.Show("Nothing was renamed since limit was too damn high", "Fail"); }
            
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

        private void comboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e) {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            string value = comboBox.SelectedItem as string;
            if (value == "TV Show folder") {
                Properties.Settings.Default["dbdown"] = 1;
                Properties.Settings.Default.Save();
            } else {
                Properties.Settings.Default["dbdown"] = 0;
                Properties.Settings.Default.Save();
            }
        }

        private void specific_Click(object sender, RoutedEventArgs e) {
            var window = new specificEP("Kappa") { Owner = this };
            window.ShowDialog();
            
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            string info = DownloadFromApi.apiGet("https://api.thetvdb.com/search/series?name=The%20100", token, 1);
            if (info == null) {
                token = DownloadFromApi.getToken();
                if (token == null) {
                    MessageBox.Show("You are either not connected to the internet or API is not responding. TVS Renamer will now close!");
                    this.Close();
                } else {
                    Properties.Settings.Default["Token"] = token;
                    Properties.Settings.Default.Save();
                }
            }
        }
    }
}