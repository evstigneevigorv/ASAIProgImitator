using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ASAIProgImitator.Converters
{
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleToTimeSpanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            TimeSpan ts = new TimeSpan();
            ts = TimeSpan.FromMilliseconds((double)value);
            return ts.ToString();
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return (TimeSpan.Parse((string)value).TotalMilliseconds);
        }
    }

    [ValueConversion(typeof(Duration), typeof(double))]
    public class DurationToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            Duration d = (Duration)value;
            if (d.HasTimeSpan) return d.TimeSpan.Milliseconds;
            else return 0.0;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            Duration d = new Duration(TimeSpan.FromMilliseconds((double)value));
            return d;
        }
    }
}
