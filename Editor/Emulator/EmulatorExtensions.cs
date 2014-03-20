using System;
using System.Windows;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Emulator
{
	public static class EmulatorExtensions
	{
		public static void RegisterTypesForConversion()
		{
			StringExtensions.AddConvertTypeCreation(typeof(Point), value => ConvertStringToPoint(value));
			StringExtensions.AddConvertTypeCreation(typeof(Size), value => ConvertStringToSize(value));
		}

		private static Point ConvertStringToPoint(string value)
		{
			try
			{
				return TryConvertStringToPoint(value);
			}
			catch (FormatException)
			{
				return new Point(0, 0);
			}
		}

		private static Point TryConvertStringToPoint(string value)
		{
			string[] components = value.Split(',');
			return new Point(Int32.Parse(components[0]), Int32.Parse(components[1]));
		}

		private static Size ConvertStringToSize(string value)
		{
			try
			{
				return TryConvertStringToSize(value);
			}
			catch (FormatException)
			{
				return new Size(0, 0);
			}
		}

		private static Size TryConvertStringToSize(string value)
		{
			string[] components = value.Split(',');
			return new Size(Int32.Parse(components[0]), Int32.Parse(components[1]));
		}
	}
}