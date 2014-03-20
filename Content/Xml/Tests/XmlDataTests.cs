using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class XmlDataTests
	{
		[Test]
		public void Constructor()
		{
			var root = new XmlData("name");
			Assert.AreEqual("name", root.Name);
			Assert.AreEqual(0, root.Children.Count);
			Assert.AreEqual(0, root.Attributes.Count);
		}

		[Test]
		public void InvalidName()
		{
			Assert.Throws<XmlData.InvalidXmlNameException>(() => new XmlData(null));
			Assert.Throws<XmlData.InvalidXmlNameException>(() => new XmlData("Hello World"));
		}

		[Test]
		public void GetChild()
		{
			var root = new XmlData("root");
			root.AddChild(new XmlData("child1"));
			var child2 = new XmlData("child2");
			root.AddChild(child2);
			Assert.AreEqual(child2, root.GetChild("child2"));
		}

		[Test]
		public void GetChildren()
		{
			var root = new XmlData("root");
			var child1 = new XmlData("child");
			root.AddChild(child1);
			root.AddChild(new XmlData("stepchild"));
			var child2 = new XmlData("child");
			root.AddChild(child2);
			var children = root.GetChildren("child");
			Assert.AreEqual(2, children.Count);
			Assert.IsTrue(children.Contains(child1));
			Assert.IsTrue(children.Contains(child2));
		}

		[Test]
		public void GetDescendant()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[1], root.GetDescendant(root.Children[1].Name));
			Assert.AreEqual(root.Children[1].Children[0], root.GetDescendant("grandchild"));
			Assert.AreEqual(null, root.GetDescendant("unknown"));
		}

		private static XmlData CreateDeepTestXmlData()
		{
			XmlData root = CreateShallowTestXmlData();
			var grandchild = new XmlData("Grandchild");
			grandchild.AddAttribute("Attr5", "Value5");
			root.Children[1].AddChild(grandchild);
			return root;
		}

		private static XmlData CreateShallowTestXmlData()
		{
			var root = new XmlData("Root");
			var child1 = new XmlData("Child1") { Value = "Tom" };
			child1.AddAttribute("Attr1", "Value1");
			child1.AddAttribute("Attr2", "Value2");
			root.AddChild(child1);
			root.AddChild(null);
			var child2 = new XmlData("Child2");
			child2.AddAttribute("Attr3", "Value3");
			child2.AddAttribute("Attr4", "Value4");
			root.AddChild(child2);
			return root;
		}

		[Test]
		public void AddChildViaNameAndValueWillOnlyAddTheValueIfNotNull()
		{
			var node = new XmlData("Root");
			Assert.IsNull(node.AddChild("Child").Value);
			Assert.IsNull(node.AddChild("Child", null).Value);
			Assert.IsEmpty(node.AddChild("Child", "").Value);
			Assert.AreEqual(4.ToString(), node.AddChild("Child", 4).Value);
		}

		[Test]
		public void GetDescendantWithAttribute()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[0], root.GetDescendant(new XmlAttribute("Attr1", "Value1")));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new XmlAttribute("Attr5", "Value5")));
			Assert.AreEqual(null, root.GetDescendant(new XmlAttribute("Attr5", "Value6")));
			Assert.AreEqual(null, root.GetDescendant(new XmlAttribute("Attr6", "Value5")));
		}

		[Test]
		public void GetDescendantWithAttributeAndName()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(root.Children[0],
				root.GetDescendant(new XmlAttribute("Attr1", "Value1"), root.Children[0].Name));
			Assert.AreEqual(null,
				root.GetDescendant(new XmlAttribute("Attr1", "Value1"), root.Children[1].Name));
		}

		[Test]
		public void GetDescendantWithAttributes()
		{
			XmlData root = CreateDeepTestXmlData();
			var attributes = new List<XmlAttribute>
			{
				new XmlAttribute("Attr1", "Value1"),
				new XmlAttribute("Attr2", "Value2")
			};
			Assert.AreEqual(root.Children[0], root.GetDescendant(attributes));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new List<XmlAttribute> { new XmlAttribute("Attr5", "Value5") }));
			attributes.Add(new XmlAttribute("Attr3", "Value3"));
			Assert.AreEqual(null, root.GetDescendant(attributes));
		}

		[Test]
		public void GetDescendantWithAttributesAndName()
		{
			XmlData root = CreateDeepTestXmlData();
			var attributes = new List<XmlAttribute>
			{
				new XmlAttribute("Attr1", "Value1"),
				new XmlAttribute("Attr2", "Value2")
			};
			Assert.AreEqual(root.Children[0], root.GetDescendant(attributes, "child1"));
			Assert.AreEqual(null, root.GetDescendant(attributes, "child2"));
			Assert.AreEqual(root.Children[1].Children[0],
				root.GetDescendant(new List<XmlAttribute> { new XmlAttribute("Attr5", "Value5") }),
				"child5");
		}

		[Test]
		public void GetTotalNodeCount()
		{
			Assert.AreEqual(3, CreateShallowTestXmlData().GetTotalNodeCount());
			Assert.AreEqual(4, CreateDeepTestXmlData().GetTotalNodeCount());
		}

		[Test]
		public void Remove()
		{
			XmlData root = CreateDeepTestXmlData();
			root.Children[0].Remove();
			Assert.AreEqual(3, root.GetTotalNodeCount());
			root.Children[0].Remove();
			Assert.AreEqual(1, root.GetTotalNodeCount());
		}

		[Test]
		public void RemoveChild()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.IsTrue(root.RemoveChild(root.Children[0]));
			Assert.IsFalse(root.RemoveChild(new XmlData("unknown")));
		}

		[Test]
		public void AddAttributeObject()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", DayOfWeek.Friday);
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", "Friday"), root.Attributes[0]);
		}

		[Test]
		public void AddAttributeChar()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", 'a');
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", 'a'), root.Attributes[0]);
		}

		[Test]
		public void AddAttributeFloat()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", 1.2f);
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", 1.2f), root.Attributes[0]);
		}

		[Test]
		public void AddAttributeDouble()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", 1.2);
			Assert.AreEqual(1, root.Attributes.Count);
			Assert.AreEqual(new XmlAttribute("attribute", 1.2), root.Attributes[0]);
		}

		[Test]
		public void RemoveAttribute()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute1", "value1");
			root.AddAttribute("attribute2", "value2");
			root.AddAttribute("attribute1", "value3");
			root.RemoveAttribute("attribute1");
			Assert.AreEqual(1, root.Attributes.Count);
			root.RemoveAttribute("attribute3");
			Assert.AreEqual(1, root.Attributes.Count);
		}

		[Test]
		public void ClearAttributes()
		{
			XmlData root = CreateShallowTestXmlData();
			XmlData child = root.Children[0];
			Assert.AreEqual(2, child.Attributes.Count);
			child.ClearAttributes();
			Assert.AreEqual(0, child.Attributes.Count);
		}

		[Test]
		public void Value()
		{
			var root = new XmlData("root") { Value = "value" };
			Assert.AreEqual("value", root.Value);
		}

		[Test]
		public void GetDescendantValue()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual("Value5", root.GetDescendantValue("Attr5"));
			Assert.AreEqual("", root.GetDescendantValue("Attr6"));
		}

		[Test]
		public void GetAttributes()
		{
			XmlData root = CreateShallowTestXmlData();
			List<XmlAttribute> attributes = root.Children[0].Attributes;
			Assert.AreEqual(2, attributes.Count);
			Assert.AreEqual("Value1", attributes[0].Value);
			Assert.AreEqual("Value2", attributes[1].Value);
		}

		[Test]
		public void GetAttributeValue()
		{
			var root = new XmlData("root");
			root.AddAttribute("attribute", "value");
			Assert.AreEqual("value", root.GetAttributeValue("attribute"));
			Assert.AreEqual("", root.GetAttributeValue("attribute2"));
		}

		[Test]
		public void GetAttributeValueAsInteger()
		{
			var root = new XmlData("root");
			root.AddAttribute("number", "123");
			Assert.AreEqual(123, root.GetAttributeValue("number", 0));
			Assert.AreEqual("", root.GetAttributeValue("nonexistant", ""));
		}

		[Test]
		public void UpdateExistingAttribute()
		{
			var root = new XmlData("root");
			root.AddAttribute("number", "123");
			root.UpdateAttribute("number", "312");
			Assert.AreEqual("312", root.GetAttributeValue("number"));
		}

		[Test]
		public void UpdateNonExistingAttribute()
		{
			var root = new XmlData("root");
			root.UpdateAttribute("number", "312");
			Assert.AreEqual("312", root.GetAttributeValue("number"));
		}

		[Test]
		public void UpdateExistingAttributeWithSameValue()
		{
			var root = new XmlData("root");
			root.AddAttribute("number", "123");
			root.UpdateAttribute("number", "123");
			Assert.AreEqual("123", root.GetAttributeValue("number"));
		}

		[Test]
		public void GetDefaultChildren()
		{
			var root = new XmlData("root");
			var child1 = new XmlData("Child1") { Value = "testValue" };
			root.AddChild(child1);
			Assert.AreEqual(child1.Value, root.GetChildValue("Child1", root.Value));
			Assert.AreEqual(root.Value, root.GetChildValue("default", root.Value));
		}

		[Test]
		public void ToStringProperty()
		{
			var root = CreateShallowTestXmlData();
			Assert.AreEqual(@"<Child1 Attr1=""Value1"" Attr2=""Value2"">Tom</Child1>",
				root.Children[0].ToString());
		}

		[Test]
		public void ToXmlString()
		{
			XmlData root = CreateDeepTestXmlData();
			Assert.AreEqual(Root, root.ToString());
		}

		private const string Root =
			"<Root>\r\n" +
			"  <Child1 Attr1=\"Value1\" Attr2=\"Value2\">Tom</Child1>\r\n" +
			"  <Child2 Attr3=\"Value3\" Attr4=\"Value4\">\r\n" +
			"    <Grandchild Attr5=\"Value5\" />\r\n" +
			"  </Child2>\r\n" +
			"</Root>";
	}
}