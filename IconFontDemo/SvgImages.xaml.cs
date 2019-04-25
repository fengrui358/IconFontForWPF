﻿using System;
using System.Diagnostics;
using System.Windows;
using ClipArtViewer;

namespace IconFontDemo
{
    /// <summary>
    /// SvgImages.xaml 的交互逻辑
    /// Svg图片集合
    /// </summary>
    public partial class SvgImages
    {
        private Stopwatch _stopwatch;

        public SvgImages(int count)
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
                var svgRender = new SVGRender();
                var image = svgRender.LoadDrawing("Resources/timer.svg");

                Container.Children.Add(new SVGImage
                {
                    ImageSource = image,
                    SizeType = SVGImage.eSizeType.SizeToContent,
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
