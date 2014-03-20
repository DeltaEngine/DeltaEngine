using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Provides additional and simplified string manipulation methods.
	/// </summary>
	public static class StringExtensions
	{
		static StringExtensions()
		{
			AddConvertTypeCreation(typeof(Color), value => new Color(value));
			AddConvertTypeCreation(typeof(Size), value => new Size(value));
			AddConvertTypeCreation(typeof(Vector2D), value => new Vector2D(value));
			AddConvertTypeCreation(typeof(Vector3D), value => new Vector3D(value));
			AddConvertTypeCreation(typeof(Rectangle), value => new Rectangle(value));
		}

		public static void AddConvertTypeCreation(Type typeToConvert, Func<string, object> conversion)
		{
			if (!RegisteredConvertCallbacks.ContainsKey(typeToConvert))
				RegisteredConvertCallbacks.Add(typeToConvert, conversion);
		}

		public static string ToInvariantString(this float number)
		{
			return number.ToString(NumberFormatInfo.InvariantInfo);
		}

		public static string ToInvariantString(this float number, string format)
		{
			return number.ToString(format, NumberFormatInfo.InvariantInfo);
		}

		public static T Convert<T>(this string value)
		{
			var type = typeof(T);
			if (type == typeof(string))
				return (T)(System.Convert.ToString(value) as object);
			if (type == typeof(int))
				return (T)(System.Convert.ToInt32(value) as object);
			if (type == typeof(double))
				return (T)(System.Convert.ToDouble(value, CultureInfo.InvariantCulture) as object);
			if (type == typeof(float))
				return (T)(System.Convert.ToSingle(value, CultureInfo.InvariantCulture) as object);
			if (type == typeof(bool))
				return (T)(System.Convert.ToBoolean(value) as object);
			if (type == typeof(char))
				return (T)(System.Convert.ToChar(value) as object);
			if (type.IsEnum)
				return (T)Enum.Parse(type, value);
			if (type == typeof(DateTime))
				return (T)(DateExtensions.Parse(value) as object);
			if (RegisteredConvertCallbacks.ContainsKey(type))
				return (T)RegisteredConvertCallbacks[type](value);
			if (type == typeof(Dictionary<string, string>))
				return ConvertStringToDictionary<T>(value);
			throw new NotSupportedException("Type " + type + " was not registered for conversion!");
		}

		private static T ConvertStringToDictionary<T>(string value)
		{
			var dictionary = new Dictionary<string, string>();
			if (value == null)
				return (T)(dictionary as object);
			string[] splitValues = value.Split(';');
			for (int i = 0; i < splitValues.Length - 1; i += 2)
				dictionary.Add(splitValues[i], splitValues[i + 1]);
			return (T)(dictionary as object);
		}

		private static readonly Dictionary<Type, Func<string, object>> RegisteredConvertCallbacks =
			new Dictionary<Type, Func<string, object>>();

		public static string ToInvariantString(object someObj)
		{
			if (someObj == null)
				return "null";
			if (someObj is float)
				return ((float)someObj).ToString(NumberFormatInfo.InvariantInfo);
			if (someObj is double)
				return ((double)someObj).ToString(NumberFormatInfo.InvariantInfo);
			if (someObj is decimal)
				return ((decimal)someObj).ToString(NumberFormatInfo.InvariantInfo);
			if (someObj is DateTime)
				return ((DateTime)someObj).ToString(CultureInfo.InvariantCulture);
			if (someObj is Dictionary<string, string>)
				return ConvertDictionaryToString(someObj);
			return someObj.ToString();
		}

		private static string ConvertDictionaryToString(object someObj)
		{
			var dictionary = someObj as Dictionary<string, string>;
			string separatedValues = "";
			foreach (string key in dictionary.Keys)
				separatedValues += (separatedValues == "" ? "" : ";") + key + ";" + dictionary[key];
			return separatedValues;
		}

		public static float[] SplitIntoFloats(this string value)
		{
			return SplitIntoFloats(value, new[] { ',', '(', ')', '{', '}' });
		}

		public static float[] SplitIntoFloats(this string value, params char[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return SplitIntoFloats(components);
		}

		public static float[] SplitIntoFloats(this string[] components)
		{
			var floats = new float[components.Length];
			for (int i = 0; i < floats.Length; i++)
				floats[i] = components[i].Convert<float>();
			return floats;
		}

		public static float[] SplitIntoFloats(this string value, params string[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return SplitIntoFloats(components);
		}

		public static string MaxStringLength(this string value, int maxLength)
		{
			if (String.IsNullOrEmpty(value) || value.Length <= maxLength)
				return value;
			if (maxLength < 2)
				maxLength = 2;
			return value.Substring(0, maxLength - 2).TrimEnd() + "..";
		}

		public static string[] SplitAndTrim(this string value, params char[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		private static string[] TrimAndRemoveEmptyElements(string[] values)
		{
			var nonEmptyElements = new List<string>();
			for (int i = 0; i < values.Length; i++)
			{
				string trimmedElement = values[i].Trim();
				if (trimmedElement.Length > 0)
					nonEmptyElements.Add(trimmedElement);
			}
			return nonEmptyElements.ToArray();
		}

		public static string[] SplitAndTrim(this string value, params string[] separators)
		{
			string[] components = value.Split(separators, StringSplitOptions.None);
			return TrimAndRemoveEmptyElements(components);
		}

		public static bool Compare(this string value, string other)
		{
			return String.Compare(value, other, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		public static bool ContainsCaseInsensitive(this string value, string searchText)
		{
			return value != null &&
				value.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsFirstCharacterInLowerCase(this string word)
		{
			if (String.IsNullOrEmpty(word))
				return true;
			char firstChar = word[0];
			return firstChar < 'A' || firstChar > 'Z';
		}

		public static string ConvertFirstCharacterToUpperCase(this string word)
		{
			if (String.IsNullOrEmpty(word) || !word.IsFirstCharacterInLowerCase())
				return word;
			return (char)(word[0] - 32) + word.Substring(1);
		}

		public static byte[] ToByteArray(string text)
		{
			return Encoding.UTF8.GetBytes(text);
		}

		public static string FromByteArray(byte[] byteArray)
		{
			return Encoding.UTF8.GetString(byteArray);
		}

		public static bool StartsWith(this string name, params string[] partialNames)
		{
			return partialNames.Any(
				x => name.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));
		}

		public static string SplitWords(this string stringToSplit,
			bool convertFirstLetterOfEachWordAfterFirstWordToLowerCase = false)
		{
			string words = "";
			if (string.IsNullOrEmpty(stringToSplit))
				return words;
			for (int i = 0; i < stringToSplit.Length; ++i)
			{
				char letter = stringToSplit[i];
				if (letter == char.ToUpper(letter) && i != 0 && letter >= 'A')
					words += " " +
						(convertFirstLetterOfEachWordAfterFirstWordToLowerCase
							? letter.ToString(CultureInfo.InvariantCulture).ToLower() : letter + "");
				else
					words += letter;
			}
			return words;
		}

		public static string GetFilenameWithoutForbiddenCharactersOrSpaces(string filename)
		{
			if (String.IsNullOrWhiteSpace(filename))
				return "";
			string cleanFileName = "";
			foreach (char character in filename)
				if (character > ' ' && IsAllowedFilenameCharacter(character))
					cleanFileName += character;
			return cleanFileName;
		}

		private static bool IsAllowedFilenameCharacter(char character)
		{
			return character >= 'A' && character <= 'Z' || character >= 'a' && character <= 'z' ||
				character >= '0' && character <= '9' || character == '.';
		}
	}
}