using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IconFontDemo.CustomerPackIcons;
using IconFontDemo.IconFontWpfs;
using IconFontWpf = IconFontDemo.IconFontWpfs.IconFontWpf;

namespace IconFontDemo
{
    /// <summary>
    /// FontImages.xaml 的交互逻辑
    /// 字体图片集合
    /// </summary>
    public partial class IconFont
    {
        private Stopwatch _stopwatch;

        public IconFont(int count)
        {
            InitializeComponent();

            GC.Collect();

            RecordStart();
            CreateImages(count);
        }

        private void CreateImages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Container.Children.Add(new IconFontWpfs.IconFontWpf
                {
                    Kind = IconFontKind.Gear,
                    Foreground = Brushes.Yellow,
                    Width = 100,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });
            }
        }

        private void RecordStart()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            _stopwatch.Stop();
            Msg.Text = $"耗时：{_stopwatch.ElapsedMilliseconds}ms";
        }
    }
}
