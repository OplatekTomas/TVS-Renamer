using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            try { textBox.Text = Properties.Settings.Default.Lokace1; } catch (NullReferenceException) { }
            try { textBox1.Text = Properties.Settings.Default.Lokace2; } catch (NullReferenceException) { }
            try { textBox2.Text = Properties.Settings.Default.Lokace3; } catch (NullReferenceException) { }
        }
        FolderBrowserDialog fbd1 = new FolderBrowserDialog();
        FolderBrowserDialog fbd2 = new FolderBrowserDialog();
        FolderBrowserDialog fbd3 = new FolderBrowserDialog();
        string location1;
        string location2;
        string location3;

        private void textBox_TextChanged(object sender, TextChangedEventArgs e) {
            location1 = textBox.Text;
            if (Directory.Exists(location1) || location1 == null) {
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace1"] = location1;
                Properties.Settings.Default.Save();
            } else if (location1 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace1"] = location1;
                Properties.Settings.Default.Save();
            } else {
                textBox.Background = Brushes.Tomato;
                button1.IsEnabled = false;
            }
           
        }
        private void textBox1_TextChanged(object sender, TextChangedEventArgs e) {
            location2 = textBox1.Text;
            if (Directory.Exists(location2) || location2 == null) {
                textBox1.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace2"] = location2;
                Properties.Settings.Default.Save();
            } else if (location2 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace2"] = location2;
                Properties.Settings.Default.Save();
            } else {
                textBox1.Background = Brushes.Tomato;
                button1.IsEnabled = false;
            }
            
        }
        private void textBox2_TextChanged(object sender, TextChangedEventArgs e) {
            location3 = textBox2.Text;
            if (Directory.Exists(location3) || location3 == null) {
                textBox2.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace3"] = location3;
                Properties.Settings.Default.Save();
            } else if (location3 == "") {
                location1 = null;
                textBox.Background = Brushes.Green;
                button1.IsEnabled = true;
                Properties.Settings.Default["Lokace3"] = location3;
                Properties.Settings.Default.Save();
            } else {
                textBox2.Background = Brushes.Tomato;
                button1.IsEnabled = false;
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
            this.Close();
        }


        private void button2_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("3 Folder selctors\nEnter location of folders that will be searched for episodes or subtitles. For example your \"Downloads\" folder\nPLEASE BE SPECIFIC!\nEnter folders like: \"D:\\Downloads\\\"\" instaed of just \"D:\\\"\n\nFiles option\nSelect Generic folder if want to rename episodes that are for example in \"Downloads\" and there might be more then 1 TV show in that folder. Select Specific Folder if there are files from only 1 TV show in selected folder.\nThis applies to location of folder you want to rename NOT to those 3 options in this window\n\nDelete folder option\nuse this if you want to delete folder where your files were - useful when downloading TV Shows using torrent - use with caution \n\nDanger Mode\nMakes this app faster, but if any of files that you want to move / rename are in use THE APP WILL CRASH!(your files will be fine)\n\n", "Help");
        }

        private void checkBox_Checked(object sender, RoutedEventArgs e) {
            if (checkBox.IsChecked == true) {
                Properties.Settings.Default["Danger"] = 1;
                Properties.Settings.Default.Save();
            } else {
                Properties.Settings.Default["Danger"] = 0;
                Properties.Settings.Default.Save();
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e) {
            if (Properties.Settings.Default.Danger == 1) {
                checkBox.IsChecked = true;
            } else {
                checkBox.IsChecked = false;
            }
            if(Properties.Settings.Default.Delete == true) {
                checkDelete.IsChecked = true;
            } else {
                checkDelete.IsChecked = false;
            }
            enterMB.Text = Properties.Settings.Default.maxSize.ToString();
        }

        private void generic_Selected(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.Folder = 0;
            Properties.Settings.Default.Save();
        }

        private void specific_Selected(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.Folder = 1;
            Properties.Settings.Default.Save();

        }

        private void comboBox_Loaded(object sender, RoutedEventArgs e) {
            if (Properties.Settings.Default.Folder == 1) {
                comboBox.SelectedIndex = 1;
            } else { comboBox.SelectedIndex = 0; }
           
        }

        private void checkDelete_Checked(object sender, RoutedEventArgs e) {
            if (checkDelete.IsChecked == true) {
                Properties.Settings.Default.Delete = true;
                Properties.Settings.Default.Save();
            } else {
                Properties.Settings.Default.Delete = false;
                Properties.Settings.Default.Save();
            }
        }

        private void enterMB_Changed(object sender, TextChangedEventArgs e) {
            Regex regex = new Regex(@"^\d{1,2}$");
            if (!regex.IsMatch(enterMB.Text)) {
                MessageBox.Show("Enter valid number!");
                enterMB.TextChanged -= enterMB_Changed;
                enterMB.Text = null;
                enterMB.TextChanged += enterMB_Changed;

            } else {
                Properties.Settings.Default.maxSize = Int32.Parse(enterMB.Text);
                Properties.Settings.Default.Save();
            }

        }
    }
}
