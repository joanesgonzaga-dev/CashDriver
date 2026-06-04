using CashDriver.Models.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashDriver.Converters
{
    public class TipoDespesaParaIconeConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EnumTipoDespesa tipo)
            {
                if (tipo == EnumTipoDespesa.Alimentacao)
                {
                    return "food_black_50dp.svg";
                }

                else if (tipo == EnumTipoDespesa.Combustivel)
                {
                    return "fuel_black_50dp.svg";
                }

                else if (tipo == EnumTipoDespesa.Limpeza)
                {
                    return "clean_black_50dp.svg";
                }

                else if (tipo == EnumTipoDespesa.Manutencao)
                {
                    return "build_black_50dp.svg";
                }

                else if (tipo == EnumTipoDespesa.Saude)
                {
                    return "health_black_50dp.svg";
                }

                else
                {
                    return "miscellaneous_black_50dp.svg";
                }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
