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

            if(string.IsNullOrEmpty(text))
                return;

            _player.Load(text);
            _player.Resume();
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;
            if (string.IsNullOrEmpty(text))
                return;
            //抖音
            var vm = DouYinGet.Instance;
            vm.InitUrl(text);
            var name = vm.GetTitle();
            this.VideoName.Text = name;
            var url = vm.GetVideoUrl();
            MpvPlay(url);
        }

        private void ButtonBase3_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;
            if (string.IsNullOrEmpty(text))
                return;
            //微视
            var vm = WeiShiGet.Instance;
            vm.InitUrl(text);
            var name = vm.GetTitle();
            this.VideoName.Text = name;
            var url = vm.GetMp4();
            MpvPlay(url);
        }

        private void ButtonBase4_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;
            if (string.IsNullOrEmpty(text))
                return;
            //西瓜视频
            var vm = XiGuaGet.Instance;
            vm.InitUrl(text);
            var name = vm.GetTitle();
            this.VideoName.Text = name;
            var url = vm.Get1080Mp4();
            MpvPlay(url);
        }

        private void ButtonBase5_OnClick(object sender, RoutedEventArgs e)
        {
            var text = UrlTextBox.Text;
            if (string.IsNullOrEmpty(text))
                return;
            //微视
            var vm = WeiShiGet.Instance;
            vm.InitUrl(text);
            var name = vm.GetTitle();
            this.VideoName.Text = name;
            var url = vm.GetMp4();
            MpvPlay(url);
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
