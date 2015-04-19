using System;
using System.Windows.Data;
using System.Windows.Media;

namespace BaseApp.Converters
{
    public class AlternateRowColour : IValueConverter
    {
        bool _isAlternate;
        SolidColorBrush even = new SolidColorBrush(Color.FromArgb(100, 241, 241, 251)); // Set these two brushes to your alternating background colours.
        SolidColorBrush odd = new SolidColorBrush(Color.FromArgb(100,241,241,251));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            _isAlternate = !_isAlternate;
            return _isAlternate ? even : odd;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
