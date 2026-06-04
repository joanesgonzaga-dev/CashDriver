using CashDriver.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Converters
{
    public class TipoMetaParaIconeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is EnumTipoMeta tipo)
            {
                return tipo == EnumTipoMeta.PorPrazo
                    ? "sandclock.svg"
                    : "currency.svg";
            }
            return "default.svg";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
