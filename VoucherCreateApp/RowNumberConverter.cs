using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Data;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Data;

namespace VoucherCreateApp
{
    class RowNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rowHandle = value as RowHandle;
            if (rowHandle != null)
            {
                if (rowHandle.Value < 0)
                    return "*";
                int num = rowHandle.Value;
                return num+1;
            }
            return "2";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return 0;
        }
    }
}
