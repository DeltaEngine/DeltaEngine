using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Editor.Converters.Tests
{
	public class SizeStringConverterTests
	{
		[SetUp]
		public void Init()
		{
			converter = new SizeStringConverter();
		}

		private SizeStringConverter converter;

		[Test]
		public void ConvertShouldReturnStringWhenPassingInASize()
		{
			Assert.AreEqual("2, 3", converter.Convert(new Size(2.0f, 3.0f), typeof(string), null, null));
		}

		[Test]
		public void ConvertShouldReturnEmptyStringWhenNotPassinInASize()
		{
			Assert.AreEqual("", converter.Convert(null, typeof(string), null, null));
			Assert.AreEqual("", converter.Convert(new object(), typeof(string), null, null));
		}

		[Test]
		public void ConvertBackShouldReturnObjectWhenPassingInAString()
		{
			Assert.AreEqual(new Size(2, 3), converter.ConvertBack("2, 3", typeof(SizeStringConverter), null, null));
		}

		[Test]
		public void ConvertBackShouldReturnNullWhenPassingInAnEmptyString()
		{
			Assert.IsNull(converter.ConvertBack("", typeof(SizeStringConverter), null, null));
		}

		[Test]
		public void ConvertBackShouldReturnNullWhenPassingInANotParsableString()
		{
			Assert.IsNull(converter.ConvertBack("abc", typeof(SizeStringConverter), null, null));
			Assert.IsNull(converter.ConvertBack("1, abc", typeof(SizeStringConverter), null, null));
			Assert.IsNull(converter.ConvertBack("3", typeof(SizeStringConverter), null, null));
			Assert.IsNull(converter.ConvertBack("0.a, 4", typeof(SizeStringConverter), null, null));
			Assert.IsNull(converter.ConvertBack("5., 2", typeof(SizeStringConverter), null, null));
		}
	}
}