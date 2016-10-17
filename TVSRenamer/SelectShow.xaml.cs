using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for SelectShow.xaml
    /// </summary>

    public partial class SelectShow : Window {
        public SelectShow(string show) {
            info = show;
            InitializeComponent();
            generate();
        }
        string final;
        string info = null;
        string[] showInfo = new string[15];
        private void next(string value, string airDate, string infoShow, string id) {
            Grid polozka = new Grid();
            polozka.Height = 32;
            Seznam.Children.Add(polozka);
            Button confirm = new Button();
            Button info = new Button();
            TextBlock show = new TextBlock();
            TextBlock txtShow = new TextBlock();
            TextBlock txtAired = new TextBlock();
            TextBlock air = new TextBlock();
            //Selected button
            confirm.Margin = new Thickness(376, 10, 10, 2);
            confirm.Width = 75;
            confirm.Click += (s, e) => { selected(id); };
            confirm.Content = "Select";
            polozka.Children.Add(confirm);
            //Info button 
            info.Margin = new Thickness(296, 10, 0, 0);
            info.Content = "More Info";
            info.Click += (s, e) => { infoWin(infoShow); };
            info.Width = 75;
            info.HorizontalAlignment = HorizontalAlignment.Left;
            info.VerticalAlignment = VerticalAlignment.Top;
            polozka.Children.Add(info);
            //Show text block
            show.Margin = new Thickness(79, 0, 0, 0);
            show.HorizontalAlignment = HorizontalAlignment.Left;
            show.VerticalAlignment = VerticalAlignment.Top;
            show.Width = 212;
            show.Text = value;
            polozka.Children.Add(show);
            //Aired text block
            air.Margin = new Thickness(79, 16, 0, 0);
            air.HorizontalAlignment = HorizontalAlignment.Left;
            air.VerticalAlignment = VerticalAlignment.Top;
            show.Width = 212;
            air.Height = 16;
            air.Text = airDate;
            polozka.Children.Add(air);
            //txtShowName text block
            txtShow.Margin = new Thickness(10, 0, 0, 0);
            txtShow.HorizontalAlignment = HorizontalAlignment.Left;
            txtShow.VerticalAlignment = VerticalAlignment.Top;
            txtShow.Text = "Show name:";
            polozka.Children.Add(txtShow);
            //txtAir text block
            txtAired.Margin = new Thickness(10, 16, 0, 0);
            txtAired.HorizontalAlignment = HorizontalAlignment.Left;
            txtAired.VerticalAlignment = VerticalAlignment.Top;
            txtAired.Text = "First aired:";
            polozka.Children.Add(txtAired);
        }
        public void selected(string name) {
            this.Close();
            final = name;
        }
        public string Return
        {
            get { return final; }
        }
        private void infoWin(string infoShow) {
            var w = new MoreInfo(infoShow);
            w.ShowDialog();
        }

        struct Shows {
            public string specificInfo;
            public string showName;
            public DateTime date;
            public string id;
        }

        private void generate() {
            JObject parse = JObject.Parse(info);
            int numberOfShows = parse["data"].Count();
            Shows[] show = new Shows[numberOfShows];
            for (int i = 0; i < numberOfShows; i++) {
                show[i].specificInfo = parse["data"][i].ToString();
                show[i].showName = parse["data"][i]["seriesName"].ToString();
                show[i].date = DateTime.ParseExact(parse["data"][i]["firstAired"].ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                show[i].id = parse["data"][i]["id"].ToString();
            }
            Array.Sort<Shows>(show, (x, y) => x.date.CompareTo(y.date));
            Array.Reverse(show);
            for (int x = 0; x < numberOfShows; x++) {
                next(show[x].showName, show[x].date.ToString("dd.MM.yyyy"), show[x].specificInfo, show[x].id);
            }
            this.Height = (numberOfShows + 1) * 32 + 10;
            Seznam.Height = (numberOfShows + 1) * 32 + 10;            
        }
    }
}

