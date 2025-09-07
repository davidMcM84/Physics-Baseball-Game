using System;
using System.Globalization;
using System.Windows.Data;

namespace Physics_Baseball_Game.Converters
{
    /// <summary>
    /// Scales a raw double into a 0-100 range for rating bars.
    /// ConverterParameter format: min|max|[invert]
    /// Example: 0|40   or 0.08|0.35|invert
    /// </summary>
    public sealed class ScaleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not double d) return 0d;
            if (parameter is not string spec) return d;

            var parts = spec.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return d;

            if (!double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var min)) return 0d;
            if (!double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var max)) return 0d;
            bool invert = parts.Length > 2 && parts[2].Equals("invert", StringComparison.OrdinalIgnoreCase);

            if (Math.Abs(max - min) < double.Epsilon) return 0d;

            double clamped = Math.Max(min, Math.Min(max, d));
            double norm = (clamped - min) / (max - min); // 0..1
            if (invert) norm = 1 - norm;

            return norm * 100.0;
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
}