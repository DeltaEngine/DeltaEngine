using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class XmlSnippetTests
	{
		[Test]
		public void ToAndFromXmlSnippetLeavesTextUnchanged()
		{
			var snippet = CreateTestXmlData().ToString();
			var xmlSnippet = new XmlSnippet(snippet);
			Assert.AreEqual(snippet, xmlSnippet.Root.ToString());
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

		[Test]
		public void IgnoresLeadingJunk()
		{
			var snippet = CreateTestXmlData().ToString();
			var xmlSnippet = new XmlSnippet("blahblah" + snippet);
			Assert.AreEqual(snippet, xmlSnippet.Root.ToString());
		}
	}
}