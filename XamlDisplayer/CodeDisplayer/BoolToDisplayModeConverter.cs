using System;
using System.Globalization;
using System.Windows.Data;

namespace CodeDisplayer {
    public class BoolToDisplayModeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = (bool) value;
            if (input) {
                return XamlDisplayer.DisplayModeEnum.TopBottom;
            }
            else {
                return XamlDisplayer.DisplayModeEnum.LeftRight;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
