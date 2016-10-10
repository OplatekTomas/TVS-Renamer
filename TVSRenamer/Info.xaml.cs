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

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window {
        public Window1() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            MessageBox.Show("v1.0 - Final version\nFolder can be deleted after files were moved from it. Code speed up! (Stuff that could take you minutes then takes couple of seconds now), Removed \"Found season\" - it never worked and was kinda useless. Max. number of files in folder increased. Moved database/downloads switch to settings. \n\nv0.4 \nYou can now rename specific files/episodes instead of whole folders, bug fixes (huge ones).\n\nv0.3\nAdded \"Select show\" feature, ability to search in downloads folder instead of just a database, slight UI change, bug fixes.\n\nv0.2.1\nHuge bug fix, improved file searching algorithm, small code changes\n\nv0.2\nAPI change (The TVDB), added function to search for files, ability to search using IMDb ID, MUCH MUCH better code, +- metric ton of bug fixes\n\nv0.1\nInitial release! (using TVMaze API)", "Changelog");
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
