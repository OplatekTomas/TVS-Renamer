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
            MessageBox.Show("v0.4 You can now rename specific files/episodes instead of whole folders, bug fixes (huge ones). Last major beta release. Bugfixes may come but nothing huge is on the way.\nv0.3 Added \"Select show\" feature, ability to search in downloads folder instead of just a database, slight UI change, bug fixes.\nv0.2.1 Huge bug fix, improved file searching algorithm, small code changes\nv0.2 API change, added function to search for files, ability to search using IMDb ID, MUCH MUCH better code, +- metric ton of bug fixes\n v0.1 Initial release!", "Changelog");
        }

        private void button1_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
