using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.Windows.Forms;
using System.IO;
using System.Windows.Threading;

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for Locations.xaml
    /// </summary>
    public partial class Locations : Page {
        public Locations(Show show) {
            InitializeComponent();
            this.show = show;
        }
        Show show;
        private void GitHub_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Process.Start("https://github.com/Kaharonus/TVS-Renamer");
        }

        private void Info_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

        }

        private void Back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Page p = new SelectShow();
            Window main = Window.GetWindow(this);
            ((MainWindow)main).SetFrame(p);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            ShowName.Text = show.name;
            Loc1.Text = Properties.Settings.Default.Lokace1;
            Loc2.Text = Properties.Settings.Default.Lokace2;
            Loc3.Text = Properties.Settings.Default.Lokace3;
        }

        private void StartButton_MouseUp(object sender, MouseButtonEventArgs e) {
            ShowWaiting();
            string text = TextBox.Text;
            if (CheckFolders()) {
                List<string> l = new List<string>();
                l.Add(Loc1.Text);
                l.Add(Loc2.Text);
                l.Add(Loc3.Text);
                Action a = null;
                a = () => Renamer.RenameBatch(l, text, show);                            
                IAsyncResult ar = a.BeginInvoke(Callback, null);               
            } else {
                System.Windows.MessageBox.Show("One of the paths you entered doesn't exist");
                HideWaiting();
            }
        }
        public void Callback(IAsyncResult result) {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => HideWaiting()));
        }
        private bool CheckFolders() {
            bool check1 = false;
            bool check2 = false;
            bool check3 = false;
            bool check4 = false;
            if (Directory.Exists(TextBox.Text)) {
                check1 = true;
            }
            if (Directory.Exists(Loc1.Text) || Loc1.Text == "") {
                check2 = true;                
            }
            if (Directory.Exists(Loc2.Text) || Loc2.Text == "") {
                check3 = true;
            }
            if (Directory.Exists(Loc3.Text) || Loc3.Text == "") {
                check4 = true;
            }
            if (check1 && check2 && check3 && check4) {
                Properties.Settings.Default.Lokace1 = Loc1.Text;
                Properties.Settings.Default.Lokace2 = Loc2.Text;
                Properties.Settings.Default.Lokace3 = Loc3.Text;
                Properties.Settings.Default.Save();
                return true;
            } else {
                return false;
            }

        }

        private void SelectLocation_MouseUp(object sender, MouseButtonEventArgs e) {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.ShowDialog();
            if (vfbd.SelectedPath != "") {
                TextBox.Text = vfbd.SelectedPath;
            }
        }

        private void Loc1Select_MouseUp(object sender, MouseButtonEventArgs e) {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.ShowDialog();
            if (vfbd.SelectedPath != "") {
                Loc1.Text = vfbd.SelectedPath;
            }
        }
        private void Loc2Select_MouseUp(object sender, MouseButtonEventArgs e) {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.ShowDialog();
            if (vfbd.SelectedPath != "") {
                Loc2.Text = vfbd.SelectedPath;
            }
        }
        private void Loc3Select_MouseUp(object sender, MouseButtonEventArgs e) {
            VistaFolderBrowserDialog vfbd = new VistaFolderBrowserDialog();
            vfbd.ShowDialog();
            if (vfbd.SelectedPath != "") {
                Loc3.Text = vfbd.SelectedPath;
            }
        }
        private void ShowWaiting() { 
            ShowHideMenu("ShowBottom", Waiting);
        }

        private void HideWaiting() {
            ShowHideMenu("HideBottom", Waiting);
        }

        private void ShowHideMenu(string Storyboard, Grid pnl) {
            System.Windows.Media.Animation.Storyboard sb = Resources[Storyboard] as System.Windows.Media.Animation.Storyboard;
            sb.Begin(pnl);
        }

    }
}
