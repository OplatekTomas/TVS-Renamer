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

namespace TVSRenamer
{
    /// <summary>
    /// Interaction logic for ProgressBarWindow.xaml
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        public ProgressBarWindow()
        {
            InitializeComponent();
        }

        private void bar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (bar.Value == 100) {
                MessageBox.Show("Files that were found were probably renamed!");
                this.Close();
            }
        }
    }
}
