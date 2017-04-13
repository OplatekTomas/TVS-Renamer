using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Process.Start("https://github.com/Kaharonus/TVS-Renamer");   
        }

        private void Info_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Page p = new Info();
            Window main = Window.GetWindow(this);
            ((MainWindow)main).AddTempFrame(p);
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
                    sr.Details.MouseLeftButtonUp += (se, e) => ShowDetails(show);
                    sr.Selected.MouseLeftButtonUp += (se, e) => NextPage(show);
                    panel.Children.Add(sr);
                }), DispatcherPriority.Send);
            }
        }
        private void ShowDetails(Show s) {
            Page p = new Details(s);
            Window main = Window.GetWindow(this);
            ((MainWindow)main).AddTempFrame(p);
        }

        private void NextPage(Show s) {
            Page p = new Locations(s);
            Window main = Window.GetWindow(this);
            ((MainWindow)main).SetFrame(p);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) {
            TextBox.Text = "";
            TextBox.Foreground = new SolidColorBrush(Colors.White);
            TextBox.GotFocus -= TextBox_GotFocus;
        }
    }
}
