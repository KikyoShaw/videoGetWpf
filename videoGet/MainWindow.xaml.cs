using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Mpv.NET.Player;
using videoGet.ViewModel;

namespace videoGet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MpvPlayer _player;

        public MainWindow()
        {
            InitializeComponent();
            _player = new MpvPlayer(PlayerHost.Handle)
            {
                Loop = true,
                Volume = 90
            };
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;

            if (string.IsNullOrEmpty(text))
                return;

            var url = GetVideoUrl(text);
            MpvPlay(url);
        }

        private string GetVideoUrl(string url)
        {
            string videoUrl = "";
            var type = this.VideoComboBox.Text;
            if (type == this.ComboBoxItem1.Content.ToString())
            {
                videoUrl = url;
                this.VideoName.Text = url;
            }
            else if (type == this.ComboBoxItem2.Content.ToString())
            {
                var vm = DouYinGet.Instance;
                vm.InitUrl(url);
                var name = vm.GetTitle();
                this.VideoName.Text = name;
                videoUrl = vm.GetVideoUrl();
            }
            else if (type == this.ComboBoxItem3.Content.ToString())
            {
                var vm = KuaiShouGet.Instance;
                vm.InitUrl(url);
                var name = vm.GetTitle();
                this.VideoName.Text = name;
                videoUrl = vm.GetMp4();
            }
            else if (type == this.ComboBoxItem4.Content.ToString())
            {

            }
            else if (type == this.ComboBoxItem5.Content.ToString())
            {
                var vm = XiGuaGet.Instance;
                vm.InitUrl(url);
                var name = vm.GetTitle();
                this.VideoName.Text = name;
                videoUrl = vm.Get1080Mp4();
            }
            else if (type == this.ComboBoxItem6.Content.ToString())
            {
                var vm = WeiShiGet.Instance;
                vm.InitUrl(url);
                var name = vm.GetTitle();
                this.VideoName.Text = name;
                videoUrl = vm.GetMp4();
            }
            else if (type == this.ComboBoxItem7.Content.ToString())
            {
                var vm = KuaiShouGet.Instance;
                vm.InitUrl(url);
                var name = vm.GetTitle();
                this.VideoName.Text = name;
                videoUrl = vm.GetMp4();
            }
            return videoUrl;
        }

        private void MpvPlay(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (_player.IsPlaying)
                    _player.Stop();
                _player.Load(url);
                _player.Resume();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this.Player.SourceStr = @"http://v.weishi.qq.com/v.weishi.qq.com/gzc_8542_1047_0bc3w4buyaadeiagu3zzwzrrjnyejs3qgtca.f70.mp4?dis_k=c9911306a3279b023cc404b14128956c&dis_t=1666339506&fromtag=0&pver=8.79.1";
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _player.Dispose();
        }
    }
}
