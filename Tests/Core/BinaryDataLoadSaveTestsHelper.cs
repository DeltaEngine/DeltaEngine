using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Core
{
	public partial class BinaryDataLoadSaveTests
	{
		private static void SaveDataTypeAndLoadAgain<Primitive>(Primitive input)
		{
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(input);
			var output = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<Primitive>(data);
			Assert.AreEqual(input, output);
		}

		private enum TestEnum
		{
			SomeFlag,
		}

		private static void SaveAndLoadList<Primitive>(List<Primitive> listData)
		{
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(listData);
			var retrievedList =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<List<Primitive>>(data);
			Assert.AreEqual(listData.Count, retrievedList.Count);
			if (typeof(Primitive).IsValueType)
				Assert.IsTrue(listData.Compare(retrievedList));

			for (int index = 0; index < listData.Count; index++)
				Assert.AreEqual(listData[index].GetType(), retrievedList[index].GetType());
		}

		private static void SaveAndLoadDictionary<Key, Value>(Dictionary<Key, Value> dictionaryData)
		{
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(dictionaryData);
			var retrievedDictionary =
				BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<Dictionary<Key, Value>>(data);
			Assert.AreEqual(dictionaryData.Count, retrievedDictionary.Count);
			if (typeof(Key).IsValueType && typeof(Value).IsValueType)
				Assert.IsTrue(dictionaryData.Compare(retrievedDictionary));
			Assert.IsTrue(!dictionaryData.Except(retrievedDictionary).Any());
		}

		private static void SaveAndLoadArray<T>(T[] array)
		{
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(array);
			var retrievedArray = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<T[]>(data);
			Assert.AreEqual(array.Length, retrievedArray.Length);
			Assert.IsTrue(array.Compare(retrievedArray));
		}
		
		private class ClassWithArrays
		{
			public readonly byte[] byteData = { 1, 2, 3, 4, 5 };
			public readonly char[] charData = { 'a', 'b', 'c' };
			public readonly int[] intData = { 10, 20, 30 };
			public readonly string[] stringData = { "Hi", "there" };
			public readonly DayOfWeek[] enumData = { DayOfWeek.Monday, DayOfWeek.Sunday };
			public readonly ByteEnum[] byteEnumData = { ByteEnum.Normal, ByteEnum.High };
		}

		private enum ByteEnum : byte
		{
			Normal,
			High,
		}

		private class ClassWithByteArray
		{
			public byte[] data;
		}

		[StructLayout(LayoutKind.Explicit)]
		private class ExplicitLayoutTestClass
		{
			[FieldOffset(0)]
			public int someValue;
			[FieldOffset(4)]
			public int anotherValue;
			[FieldOffset(4)]
			public int unionValue;
		}

		[SaveSafely]
		private class ClassWithAnotherClassInside
		{
			internal class InnerClass
			{
				public double Value { get; set; }
			}

			internal class InnerDerivedClass : InnerClass
			{
				internal bool additionalFlag;
			}

			public int Number { get; set; }

			public InnerDerivedClass Data;
			public InnerDerivedClass SecondInstanceNotSet;
		}

		private class XmlBinaryData
		{
			public XmlBinaryData(string text)
				: this()
			{
				Text = text;
			}

			public string Text { get; private set; }

			private XmlBinaryData() {}
		}

		private class ClassWithMemoryStream
		{
			private ClassWithMemoryStream()
			{
				data = new MemoryStream();
				reader = new BinaryReader(data);
				Writer = new BinaryWriter(data);
			}

			public ClassWithMemoryStream(byte[] bytes)
				: this()
			{
				Writer.Write(bytes);
			}

			public int Length
			{
				get { return (int)data.Length; }
			}
			public int Version { get; set; }
			private readonly MemoryStream data;
			internal readonly BinaryReader reader;
			public BinaryWriter Writer { get; private set; }

			public IEnumerable<byte> Bytes
			{
				get { return data.ToArray(); }
			}
		}

		//ncrunch: no coverage start 
		private class ClassThatRequiresConstructorParameter
		{
			public ClassThatRequiresConstructorParameter(string parameter)
			{
				Assert.NotNull(parameter);
			}
		} //ncrunch: no coverage end
	}
}