using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Particles;
using NUnit.Framework;

namespace DeltaEngine.Editor.Converters.Tests
{
	internal class ValueRangeGraphStringConverterTest
	{
		[SetUp]
		public void CreateConverter()
		{
			converter = new ValueRangeGraphStringConverter();
		}

		private ValueRangeGraphStringConverter converter;

		[Test]
		public void ConvertCompleteValueRangeGraph()
		{
			var valueRangeList =
				new List<ValueRange>(new[]
				{ new ValueRange(0.0f, 1.0f), new ValueRange(0.5f, 0.75f), new ValueRange(1.0f, 2.0f) });
			var graphToConvert = new RangeGraph<ValueRange>(valueRangeList);
			var convertedString = converter.Convert(graphToConvert, typeof(string), null, null);
			var expectedString = "({" + valueRangeList[0] + "}, {" + valueRangeList[1] + "}, {" +
				valueRangeList[2] + "})";
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
			var ValueRangeList =
				new List<ValueRange>(new[]
				{ new ValueRange(0.0f, 1.0f), new ValueRange(0.5f, 0.75f), new ValueRange(1.0f, 2.0f) });
			var toConvert = "{" + ValueRangeList[0] + ", " + ValueRangeList[1] + ", " +
				ValueRangeList[2] + "}";
			var retrieved =
				(RangeGraph<ValueRange>)
					converter.ConvertBack(toConvert, typeof(RangeGraph<Vector3D>), null, null);
			var expected = new RangeGraph<ValueRange>(ValueRangeList);
			Assert.AreEqual(expected.Values, retrieved.Values);
		}

		[Test]
		public void ConvertingBackReversesConverting()
		{
			var ValueRangeList =
				new List<ValueRange>(new[]
				{ new ValueRange(0.0f, 1.0f), new ValueRange(0.5f, 0.75f), new ValueRange(1.0f, 2.0f) });
			var initialGraph = new RangeGraph<ValueRange>(ValueRangeList);
			var convertedString = converter.Convert(initialGraph, typeof(string), null, null);
			var retrievedGraph =
				(RangeGraph<ValueRange>)
					converter.ConvertBack(convertedString, typeof(RangeGraph<Vector3D>), null, null);
			Assert.AreEqual(initialGraph.Values, retrievedGraph.Values);
		}
	}
}