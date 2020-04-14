using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;

namespace ASAIProgImitator
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IndexToIsSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            if ((int)value == -1) return false;
            else return true;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return 0;
        }
    }

    [ValueConversion(typeof(RLSType), typeof(int))]
    public class RLSTypeToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            //if      ((RLSType)value == RLSType.PRL) return 0;
            //else if ((RLSType)value == RLSType.VRL) return 1;
            //else                                    return 2;
            return 0;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            //if      ((int)value == 0) return RLSType.PRL;
            //else if ((int)value == 1) return RLSType.VRL;
            //else                      return RLSType.NRZ;
            return RLSType.PRL;
        }
    }
}
