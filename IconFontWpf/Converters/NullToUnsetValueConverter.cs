using System;
using System.Globalization;
using System.Windows;

namespace IconFontWpf.Converters
{
    public class NullToUnsetValueConverter : MarkupConverter
    {
        private static NullToUnsetValueConverter _instance;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return (object) NullToUnsetValueConverter._instance ??
                   (object) (NullToUnsetValueConverter._instance = new NullToUnsetValueConverter());
        }

        protected override object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value ?? DependencyProperty.UnsetValue;
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