using System;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class EnumExtensionTests
	{
		[Test]
		public void GetValues()
		{
			Array enumValues = TestEnum.SomeValue.GetEnumValues();
			Assert.AreEqual(2, enumValues.Length);
			Assert.AreEqual(TestEnum.SomeValue, enumValues.GetValue(0));
			Assert.AreEqual(TestEnum.AnotherValue, enumValues.GetValue(1));
		}

		[Test]
		public void GetCount()
		{
			Assert.AreEqual(2, TestEnum.SomeValue.GetCount());
			Assert.AreEqual(2, TestEnum.AnotherValue.GetCount());
			Assert.AreEqual(2, EnumExtensions.GetCount<TestEnum>());
		}

		private enum TestEnum
		{
			SomeValue,
			AnotherValue,
		}

		[Test]
		public void TextToEnum()
		{
			Assert.AreEqual(TestEnum.AnotherValue, "AnotherValue".TryParse(TestEnum.AnotherValue));
			Assert.AreEqual(TestEnum.SomeValue, "InvalidValue".TryParse(TestEnum.SomeValue));
		}

		[Test]
		public void GetIndex()
		{
			Assert.AreEqual(0, EnumExtensions.GetIndex(FlagsEnum.Red));
			Assert.AreEqual(1, EnumExtensions.GetIndex(FlagsEnum.Green));
			Assert.AreEqual(2, EnumExtensions.GetIndex(FlagsEnum.Blue));
			Assert.AreEqual(-1, EnumExtensions.GetIndex((FlagsEnum)17));
		}

		private enum FlagsEnum
		{
			Red = 1,
			Green = 2,
			Blue = 4
		}
	}
}