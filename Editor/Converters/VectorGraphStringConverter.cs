using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Converters
{
	public class VectorGraphStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is RangeGraph<Vector3D>))
				return "";
			return (value as RangeGraph<Vector3D>).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			CultureInfo culture)
		{
			var stringValue = value is string ? value as string : null;
			if (string.IsNullOrEmpty(stringValue))
				return null;
			var stringPartitions = stringValue.SplitAndTrim(new[] { '(', '{', ',', ' ', '}', ')' });
			if (stringPartitions.Length % 3 != 0 || stringPartitions.Length < 6)
				return null;
			return FillRangeFromStringPartitions(stringPartitions);
		}

		private static RangeGraph<Vector3D> FillRangeFromStringPartitions(string[] partitions)
		{
			var vectorList = new List<Vector3D>();
			for (int i = 0; i < partitions.Length - 2; i += 3)
			{
				if (!IsInputParsable(partitions[i]) || !IsInputParsable(partitions[i + 1]) ||
					!IsInputParsable(partitions[i + 2]))
					return null;
				try
				{
					vectorList.Add(new Vector3D(float.Parse(partitions[i], CultureInfo.InvariantCulture),
						float.Parse(partitions[i + 1], CultureInfo.InvariantCulture),
						float.Parse(partitions[i + 2], CultureInfo.InvariantCulture)));
				}
				catch
				{
					return null;
				}
			}
			return new RangeGraph<Vector3D>(vectorList);
		}

		private static bool IsInputParsable(string input)
		{
			return !(input == "-0" || input.EndsWith(".") || input.EndsWith("0") && input.Contains("."));
		}
	}
}