using System;
using System.Globalization;
using System.Windows.Data;

namespace DeltaEngine.Editor.Converters
{
	public class FloatStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is float))
				return "";
			return ((float)value).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue) || !IsInputParsable(value.ToString()))
				return null;
			try
			{
				return float.Parse(stringValue, CultureInfo.InvariantCulture);
			}
			catch
			{
				return null;
			}
		}

		private static bool IsInputParsable(string input)
		{
			return !(input == "-0" || input.EndsWith(".") || input.EndsWith("0") && input.Contains("."));
		}
	}
}