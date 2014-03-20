using System.Globalization;
using System.Windows.Data;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class EnumToBoolConverterTests
	{
		[Test]
		public void ConvertEnumToBoolean()
		{
			var converter = new EnumToBooleanConverter();
			Assert.AreEqual(true, converter.Convert("test", typeof(string), "test", new CultureInfo(1)));
		}

		[Test]
		public void ConvertBooleanToEnum()
		{
			var converter = new EnumToBooleanConverter();
			Assert.AreEqual(Binding.DoNothing,
				converter.ConvertBack("test", typeof(string), "test", new CultureInfo(1)));
		}
	}
}