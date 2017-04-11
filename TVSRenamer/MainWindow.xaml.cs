using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using TVSRenamer;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using Panel = System.Windows.Controls.Panel;

namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private void MainFrame_Loaded(object sender, RoutedEventArgs e) {
            Page s = new SelectShow();
            MainFrame.Content = s;
        }
        public void AddTempFrame(Page page) {
            Frame fr = new Frame();
            BaseGrid.Children.Add(fr);
            Panel.SetZIndex(fr, 1000);
            fr.Content = page;
        }
        public void SetFrame(Page page) {
            MainFrame.Content = page;
        }
        public void CloseTempFrame() {
            BaseGrid.Children.RemoveAt(BaseGrid.Children.Count - 1);
        }
        
    }
}