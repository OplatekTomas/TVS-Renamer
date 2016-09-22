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
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Drawing;
namespace TVSRenamer {
    /// <summary>
    /// Interaction logic for MoreInfo.xaml
    /// </summary>
    public partial class MoreInfo : Window {
        public MoreInfo(string showInfo) {
            InitializeComponent();
            info = showInfo;
            parse();
        }
        string info;
        public void parse() {
            setBanner();
            JObject aired = JObject.Parse(info);
            try {
                onair.Text = aired["firstAired"].ToString();
            } catch (NullReferenceException) { }
            JObject networkAir = JObject.Parse(info);
            try {
                network.Text = networkAir["network"].ToString();
            } catch (NullReferenceException) { }
            JObject overview2 = JObject.Parse(info);
            try {
                overview.Text = overview2["overview"].ToString();
            } catch (NullReferenceException) { }
            JObject seriesName = JObject.Parse(info);
            try {
                name.Text = seriesName["seriesName"].ToString();
            } catch (NullReferenceException) { }
            JObject status = JObject.Parse(info);
            try {
                statusNow.Text = status["status"].ToString();
            } catch (NullReferenceException) { }
        }

        private void setBanner() {
            JObject banner = JObject.Parse(info);
            var bannerPic = banner["banner"];
            WebClient wc = new WebClient();
            try {
                using (MemoryStream stream = new MemoryStream(wc.DownloadData("http://thetvdb.com/banners/" + bannerPic.ToString()))) {
                    var imageSource = new BitmapImage();
                    System.Drawing.Image img = System.Drawing.Image.FromStream(stream);
                    image.Source = GetImageStream(img);
                }
            } catch (WebException) { }
        }
        public static BitmapSource GetImageStream(System.Drawing.Image myImage) {
            var bitmap = new Bitmap(myImage);
            IntPtr bmpPt = bitmap.GetHbitmap();
            BitmapSource bitmapSource =
             System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                   bmpPt,
                   IntPtr.Zero,
                   Int32Rect.Empty,
                   BitmapSizeOptions.FromEmptyOptions());
            return bitmapSource;
        }

        private void overview_TextChanged(object sender, TextChangedEventArgs e) {
           
            
        }
    }
}
