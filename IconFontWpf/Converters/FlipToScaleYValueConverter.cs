using System;
using System.Globalization;
using System.Windows;

namespace IconFontWpf.Converters
{
    /// <summary>
    /// ValueConverter which converts the PackIconFlipOrientation enumeration value to ScaleY value of a ScaleTransformation.
    /// </summary>
    /// <seealso cref="T:MahApps.Metro.IconPacks.Converter.MarkupConverter" />
    public class FlipToScaleYValueConverter : MarkupConverter
    {
        private static FlipToScaleYValueConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return (object) FlipToScaleYValueConverter._instance ??
                   (object) (FlipToScaleYValueConverter._instance = new FlipToScaleYValueConverter());
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
                case IconFontFlipOrientation.Vertical:
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