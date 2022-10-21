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
        }
    }
}
