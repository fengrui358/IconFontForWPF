﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace IconFontDemo
{
    /// <summary>
    /// Paths.xaml 的交互逻辑
    /// </summary>
    public partial class Paths
    {
        private Stopwatch _stopwatch;

        public string Data => "M911.945421 589.396992l-62.497894-36.07767c-17.62304-10.177843-40.14592-4.144435-50.315981 13.476966-10.177229 17.622323-4.143514 40.145306 13.469286 50.324173l62.422221 36.033638c-30.075187 43.229594-67.660698 80.808448-110.886912 110.890598l-36.04951-62.414541c-10.171085-17.621299-32.692941-23.654707-50.315981-13.486182-17.625088 10.177843-23.656755 32.700723-13.479526 50.322125L700.379443 800.962458c-46.631936 22.00105-97.723597 36.02135-151.551693 40.466637l0-72.03564c0-20.337152-16.480973-36.835942-36.837478-36.835942-20.336947 0-36.836454 16.49879-36.836454 36.835942L475.153818 841.43015c-53.817856-4.44631-104.912589-18.465587-151.533261-40.461517l36.089344-62.502502c10.170061-17.622323 4.134298-40.144282-13.488742-50.322125-17.611776-10.168627-40.14592-4.135219-50.325197 13.486182l-36.032102 62.414541c-43.23543-30.08215-80.811725-67.666125-110.89408-110.890598l62.410957-36.033638c17.624064-10.178867 23.656755-32.702771 13.488742-50.324173-10.170061-17.621299-32.703181-23.655731-50.327245-13.476966l-62.497894 36.071526c-26.967245-57.152717-42.104627-120.981402-42.104627-188.367155 0-244.134298 197.907354-442.041651 442.05056-442.041651 244.153446 0 442.066944 197.905408 442.066944 442.041651C954.05824 468.413542 938.919834 532.244275 911.945421 589.396992zM767.225549 253.680947c-81.385882-140.943053-261.623091-189.244109-402.584166-107.86775-140.962099 81.382502-189.253939 261.618483-107.864986 402.564608 81.37769 140.946125 261.624115 189.245133 402.584166 107.869798C800.302285 574.86295 848.603238 394.627072 767.225549 253.680947zM667.258778 590.457139 562.319565 413.687091c-10.170061-17.611059-32.694989-23.654707-50.325197-13.487206-17.6128 10.178867-23.655731 32.710963-13.478502 50.324173L603.857306 627.959296c-28.383539 11.459994-59.370598 17.823949-91.862938 17.823949-135.626342 0-245.582029-109.951181-245.582029-245.583258 0-135.619789 109.955686-245.571994 245.582029-245.571994 135.644774 0 245.593293 109.950157 245.593293 245.571994C757.587661 476.928512 722.381414 545.418342 667.258778 590.457139z";

        public Paths(int count)
        {
            InitializeComponent();

            RecordStart();
            CreateImages(count);
        }

        private void CreateImages(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var path = new Path
                {
                    Fill = Brushes.Red,
                    Stretch = Stretch.Fill,
                    Width = 100,
                    Height = 100,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                var pathDataBinding = new Binding();
                pathDataBinding.Source = this;
                pathDataBinding.Path = new PropertyPath(nameof(Data));
                pathDataBinding.Mode = BindingMode.OneWay;

                path.SetBinding(Path.DataProperty, pathDataBinding);

                Container.Children.Add(path);
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
