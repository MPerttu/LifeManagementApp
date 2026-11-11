using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeManagementApp.Converters
{
    /// <summary>
    /// ValueConverter, joka muuntaa TodoItemin prioriteettinumeron (1-5) Color-olioksi.
    /// </summary>
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int priority)
            {
                return priority switch
                {
                    5 => Color.FromArgb("#FEE2E2"), // Punainen (Kriittinen)
                    4 => Color.FromArgb("#FFEDD5"), // Oranssi (Korkea)
                    3 => Color.FromArgb("#FEF3C7"), // Keltainen (Normaali)
                    _ => Color.FromArgb("#E0F2FE")  // Sinertävä (Matala 1-2)
                };
            }

            return Color.FromArgb("#E2E8F0");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}