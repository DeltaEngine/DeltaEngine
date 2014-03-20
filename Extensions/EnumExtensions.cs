using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Provides the number of elements in an enum and some conversion and enumeration methods.
	/// </summary>
	public static class EnumExtensions
	{
		public static Array GetEnumValues(this Enum anyEnum)
		{
			Type enumType = anyEnum.GetType();
			return Enum.GetValues(enumType);
		}

		public static IEnumerable<EnumType> GetEnumValues<EnumType>()
		{
			return from object value in Enum.GetValues(typeof(EnumType)) select (EnumType)value;
		}

		public static int GetCount(this Enum anyEnum)
		{
			return GetEnumValues(anyEnum).Length;
		}

		public static int GetCount<EnumType>()
		{
			return Enum.GetValues(typeof(EnumType)).Length;
		}

		/// <summary>
		/// Tries to convert any text to a enum, if it fails defaultValue is returned. If you know the
		/// text will be in the correct format use <see cref="StringExtensions.Convert{T}"/>.
		/// </summary>
		public static T TryParse<T>(this string text, T defaultValue) where T : struct
		{
			T result;
			if (Enum.TryParse(text, true, out result))
				return result;
			if (Logger.TotalNumberOfAttachedLoggers > 0)
				Logger.Warning("Failed to parse enum value " + text + ", using default: " + defaultValue); //ncrunch: no coverage
			return defaultValue;
		}

		public static int GetIndex<T>(T searchEnumValue)
		{
			var list = new List<T>(GetEnumValues<T>());
			for (int index = 0; index < list.Count; index++)
				if (list[index].Equals(searchEnumValue))
					return index;
			return -1;
		}
	}
}