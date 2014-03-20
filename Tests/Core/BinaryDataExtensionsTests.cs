using System;
using System.Text;
using System.Dynamic;
using System.IO;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Tests.Core
{
	public class BinaryDataExtensionsTests
	{
		[Test]
		public void DynamicallyCreatedTypeIsNotStored()
		{
			var unknownObject = new ExpandoObject();
			Assert.Throws<BinaryDataExtensions.NoShortNameStoredFor>(
				() => BinaryDataExtensions.GetShortName(unknownObject));
		}

		[Test]
		public void BuiltInTypeGetShortName()
		{
			const int Value = 0;
			Assert.AreEqual("Int32", BinaryDataExtensions.GetShortName(Value));
		}

		[Test]
		public void CannotSaveObjectDataIfNull()
		{
			var binaryWriter = new BinaryWriter(new MemoryStream());
			Assert.Throws<ArgumentNullException>(() => BinaryDataExtensions.Save(null, binaryWriter));
		}

		[Test]
		public void ThereMustBeSomeDataToReadInStream()
		{
			var binaryReader = new BinaryReader(new MemoryStream());
			Assert.Throws<BinaryDataExtensions.NotEnoughDataLeftInStream>(() => binaryReader.Create());
		}

		[Test]
		public void LoadKnownTypeWithoutVersionCheck()
		{
			const int Value = 100;
			var memoryStream = new MemoryStream(new byte[4]);
			var binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(Value);
			memoryStream.Position = 0;
			var binaryReader = new BinaryReader(memoryStream);
			int loadedValue = binaryReader.LoadKnownTypeWithoutVersionCheck<int>();
			Assert.AreEqual(Value, loadedValue);
		}

		[Test]
		public void ExpectExceptionForUnreadableData()
		{
			var message = new TestMessage { content = "This message cannot be resolved!", ID = 0 };
			var memoryStream = new MemoryStream(new byte[64]);
			var binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write(BinaryDataExtensions.ToByteArray(message));
			memoryStream.Position = 0;
			var binaryReader = new BinaryReader(memoryStream);
			Assert.Throws<TypeLoadException>(() => binaryReader.Create());
		}

		[Test]
		public void ObjectToBinaryData()
		{
			string message = "This message cannot be resolved!";
			byte [] data = BinaryDataExtensions.ToByteArrayWithLengthHeader(message);
			Assert.AreEqual(data[0], 44);
		}

		private struct TestMessage
		{
			public int ID;
			public string content;
		}
	}
}