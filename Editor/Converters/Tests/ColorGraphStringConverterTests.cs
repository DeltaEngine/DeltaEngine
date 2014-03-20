using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Editor.Converters.Tests
{
	internal class StringToColorGraphConverterTests
	{
		[SetUp]
		public void CreateConverter()
		{
			converter = new ColorGraphStringConverter();
		}

		private ColorGraphStringConverter converter;

		[Test]
		public void ConvertCompleteColorGraph()
		{
			var colorList = new List<Color>(new[] { Color.Black, Color.Red, Color.Green });
			var graphToConvert = new RangeGraph<Color>(colorList);
			var convertedString = converter.Convert(graphToConvert, typeof(string), null, null);
			var expectedString = "({" + colorList[0] + "}, {" + colorList[1] + "}, {" + colorList[2] + "})";
			Assert.AreEqual(expectedString, convertedString);
		}

		[Test]
		public void ConvertingFromNullAtLeastGivesEmpty()
		{
			var stringFromNull = converter.Convert(null, typeof(string), null, null);
			Assert.NotNull(stringFromNull);
			Assert.AreEqual("", stringFromNull);
		}

		[Test]
		public void ConvertingWrongTypeGivesEmpty()
		{
			var stringFromWrongType = converter.Convert(1.0f, typeof(string), null, null);
			Assert.NotNull(stringFromWrongType);
			Assert.AreEqual("", stringFromWrongType);
		}

		[Test]
		public void ConvertingBackEmptyOrNullGivesNull()
		{
			var retrievedFromEmpty = converter.ConvertBack("", typeof(RangeGraph<Color>), null, null);
			var retrievedFromNull = converter.ConvertBack(null, typeof(RangeGraph<Color>), null, null);
			Assert.IsNull(retrievedFromEmpty);
			Assert.IsNull(retrievedFromNull);
		}

		[Test]
		public void ConvertingBackFalselyFormattedStringGivesNull()
		{
			const string FaultyString = "{{1,2,5,6";
			var retrievedFromFaultyString = converter.ConvertBack(FaultyString,
				typeof(RangeGraph<Color>), null, null);
			Assert.IsNull(retrievedFromFaultyString);
		}

		[Test]
		public void ConvertBackCorrectString()
		{
			var colorList = new List<Color>(new[] { Color.Black, Color.Red, Color.Green });
			var toConvert = "{" + colorList[0] + ", " + colorList[1] + ", " + colorList[2] + "}";
			var retrieved =
				(RangeGraph<Color>)
					converter.ConvertBack(toConvert, typeof(RangeGraph<Color>), null, null);
			var expected = new RangeGraph<Color>(colorList);
			Assert.AreEqual(expected.Values, retrieved.Values);
		}

		[Test]
		public void ConvertingBackReversesConverting()
		{
			var colorList = new List<Color>(new[] { Color.Black, Color.Red, Color.Green });
			var initialGraph = new RangeGraph<Color>(colorList);
			var convertedString = converter.Convert(initialGraph, typeof(string), null, null);
			var retrievedGraph = (RangeGraph<Color>)converter.ConvertBack(convertedString,
				typeof(RangeGraph<Color>), null, null);
			Assert.AreEqual(initialGraph.Values, retrievedGraph.Values);
		}
	}
}