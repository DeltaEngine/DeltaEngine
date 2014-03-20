using System;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class XmlAttributeTests
	{
		[Test]
		public void ConstructorWithObject()
		{
			var attribute = new XmlAttribute("name", DayOfWeek.Friday);
			Assert.AreEqual("name", attribute.Name);
			Assert.AreEqual("Friday", attribute.Value);
		}

		[Test]
		public void ConstructorWithFloat()
		{
			var attribute = new XmlAttribute("name", 2.1f);
			Assert.AreEqual("name", attribute.Name);
			Assert.AreEqual("2.1", attribute.Value);
		}

		[Test]
		public void ConstructorWithDouble()
		{
			var attribute = new XmlAttribute("name", 3.14);
			Assert.AreEqual("name", attribute.Name);
			Assert.AreEqual("3.14", attribute.Value);
		}

		[Test]
		public void ConstructorWithChar()
		{
			var attribute = new XmlAttribute("name", 'a');
			Assert.AreEqual("name", attribute.Name);
			Assert.AreEqual("a", attribute.Value);
		}
	}
}