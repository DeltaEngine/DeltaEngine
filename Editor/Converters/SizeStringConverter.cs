using System;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Converters
{
	public class SizeStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is Size))
				return "";
			return ((Size)value).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue) || !IsInputParsable(value.ToString()))
				return null;
			var stringPartitions = stringValue.SplitAndTrim(new[] {',', ' ' });
			if (stringPartitions.Length < 2)
				return null;
			try
			{
				return TryConvertBack(stringPartitions);
			}
			catch
			{
				return null;
			}
		}

		private static object TryConvertBack(string[] stringPartitions)
		{
			var size = new Size();
			if (!IsInputParsable(stringPartitions[0]) || !IsInputParsable(stringPartitions[1]))
				return null;
			size.Width = float.Parse(stringPartitions[0], CultureInfo.InvariantCulture);
			size.Height = float.Parse(stringPartitions[1], CultureInfo.InvariantCulture);
			return size;
		}

		private static bool IsInputParsable(string input)
		{
			return !(input == "-0" || input.EndsWith(".") || input.EndsWith("0") && input.Contains("."));
		}
	}
}