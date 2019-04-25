using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using IconFontDemo.CustomerPackIcons;
using MahApps.Metro.IconPacks;

namespace IconFontDemo

{
    /// <summary>
    /// PackIconMaterials.xaml 的交互逻辑
    /// https://github.com/MahApps/MahApps.Metro.IconPacks
    /// </summary>
    public partial class PackIconMaterials
    {
        private Stopwatch _stopwatch;

        public PackIconMaterials(int count)
        {
            InitializeComponent();

            RecordStart();
            CreateImages(count);
        }

        private void CreateImages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Container.Children.Add(new CustomerPackIcon
                {
                    Kind = CustomerPackIconKind.Gear,
                    Foreground = Brushes.Red,
                    Width = 100,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                });

                //Container.Children.Add(new PackIconMaterial
                //{
                //    Kind = PackIconMaterialKind.Blur,
                //    Foreground = Brushes.Red,
                //    Width = 100,
                //    Height = 100,
                //    HorizontalAlignment = HorizontalAlignment.Center,
                //    VerticalAlignment = VerticalAlignment.Center
                //});
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
