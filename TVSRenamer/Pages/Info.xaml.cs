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

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Page {
        public Info() {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            Window main = Window.GetWindow(this);
            ((MainWindow)main).CloseTempFrame();
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.thetvdb.com/");
        }

        private void TextBlock_MouseUp_1(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.tvmaze.com");
        }

        private void TextBlock_MouseUp_2(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.flaticon.com/authors/dave-gandy");
        }

        private void TextBlock_MouseUp_3(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.flaticon.com/authors/gregor-cresnar");
        }

        private void TextBlock_MouseUp_4(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.flaticon.com/authors/madebyoliver");
        }

        private void TextBlock_MouseUp_5(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.flaticon.com");
        }

        private void TextBlock_MouseUp_6(object sender, MouseButtonEventArgs e) {
            Process.Start("https://github.com/Kaharonus/TVS-Renamer/");           
        }

        private void TextBlock_MouseUp_7(object sender, MouseButtonEventArgs e) {
            Process.Start("https://github.com/TheNumerus/");

        }

        private void TextBlock_MouseUp_8(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.flaticon.com/authors/freepik");
        }
    }
}
