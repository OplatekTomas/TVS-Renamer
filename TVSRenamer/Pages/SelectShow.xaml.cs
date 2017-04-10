using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TVSRenamer
{
    /// <summary>
    /// Interaction logic for SelectShow.xaml
    /// </summary>
    public partial class SelectShow : Page
    {
        public SelectShow()
        {
            InitializeComponent();
        }

        private void GitHub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            API.getShows("Game of Thrones");
        }

        private void Info_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            string text = TextBox.Text;
            Action a = () => FillUI(text);
            Thread t = new Thread(a.Invoke);
            t.IsBackground = true;
            t.Name = "Search";
            t.Start();             
        }
        private void FillUI(string name) {
           
            List<Show> s = API.getShows(name);
            if (s.Count > 0) {
                Dispatcher.Invoke(new Action(() => {
                    panel.Children.Clear();              
                }), DispatcherPriority.Send);
            }
            foreach (Show show in s) {
                Dispatcher.Invoke(new Action(() => {
                    SearchResult sr = new SearchResult();
                    sr.Height = 30;
                    sr.MainText.Text = show.name;
                    panel.Children.Add(sr);
                }), DispatcherPriority.Send);
            }
        }

    }
}
