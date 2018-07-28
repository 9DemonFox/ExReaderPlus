﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ExReaderPlus.View.Converter {

    public class BOOLtoICONF :IValueConverter{
        public object Convert(object value, Type targetType, object parameter, string language) {
            return ((IconKind)value).Equals((IconKind)parameter) ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class GRIDLtoDOUBLE : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return new GridLength(System.Convert.ToDouble(value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return ((GridLength)value).Value;
        }
    }

    public class NegativeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (parameter is null)
                return -System.Convert.ToDouble(value);
            if (parameter.ToString().Equals("bool"))
                return (bool)value ? false : true;
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return null;
        }
    }

    public class BOOLC : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return (bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            return System.Convert.ToBoolean(value);
        }
    }

    public class NULLnoVIS : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            if (parameter is null)
                return (value is null) ? Visibility.Collapsed : Visibility.Visible;
            else if (parameter.Equals("1"))
                return System.Convert.ToInt32(value).Equals(0) ? Visibility.Collapsed : Visibility.Visible;
            else if (parameter.Equals("0"))
                return System.Convert.ToInt32(value).Equals(1) ? Visibility.Collapsed : Visibility.Visible;
            else return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class PhoneticCON : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return "[" + value.ToString().Split(',')[0] + "]";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class INTtoSTR : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            int k = System.Convert.ToInt32(value);
            return parameter.ToString().Split(',')[k];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            throw new NotImplementedException();
        }
    }

    public class ObjToString : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, string language) {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) {
            ComboBoxItem cbi = value as ComboBoxItem;
            
            return cbi.Content.ToString();
        }
    }

}
