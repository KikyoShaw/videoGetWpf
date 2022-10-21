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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace videoGet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;

            if(string.IsNullOrEmpty(text))
                return;

            //微视
            var vm = WeiShiGet.Instance;
            vm.InitUrl(text);
            var url = vm.GetMp4();

            //抖音
            //var vm = DouYinGet.Instance;
            //vm.InitUrl(text);
            //var url = vm.GetVideoUrl();

            //快手
            //var vm = KuaiShouGet.Instance;
            //vm.InitUrl(text);
            //var url = vm.GetMp4();

            //西瓜视频
            //var vm = XiGuaGet.Instance;
            //vm.InitUrl(text);
            //var url = vm.Get1080Mp4();

            if (!string.IsNullOrEmpty(url))
            {
                this.Player.SourceStr = url;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Player.SourceStr = @"http://v.weishi.qq.com/v.weishi.qq.com/gzc_8542_1047_0bc3w4buyaadeiagu3zzwzrrjnyejs3qgtca.f70.mp4?dis_k=c9911306a3279b023cc404b14128956c&dis_t=1666339506&fromtag=0&pver=8.79.1";
        }
    }
}
