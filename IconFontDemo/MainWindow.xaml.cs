using System.Windows;
using System.Windows.Controls;

namespace IconFontDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public int Count { get; set; }

        public MainWindow()
        {
            Count = 3000;
            InitializeComponent();

            DataContext = this;
        }

        private void ShowWindow_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = (Button)e.Source;
            switch (btn.Content.ToString().ToUpperInvariant())
            {
                case "PNG":
                    new PngImages(Count) { Owner = this }.ShowDialog();
                    break;
                case "SVG":
                    new SvgImages(Count) { Owner = this }.ShowDialog();
                    break;
                case "PATHIMAGES":
                    new PathImages(Count) { Owner = this }.ShowDialog();
                    break;
                case "FONT":
                    new FontImages(Count) { Owner = this }.ShowDialog();
                    break;
                case "PACKICONMATERIALS":
                    new PackIconMaterials(Count) { Owner = this }.ShowDialog();
                    break;
                case "ICONFONT":
                    new IconFont(Count) { Owner = this }.ShowDialog();
                    break;
                case "ICONFONTBUTTON":
                    new IconFontButton(Count) { Owner = this }.ShowDialog();
                    break;
            }
        }
    }
}
