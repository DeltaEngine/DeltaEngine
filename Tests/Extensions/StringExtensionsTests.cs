using System;
using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class StringExtensionsTests
	{
		[Test]
		public void ConvertFloatToInvariantString()
		{
			Assert.AreEqual("2.5", 2.5f.ToInvariantString());
			Assert.AreNotEqual("3.5", 1.5f.ToInvariantString());
			Assert.AreEqual("01", 1.0f.ToInvariantString("00"));
			Assert.AreEqual("1.23", 1.2345f.ToInvariantString("0.00"));
			Assert.AreNotEqual("1.2345", 1.2345f.ToInvariantString("0.00"));
			Assert.AreEqual("1", StringExtensions.ToInvariantString(1));
		}

		[Test]
		public void Convert()
		{
			Assert.AreEqual(1.0f, "1.0".Convert<float>());
			Assert.AreEqual("abc", "abc".Convert<string>());
			Assert.Throws<FormatException>(() => "val".Convert<float>());
			Assert.AreEqual(1, "1".Convert<int>());
			Assert.AreEqual(24, "24".Convert<int>());
			Assert.AreEqual('A', "A".Convert<char>());
			Assert.AreEqual(943578.39543, "943578.39543".Convert<double>());
			Assert.AreEqual(true, "True".Convert<bool>());
			Assert.AreEqual(false, "False".Convert<bool>());
			Assert.AreEqual(SomeEnum.Additive, "Additive".Convert<SomeEnum>());
			Assert.AreEqual(new DateTime(2002, 12, 24), "2002-12-24".Convert<DateTime>());
		}

		public enum SomeEnum
		{
			Normal,
			Additive,
		}

		[Test]
		public void ToInvariantString()
		{
			Assert.AreEqual("null", StringExtensions.ToInvariantString(null));
			Assert.AreEqual("1", StringExtensions.ToInvariantString((object)1.0f));
			Assert.AreEqual("1.2", StringExtensions.ToInvariantString(1.2));
			Assert.AreEqual("1.4", StringExtensions.ToInvariantString(1.4m));
			Assert.AreEqual("abc", StringExtensions.ToInvariantString("abc"));
			Assert.AreEqual("01/01/2014 00:00:00",
				StringExtensions.ToInvariantString(new DateTime(2014, 1, 1)));
		}

		[Test]
		public void MaxStringLength()
		{
			Assert.AreEqual(null, ((string)null).MaxStringLength(4));
			Assert.AreEqual("", "".MaxStringLength(4));
			Assert.AreEqual("abcd", "abcd".MaxStringLength(4));
			Assert.AreEqual("ab..", "abcde".MaxStringLength(4));
			Assert.AreEqual("..", "abcde".MaxStringLength(1));
		}

		[Test]
		public static void SplitAndTrimByChar()
		{
			string[] components = "abc, 123, def".SplitAndTrim(',');
			Assert.AreEqual(components.Length, 3);
			Assert.AreEqual(components[0], "abc");
			Assert.AreEqual(components[1], "123");
			Assert.AreEqual(components[2], "def");
		}

		[Test]
		public static void SplitAndTrimByString()
		{
			string[] components = "3 plus 5 is 8".SplitAndTrim("plus", "is");
			Assert.AreEqual(components.Length, 3);
			Assert.AreEqual(components[0], "3");
			Assert.AreEqual(components[1], "5");
			Assert.AreEqual(components[2], "8");
		}

		[Test]
		public void SplitIntoFloats()
		{
			var stringFloats = new[]
			{ "1.0", "2.0", "0511.580254", Math.PI.ToString(CultureInfo.InvariantCulture) };
			var expectedFloats = new[] { 1.0f, 2.0f, 511.580261f, 3.14159274f };
			float[] floats = stringFloats.SplitIntoFloats();
			CollectionAssert.AreEqual(expectedFloats, floats);
		}

		[Test]
		public void SplitIntoFloatsWithSeparator()
		{
			const string StringFloats = "1.0, 2.0, 0511.580254";
			var expectedFloats = new[] { 1.0f, 2.0f, 511.580261f };
			CollectionAssert.AreEqual(expectedFloats, StringFloats.SplitIntoFloats(','));
			CollectionAssert.AreEqual(expectedFloats, StringFloats.SplitIntoFloats(", "));
		}

		[Test]
		public void Compare()
		{
			Assert.IsTrue("AbC1".Compare("aBc1"));
			Assert.IsTrue("1.23".Compare("1.23"));
			Assert.IsFalse("Hello".Compare("World"));
		}

		[Test]
		public void ContainsCaseInsensitive()
		{
			Assert.IsTrue("hallo".ContainsCaseInsensitive("ha"));
			Assert.IsTrue("1.23".ContainsCaseInsensitive("1.2"));
			Assert.IsTrue("Hello".ContainsCaseInsensitive("hel"));
			Assert.IsFalse("Banana".ContainsCaseInsensitive("Apple"));
			Assert.IsFalse(((String)null).ContainsCaseInsensitive("abc"));
		}

		[Test]
		public void IsFirstCharacterInLowerCase()
		{
			Assert.IsFalse("Banana".IsFirstCharacterInLowerCase());
			Assert.IsTrue("banana".IsFirstCharacterInLowerCase());
			Assert.IsTrue("".IsFirstCharacterInLowerCase());
		}

		[Test]
		public void ConvertFirstCharacterToUpperCase()
		{
			const string UpperCaseWord = "Banana";
			Assert.AreEqual(UpperCaseWord, UpperCaseWord.ConvertFirstCharacterToUpperCase());
			Assert.AreEqual(UpperCaseWord, "banana".ConvertFirstCharacterToUpperCase());
			Assert.AreEqual("", "".ConvertFirstCharacterToUpperCase());
		}

		[Test]
		public void SplitTextIntoWords()
		{
			var stringWords = "TestForSplittingAStringIntoWords".SplitWords(true);
			Assert.AreEqual(' ', stringWords[4]);
			Assert.AreEqual("Test for splitting a string into words", stringWords);
		}

		[Test]
		public void FromByteArray()
		{
			const string TestString = "TestString";
			Assert.AreEqual(TestString,
				StringExtensions.FromByteArray(StringExtensions.ToByteArray(TestString)));
		}

		[Test]
		public void StartsWith()
		{
			Assert.True(StringExtensions.StartsWith("Hi there, whattup?", "Hi"));
			Assert.False(StringExtensions.StartsWith("Hi there, whattup?", "what"));
			Assert.True(StringExtensions.StartsWith("bcdeuf", "bc"));
			Assert.False(StringExtensions.StartsWith("bcdeuf", "abc"));
			Assert.True("Hi there, whattup?".StartsWith(new[] { "Hi", "there", "what" }));
			Assert.False("Hi there, whattup?".StartsWith(new[] { "she", "there", "what" }));
			Assert.True(StringExtensions.StartsWith("ATI Radeon 9500+", "ati"));
			Assert.False(StringExtensions.StartsWith("A-t-i da gaga", "ati"));
			Assert.True(StringExtensions.StartsWith("NVidia Geforce3", "nvidia"));
		}

		[Test]
		public void SplitWordsWithEmptyWord()
		{
			const string TestString = "";
			Assert.AreEqual(TestString, TestString.SplitWords(true));
		}

		[Test]
		public void ConvertNotRecognizedType()
		{
			Assert.Throws<NotSupportedException>(() => "false,false".Convert<List<bool>>());
		}

		[Test]
		public void ConvertNullStringToDictionaryReturns()
		{
			const string ArrayOfStrings = null;
			var dictionary = ArrayOfStrings.Convert<Dictionary<string, string>>();
			Assert.IsInstanceOf<Dictionary<string, string>>(dictionary);
		}

		[Test]
		public void ConvertStringToDictionary()
		{
			const string ArrayOfStrings = "FirstKey;FirstValue;SecondKey;SecondValue";
			var dictionary = ArrayOfStrings.Convert<Dictionary<string, string>>();
			Assert.IsTrue(dictionary.ContainsKey("FirstKey"));
			Assert.IsTrue(dictionary.ContainsValue("SecondValue"));
		}

		[Test]
		public void ConvertDictionaryToString()
		{
			var dictionary = new Dictionary<string, string>();
			dictionary.Add("FirstKey","FirstValue");
			dictionary.Add("SecondKey","SecondValue");
			var arrayOfStrings = StringExtensions.ToInvariantString(dictionary);
			Assert.AreEqual(41, arrayOfStrings.Length);
			Assert.AreEqual(8, arrayOfStrings.IndexOf(';'));
			Assert.AreEqual(29, arrayOfStrings.LastIndexOf(';'));
		}

		[Test]
		public void CheckForCleanFileName()
		{
			Assert.AreEqual("", GetCleanFileName(null));
			Assert.AreEqual("", GetCleanFileName(""));
			Assert.AreEqual("", GetCleanFileName("   "));
			Assert.AreEqual("PureFilename", GetCleanFileName("PureFilename"));
			Assert.AreEqual("PureFilename", GetCleanFileName("Pure Filename"));
			Assert.AreEqual("FullFilename.ext", GetCleanFileName("FullFilename.ext"));
			Assert.AreEqual("File1.ext", GetCleanFileName("File#1.ext"));
			Assert.AreEqual("FileAB.ext", GetCleanFileName("FileA|B.ext"));
		}

		private static string GetCleanFileName(string filename)
		{
			return StringExtensions.GetFilenameWithoutForbiddenCharactersOrSpaces(filename);
		}
	}
}