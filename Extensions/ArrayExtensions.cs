using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Additional array manipulation and array to text methods.
	/// </summary>
	public static class ArrayExtensions
	{
		public static bool Compare<T>(this IEnumerable<T> array1, IEnumerable<T> array2)
		{
			return array1 == null && array2 == null ||
				array1 != null && array2 != null && array1.SequenceEqual(array2);
		}

		public static string ToText<T>(this IEnumerable<T> texts, string separator = ", ")
		{
			return string.Join(separator, texts);
		}

		public static Value GetWithDefault<Key, Value>(Dictionary<Key, object> dict, Key key)
		{
			if (dict.ContainsKey(key) == false)
				return default(Value);
			Value result;
			try
			{
				result = (Value)dict[key];
			}
			catch
			{
				result = default(Value);
			}
			return result;
		}

		public static T[] Insert<T>(this T[] array, T value, int insertIndex)
		{
			var result = new T[array.Length + 1];
			for (int i = 0; i < result.Length; i++)
				if (i == insertIndex)
					result[i] = value;
				else if (i > insertIndex)
					result[i] = array[i - 1];
				else
					result[i] = array[i];
			return result;
		}
	}
}