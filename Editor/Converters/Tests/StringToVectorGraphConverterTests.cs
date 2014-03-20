using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Editor.Converters.Tests
{
	internal class StringToVectorGraphConverterTests
	{
		[SetUp]
		public void CreateConverter()
		{
			converter = new VectorGraphStringConverter();
		}

		private VectorGraphStringConverter converter;

		[Test]
		public void ConvertCompleteVectorGraph()
		{
			var vectorList = new List<Vector3D>(new[] { Vector3D.Zero, Vector3D.One, Vector3D.UnitY });
			var graphToConvert = new RangeGraph<Vector3D>(vectorList);
			var convertedString = converter.Convert(graphToConvert, typeof(string), null, null);
			var expectedString = "({" + vectorList[0] + "}, {" + vectorList[1] + "}, {" + vectorList[2] + "})";
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
		public void ConvertingBackFalselyFormattedStringGivesNull()
		{
			const string FaultyString = "{{1,2,5,6";
			var retrievedFromFaultyString = converter.ConvertBack(FaultyString,
				typeof(RangeGraph<Vector3D>), null, null);
			Assert.IsNull(retrievedFromFaultyString);
		}

		[Test]
		public void ConvertBackCorrectString()
		{
			var vectorList = new List<Vector3D>(new[] { Vector3D.Zero, Vector3D.One, Vector3D.UnitY });
			var toConvert = "{" + vectorList[0] + ", " + vectorList[1] + ", " + vectorList[2] + "}";
			var retrieved =
				(RangeGraph<Vector3D>)
					converter.ConvertBack(toConvert, typeof(RangeGraph<Vector3D>), null, null);
			var expected = new RangeGraph<Vector3D>(vectorList);
			Assert.AreEqual(expected.Values, retrieved.Values);
		}

		[Test]
		public void ConvertingBackReversesConverting()
		{
			var vectorList = new List<Vector3D>(new[] { Vector3D.Zero, Vector3D.One, Vector3D.UnitY });
			var initialGraph = new RangeGraph<Vector3D>(vectorList);
			var convertedString = converter.Convert(initialGraph, typeof(string), null, null);
			var retrievedGraph =
				(RangeGraph<Vector3D>)
					converter.ConvertBack(convertedString, typeof(RangeGraph<Vector3D>), null, null);
			Assert.AreEqual(initialGraph.Values, retrievedGraph.Values);
		}
	}
}