using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for OknoSettings.xaml
    /// </summary>
    public partial class OknoSettings : Window {
        public OknoSettings() {
            
            InitializeComponent();

            try { textBox.Text = Properties.Settings.Default["Lokace1"].ToString();} catch (NullReferenceException) { }
            try { textBox1.Text = Properties.Settings.Default["Lokace2"].ToString();} catch (NullReferenceException) { }
            try { textBox2.Text = Properties.Settings.Default["Lokace3"].ToString(); } catch (NullReferenceException) { }
            
            


        }
        FolderBrowserDialog fbd1 = new FolderBrowserDialog();
        FolderBrowserDialog fbd2 = new FolderBrowserDialog();
        FolderBrowserDialog fbd3 = new FolderBrowserDialog();

        int saved=0;
        string location1;
        string location2;
        string location3;



        private void save() {
  
            Properties.Settings.Default["Lokace1"] = location1;
            Properties.Settings.Default.Save();
            Properties.Settings.Default["Lokace2"] = location2;
            Properties.Settings.Default.Save();
            Properties.Settings.Default["Lokace3"] = location3;
            Properties.Settings.Default.Save();
            if (checkBox.IsChecked == true) {
                Properties.Settings.Default["Danger"] = 1;
                Properties.Settings.Default.Save();
            } else {
                Properties.Settings.Default["Danger"] = 0;
                Properties.Settings.Default.Save();
            }
            this.Close();
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e) {
            location1 = textBox.Text;
            if (Directory.Exists(location1) || location1 == null) {
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else if (location1 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else {
                textBox.Background = Brushes.Tomato;
                button1.IsEnabled = false;
                Close.IsEnabled = false;
            }
           
        }
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e) {
            location2 = textBox1.Text;
            if (Directory.Exists(location2) || location2 == null) {
                textBox1.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else if (location2 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else {
                textBox1.Background = Brushes.Tomato;
                button1.IsEnabled = false;
                Close.IsEnabled = false;
            }
            
        }
        private void textBox2_TextChanged(object sender, TextChangedEventArgs e) {
            location3 = textBox2.Text;
            if (Directory.Exists(location3) || location3 == null) {
                textBox2.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else if (location3 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Close.IsEnabled = true;
            } else {
                textBox2.Background = Brushes.Tomato;
                button1.IsEnabled = false;
                Close.IsEnabled = false;
            }
            
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            fbd1.ShowDialog();
            textBox.Text = fbd1.SelectedPath;
            location1 = fbd1.SelectedPath;
        }
    
        private void button_Copy_Click(object sender, RoutedEventArgs e) {
            fbd2.ShowDialog();
            textBox1.Text = fbd2.SelectedPath;
            location2 = fbd2.SelectedPath;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e) {
            fbd3.ShowDialog();
            textBox2.Text = fbd3.SelectedPath;
            location3 = fbd3.SelectedPath;
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            save();
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Do you want to save before closing?", "File Save", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                save();
            } else this.Close();



        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("Danger Mode - Makes this app much much faster, but if any of files that you want to move/rename are in use THE APP WILL CRASH! (your files will be fine)\n\nEnter location of folders that will be searched for episodes/subtitles.\nThis can be useful when you search for subtitles in your \"Downloads\" folder\n\nPLEASE BE SPECIFIC!\nEnter folders like: \"D:\\Downloads\\\"\" instaed of just \"D:\\\"","help");
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e) {
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            if (Int32.Parse(Properties.Settings.Default["Danger"].ToString()) == 1) {
                checkBox.IsChecked = true;
            } else {
                checkBox.IsChecked = false;
            }
        }
    }
}
