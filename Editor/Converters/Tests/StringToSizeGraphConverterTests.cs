using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Editor.Converters.Tests
{
	internal class StringToSizeGraphConverterTests
	{
		[SetUp]
		public void CreateConverter()
		{
			converter = new SizeGraphStringConverter();
		}

		private SizeGraphStringConverter converter;

		[Test]
		public void ConvertCompleteSizeGraph()
		{
			var sizeList = new List<Size>(new[] { Size.Half, Size.One, Size.Zero, Size.Half });
			var graphToConvert = new RangeGraph<Size>(sizeList);
			var convertedString = converter.Convert(graphToConvert, typeof(string), null, null);
			var expectedString = "({" + sizeList[0] + "}, {" + sizeList[1] + "}, {" + sizeList[2] + "}, {" +
				sizeList[3] + "})";
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
			var retrievedFromEmpty = converter.ConvertBack("", typeof(RangeGraph<Vector3D>), null, null);
			var retrievedFromNull = converter.ConvertBack(null, typeof(RangeGraph<Vector3D>), null, null);
			Assert.IsNull(retrievedFromEmpty);
			Assert.IsNull(retrievedFromNull);
		}

		[Test]
		public void ConvertBackCorrectString()
		{
			var sizeList = new List<Size>(new[] { Size.Half, Size.One, Size.Zero, Size.Half });
			var toConvert = "{" + sizeList[0] + ", " + sizeList[1] + ", " + sizeList[2] + ", " +
				sizeList[3] + "}";
			var retrieved =
				(RangeGraph<Size>)
					converter.ConvertBack(toConvert, typeof(RangeGraph<Vector3D>), null, null);
			var expected = new RangeGraph<Size>(sizeList);
			Assert.AreEqual(expected.Values, retrieved.Values);
		}

		[Test]
		public void ConvertingBackReversesConverting()
		{
			var sizeList = new List<Size>(new[] { Size.Half, Size.One, Size.Zero, Size.Half });
			var initialGraph = new RangeGraph<Size>(sizeList);
			var convertedString = converter.Convert(initialGraph, typeof(string), null, null);
			var retrievedGraph =
				(RangeGraph<Size>)
					converter.ConvertBack(convertedString, typeof(RangeGraph<Vector3D>), null, null);
			Assert.AreEqual(initialGraph.Values, retrievedGraph.Values);
		}
	}
}