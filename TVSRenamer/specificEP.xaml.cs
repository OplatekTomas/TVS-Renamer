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
    /// Interaction logic for specificEP.xaml
    /// </summary>
    public partial class specificEP : Window {
        public specificEP(string folder) {
            string path = folder;
            InitializeComponent();   
        }
        private void textBox_TextChanged(object sender, TextChangedEventArgs e) {

        }

        private void textBox2_TextChanged(object sender, TextChangedEventArgs e) {

        }
    }
}
