using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;


namespace Checkers.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Конвертер данных кастомной логики MultiBinding.
    /// Используется для передачи 2-х полей PasswordBox через
    /// СommandParameter.
    /// </summary>
    public class PasswordBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var tuple = new Tuple<PasswordBox, PasswordBox>((PasswordBox)values[0], (PasswordBox)values[1]);
            return tuple;
        } // Convert

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // ConvertBack
    } // PasswordBoxConverter class
} // Checkers.Services