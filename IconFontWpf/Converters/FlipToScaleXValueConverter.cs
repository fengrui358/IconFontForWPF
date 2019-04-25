using System;
using System.Globalization;
using System.Windows;

namespace IconFontWpf.Converters
{
    /// <summary>
    /// ValueConverter which converts the PackIconFlipOrientation enumeration value to ScaleX value of a ScaleTransformation.
    /// </summary>
    /// <seealso cref="T:MahApps.Metro.IconPacks.Converter.MarkupConverter" />
    public class FlipToScaleXValueConverter : MarkupConverter
    {
        private static FlipToScaleXValueConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return (object) FlipToScaleXValueConverter._instance ??
                   (object) (FlipToScaleXValueConverter._instance = new FlipToScaleXValueConverter());
        }

        protected override object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (!(value is IconFontFlipOrientation))
                return DependencyProperty.UnsetValue;
            int num;
            switch ((IconFontFlipOrientation) value)
            {
                case IconFontFlipOrientation.Horizontal:
                case IconFontFlipOrientation.Both:
                    num = -1;
                    break;
                default:
                    num = 1;
                    break;
            }

            return (object) num;
        }

        protected override object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}