using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Core
{
	public partial class BinaryDataLoadSaveTests
	{
		[Test]
		public void SaveAndLoadPrimitiveDataTypes()
		{
			SaveDataTypeAndLoadAgain((sbyte)-8);
			SaveDataTypeAndLoadAgain(-8);
			SaveDataTypeAndLoadAgain((Int16)8);
			SaveDataTypeAndLoadAgain((UInt16)8);
			SaveDataTypeAndLoadAgain((long)-8);
			SaveDataTypeAndLoadAgain((uint)8);
			SaveDataTypeAndLoadAgain((ulong)8);
			SaveDataTypeAndLoadAgain(3.4f);
			SaveDataTypeAndLoadAgain(8.4);
			SaveDataTypeAndLoadAgain(false);
		}

		[Test]
		public void SaveAndLoadOtherDatatypes()
		{
			SaveDataTypeAndLoadAgain("Hi");
			SaveDataTypeAndLoadAgain('x');
			SaveDataTypeAndLoadAgain((decimal)8.4);
			SaveDataTypeAndLoadAgain("asdf".ToCharArray());
			SaveDataTypeAndLoadAgain(StringExtensions.ToByteArray("asdf"));
			SaveDataTypeAndLoadAgain(TestEnum.SomeFlag);
		}

		[Test]
		public void SaveAndLoadLists()
		{
			SaveAndLoadList(new List<int> { 2, 4, 7, 15 });
			SaveAndLoadList(new List<Object> { 2, 0.5f, "Hello" });
		}

		[Test]
		public void SaveAndLoadDictionaries()
		{
			SaveAndLoadDictionary(new Dictionary<string, string>());
			SaveAndLoadDictionary(new Dictionary<string, string> { { "Key", "Value" } });
			SaveAndLoadDictionary(new Dictionary<string, int> { { "One", 1 }, { "Two", 2 } });
			SaveAndLoadDictionary(new Dictionary<int, float> { { 1, 1.1f }, { 2, 2.2f }, { 3, 3.3f } });
			SaveAndLoadDictionary(new Dictionary<int, object> { { 1, Vector2D.One }, { 2, Color.Red } });
		}

		[Test]
		public void SaveAndLoadArrays()
		{
			SaveAndLoadArray(new[] { 2, 4, 7, 15 });
			SaveAndLoadArray(new object[] { 2, 0.5f, "Hello" });
			SaveAndLoadArray(new byte[] { 5, 6, 7 });
			SaveAndLoadArray(new byte[0]);
		}

		[Test]
		public void SaveAndLoadEnumArray()
		{
			SaveAndLoadArray(new[] { TestEnum.SomeFlag, TestEnum.SomeFlag });
		}

		[Test]
		public void SaveAndLoadArraysContainingNullValues()
		{
			BinaryDataExtensions.SaveDataIntoMemoryStream(new object[] { null });
			BinaryDataExtensions.SaveDataIntoMemoryStream(new object[] { 0, 'a', "hallo", null });
		}

		[Test]
		public void SaveAndLoadClassWithArrays()
		{
			var instance = new ClassWithArrays();
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var retrieved =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ClassWithArrays>(data);
			Assert.IsTrue(retrieved.byteData.Compare(new byte[] { 1, 2, 3, 4, 5 }),
				retrieved.byteData.ToText());
			Assert.IsTrue(retrieved.charData.Compare(new[] { 'a', 'b', 'c' }),
				retrieved.charData.ToText());
			Assert.IsTrue(retrieved.intData.Compare(new[] { 10, 20, 30 }), retrieved.intData.ToText());
			Assert.IsTrue(retrieved.stringData.Compare(new[] { "Hi", "there" }),
				retrieved.stringData.ToText());
			Assert.IsTrue(retrieved.enumData.Compare(new[] { DayOfWeek.Monday, DayOfWeek.Sunday }),
				retrieved.enumData.ToText());
			Assert.IsTrue(retrieved.byteEnumData.Compare(new[] { ByteEnum.Normal, ByteEnum.High }),
				retrieved.byteEnumData.ToText());
		}

		[Test]
		public void SaveAndLoadClassWithEmptyByteArray()
		{
			var instance = new ClassWithByteArray { data = new byte[] { 1, 2, 3 } };
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var retrieved =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ClassWithByteArray>(data);
			Assert.IsTrue(instance.data.Compare(retrieved.data));
		}

		[Test]
		public void SaveAndLoadArrayWithOnlyNullElements()
		{
			var instance = new object[] { null, null };
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var retrieved = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<object[]>(data);
			Assert.IsTrue(instance.Compare(retrieved));
		}

		[Test]
		public void SaveAndLoadArrayWithMixedNumbersAndNullElements()
		{
			var instance = new object[] { 1, null };
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var retrieved = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<object[]>(data);
			Assert.IsTrue(instance.Compare(retrieved));
		}

		[Test]
		public void SaveAndLoadExplicitLayoutStruct()
		{
			var explicitLayoutTest = new ExplicitLayoutTestClass
			{
				someValue = 8,
				anotherValue = 5,
				unionValue = 7
			};
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(explicitLayoutTest);
			var retrieved =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ExplicitLayoutTestClass>(data);
			Assert.AreEqual(8, retrieved.someValue);
			Assert.AreEqual(7, retrieved.anotherValue);
			Assert.AreEqual(7, retrieved.unionValue);
		}

		[Test]
		public void SaveAndLoadClassWithAnotherClassInside()
		{
			var instance = new ClassWithAnotherClassInside
			{
				Number = 17,
				Data =
					new ClassWithAnotherClassInside.InnerDerivedClass { Value = 1.5, additionalFlag = true },
				SecondInstanceNotSet = null
			};
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			var retrieved =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ClassWithAnotherClassInside>(
					data);
			Assert.AreEqual(instance.Number, retrieved.Number);
			Assert.AreEqual(instance.Data.Value, retrieved.Data.Value);
			Assert.AreEqual(instance.Data.additionalFlag, retrieved.Data.additionalFlag);
			Assert.AreEqual(instance.SecondInstanceNotSet, retrieved.SecondInstanceNotSet);
		}

		[Test]
		public void ThrowExceptionIfTypeNameStartsWithXml()
		{
			Assert.Throws<BinaryDataSaver.UnableToSave>(
				() => BinaryDataExtensions.SaveToMemoryStream(new XmlBinaryData("Xml")));
			Assert.AreEqual("Xml", new XmlBinaryData("Xml").Text);
		}

		[Test]
		public void ThrowExceptionIfTypeNameStartsWithMock()
		{
			Assert.Throws<BinaryDataSaver.UnableToSave>(
				() => BinaryDataExtensions.SaveToMemoryStream(new MockBinaryData()));
		}

		private class MockBinaryData {}

		[Test]
		public void ThrowExceptionIfTypeNameEndsWithImageOrSound()
		{
			Assert.Throws<BinaryDataSaver.UnableToSave>(
				() => BinaryDataExtensions.SaveToMemoryStream(new NonContentImage()));
			Assert.Throws<BinaryDataSaver.UnableToSave>(
				() => BinaryDataExtensions.SaveToMemoryStream(new NonContentSound()));
		}

		private class NonContentImage { }
		private class NonContentSound { }

		[Test]
		public void LoadAndSaveClassWithMemoryStream()
		{
			var instance = new ClassWithMemoryStream(new byte[] { 1, 2, 3, 4 });
			instance.Writer.Write(true);
			instance.Version = 3;
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(instance);
			// Only the internal data should be saved, 1 byte memory stream not null, 1 byte data length,
			// memory stream data: 4 bytes+1 bool byte, 4 byte for the int Version
			Assert.AreEqual(11, data.Length);
			var retrieved =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<ClassWithMemoryStream>(data);
			Assert.IsNotNull(retrieved.reader);
			Assert.AreEqual(instance.Version, retrieved.Version);
			Assert.AreEqual(instance.Length, retrieved.Length);
			Assert.IsTrue(instance.Bytes.Compare(retrieved.Bytes), retrieved.Bytes.ToText());
		}

		[Test]
		public void LoadingAndSavingKnownTypeShouldNotCauseLoggerMessage()
		{
			using (var logger = new MockLogger())
			{
				var data = BinaryDataExtensions.SaveDataIntoMemoryStream(Vector2D.One);
				var loaded = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<Vector2D>(data);
				Assert.AreEqual(Vector2D.One, loaded);
				Assert.AreEqual(0, logger.NumberOfMessages);
			}
		}

		[Test]
		public void LoadUnknownTypeShouldThrowException()
		{
			Assert.Throws<Exception>(
				() =>
					BinaryDataLoader.CreateAndLoad(typeof(Vector2D), new BinaryReader(new MemoryStream()),
						new Version(0, 0)));
		}

		[Test]
		public void CreateInstanceOfTypeWithCtorParamsShouldThrowException()
		{
			Assert.Throws<MissingMethodException>(
				() =>
					BinaryDataLoader.CreateAndLoad(typeof(ClassThatRequiresConstructorParameter),
						new BinaryReader(new MemoryStream()), new Version(0, 0)));
		}

		[Test]
		public void WriteAndReadNumberMostlyBelow255ThatIsReallyBelow255()
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			const int NumberBelow255 = 123456;
			writer.WriteNumberMostlyBelow255(NumberBelow255);
			data.Position = 0;
			var reader = new BinaryReader(data);
			Assert.AreEqual(NumberBelow255, reader.ReadNumberMostlyBelow255());
		}

		[Test]
		public void WriteAndReadNumberMostlyBelow255WithANumberOver255()
		{
			var data = new MemoryStream();
			var writer = new BinaryWriter(data);
			const int NumberOver255 = 123456;
			writer.WriteNumberMostlyBelow255(NumberOver255);
			data.Position = 0;
			var reader = new BinaryReader(data);
			Assert.AreEqual(NumberOver255, reader.ReadNumberMostlyBelow255());
		}

		[Test]
		public void ThrowExceptionOnSavingAnInvalidObject()
		{
			Assert.Throws<NullReferenceException>(
				() => BinaryDataSaver.SaveDataType(null, typeof(object), null));
		}

		[Test]
		public void ThrowExceptionOnSavingAnUnsupportedStream()
		{
			using (var otherStreamThanMemory = new BufferedStream(new MemoryStream()))
			using (var dataWriter = new BinaryWriter(otherStreamThanMemory))
				Assert.Throws<BinaryDataSaver.UnableToSave>(
					() => BinaryDataSaver.SaveDataType(otherStreamThanMemory, typeof(object), dataWriter));
		}

		[Test]
		public void SaveAndLoadRange()
		{
			var range = new Range<Vector2D>(Vector2D.Zero, new Vector2D(3.0f, 3.0f));
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(range);
			var output =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<Range<Vector2D>>(data);
			Assert.AreEqual(range.Start, output.Start);
			Assert.AreEqual(range.End, output.End);
		}

		[Test]
		public void SaveGenericTypeOnlySavesTheGenericTypeNameAndTheArgument()
		{
			var range = new Range<Vector2D>(Vector2D.Zero, new Vector2D(3.0f, 3.0f));
			var data = BinaryDataExtensions.SaveToMemoryStream(range);
			int rangeNameLength = "Range".Length + 1;
			int vector2DNameLength = "Vector2D".Length + 1;
			const int VersionLength = 4;
			int vector2DLength = Vector2D.SizeInBytes;
			Assert.AreEqual(rangeNameLength + vector2DNameLength + VersionLength + vector2DLength * 2,
				data.Length);
		}

		[Test]
		public void SaveRangeGraph()
		{
			var range = new RangeGraph<Vector2D>(Vector2D.Zero, new Vector2D(3.0f, 3.0f));
			var data = BinaryDataExtensions.SaveToMemoryStream(range);
			Assert.AreEqual(68, data.Length);
		}

		[Test]
		public void SaveAndLoadGenericType()
		{
			var range = new Range<Vector2D>(Vector2D.Zero, new Vector2D(3.0f, 3.0f));
			var data = BinaryDataExtensions.SaveToMemoryStream(range);
			var loadedRange = data.CreateFromMemoryStream() as Range<Vector2D>;
			Assert.AreEqual(range.Start, loadedRange.Start);
			Assert.AreEqual(range.End, loadedRange.End);
		}

		[Test]
		public void CreateTypeFromShortName()
		{
			Assert.IsNotNull(BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound("Int32"));
			Assert.IsNotNull(BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound("Material"));
			Assert.Throws<TypeLoadException>(
				() => BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound("abc535"));
		}

		[Test]
		public void CreateTypeFromFullName()
		{
			Assert.IsNotNull(
				BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound(
					"DeltaEngine.Content.Material"));
		}
	}
}