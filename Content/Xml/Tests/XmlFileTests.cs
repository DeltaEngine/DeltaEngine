using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class XmlFileTests
	{
		[Test]
		public void XmlDataConstructor()
		{
			var data = new XmlData("name");
			var file = new XmlFile(data);
			Assert.AreEqual(data, file.Root);
		}

		[Test]
		public void LoadXmlFromStream()
		{
			var memoryStream = new MemoryStream();
			var writer = new BinaryWriter(memoryStream);
			writer.Write(new XmlData("MyData").ToString());
			memoryStream.Seek(0, SeekOrigin.Begin);
			var file = new XmlFile(memoryStream);
			Assert.AreEqual("MyData", file.Root.Name);
		}

		[Test]
		public void SavingAndLoadingLeavesItUnchanged()
		{
			XmlData data = CreateTestXmlData();
			var file = new XmlFile(data);
			file.Save("file.xml");
			XmlData loaded = new XmlFile("file.xml").Root;
			Assert.AreEqual(data.ToString(), loaded.ToString());
		}

		private static XmlData CreateTestXmlData()
		{
			var root = new XmlData("Root");
			AddChild1(root);
			AddChild2(root);
			return root;
		}

		private static void AddChild1(XmlData root)
		{
			var child1 = new XmlData("Child1");
			child1.AddAttribute("Attr1", "Value with space");
			child1.AddAttribute("Attr2", "Value2");
			root.AddChild(child1);
		}

		private static void AddChild2(XmlData root)
		{
			var child2 = new XmlData("Child2");
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");
			child2.AddChild(new XmlData("Grandchild"));
			root.AddChild(child2);
		}
	}
}