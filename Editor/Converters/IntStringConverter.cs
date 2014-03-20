using System;
using System.Globalization;
using System.Windows.Data;

namespace DeltaEngine.Editor.Converters
{
	public class IntStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is int))
				return "";
			return ((int)value).ToString(CultureInfo.InvariantCulture);
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue))
				return null;
			int newInt;
			return int.TryParse(stringValue, out newInt) ? (object)newInt : null;
		}
	}
}
