using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Simplified Xml handling
	/// </summary>
	[DebuggerDisplay(
		"XmlData: {DeltaEngine.Extensions.StringExtensions.MaxStringLength(ToString(), 200)}")]
	public class XmlData
	{
		static XmlData()
		{
			StringExtensions.AddConvertTypeCreation(typeof(XmlData), value => new XmlSnippet(value).Root);
		}

		protected XmlData()
		{
			Children = new List<XmlData>();
			Attributes = new List<XmlAttribute>();
		}

		public List<XmlData> Children { get; private set; }
		public List<XmlAttribute> Attributes { get; private set; }

		public XmlData(string name)
			: this()
		{
			Name = name;
		}

		public string Name
		{
			get { return name; }
			set
			{
				if (string.IsNullOrEmpty(value) || value.Contains(" "))
					throw new InvalidXmlNameException();
				name = value;
			}
		}

		public class InvalidXmlNameException : Exception {}

		private string name;

		internal XmlData(XElement root)
			: this()
		{
			Name = root.Name.LocalName;
			Value = string.Concat(root.Nodes().OfType<XText>().Select(t => t.Value));
			InitializeAttributes(root);
			InitializeChildren(root);
		}

		public XmlData Parent { get; private set; }
		public string Value { get; set; }

		private void InitializeAttributes(XElement root)
		{
			var attributes = new List<XAttribute>(root.Attributes());
			foreach (XAttribute attribute in attributes)
				Attributes.Add(new XmlAttribute(attribute.Name.LocalName, attribute.Value));
		}

		private void InitializeChildren(XElement root)
		{
			var children = new List<XElement>(root.Elements());
			foreach (XElement childXElement in children)
				AddChild(new XmlData(childXElement));
		}

		public XmlData AddChild(XmlData child)
		{
			if (child == null)
				return this;
			child.Parent = this;
			Children.Add(child);
			return this;
		}

		public XmlData AddChild(string childName, object childValue = null)
		{
			var child = new XmlData(childName);
			if (childValue != null)
				child.Value = StringExtensions.ToInvariantString(childValue);
			AddChild(child);
			return child;
		}

		public XmlData AddAttribute(string attribute, object value)
		{
			Attributes.Add(new XmlAttribute(attribute, value));
			return this;
		}

		public string GetAttributeValue(string attributeName)
		{
			foreach (XmlAttribute attribute in Attributes.Where(a => a.Name.Compare(attributeName)))
				return attribute.Value;
			return "";
		}

		public T GetAttributeValue<T>(string attributeName, T defaultValue)
		{
			foreach (XmlAttribute attribute in Attributes.Where(a => a.Name.Compare(attributeName)))
				return attribute.Value.Convert<T>();
			return defaultValue;
		}

		public void UpdateAttribute(string attributeName, string value)
		{
			if (GetAttributeValue(attributeName) == value)
				return;
			RemoveAttribute(attributeName);
			AddAttribute(attributeName, value);
		}

		public T GetChildValue<T>(string childName, T defaultValue)
		{
			foreach (var child in Children.Where(child => child.Name.Compare(childName)))
				return child.Value.Convert<T>();
			return defaultValue;
		}

		public string GetDescendantValue(string attributeName)
		{
			XmlAttribute? attribute = FindFirstDescendantAttribute(Children, attributeName);
			return attribute == null ? "" : ((XmlAttribute)attribute).Value;
		}

		private static XmlAttribute? FindFirstDescendantAttribute(IEnumerable<XmlData> children,
			string attributeName)
		{
			foreach (XmlData child in children)
			{
				foreach (XmlAttribute attribute in child.Attributes)
					if (attribute.Name.Compare(attributeName))
						return attribute;
				XmlAttribute? childAttribute = FindFirstDescendantAttribute(child.Children, attributeName);
				if (childAttribute != null)
					return childAttribute;
			}
			return null;
		}

		public XmlData GetChild(string childName)
		{
			return Children.FirstOrDefault(child => child.Name.Compare(childName));
		}

		public List<XmlData> GetChildren(string childName)
		{
			return Children.Where(child => child.Name.Compare(childName)).ToList();
		}

		public XmlData GetDescendant(string childName)
		{
			foreach (XmlData child in Children)
			{
				if (child.Name.Compare(childName))
					return child;
				XmlData childOfChild = child.GetChild(childName);
				if (childOfChild != null)
					return childOfChild;
			}
			return null;
		}

		public XmlData GetDescendant(XmlAttribute attribute, string childName = null)
		{
			bool anyDescendant = String.IsNullOrEmpty(childName);
			foreach (XmlData child in Children)
			{
				if ((anyDescendant || child.Name.Compare(childName)) &&
						child.GetAttributeValue(attribute.Name) == attribute.Value)
					return child;
				XmlData childOfChild = child.GetDescendant(attribute, childName);
				if (childOfChild != null)
					return childOfChild;
			}
			return null;
		}

		public XmlData GetDescendant(List<XmlAttribute> attributes, string childName = null)
		{
			bool anyDescendant = String.IsNullOrEmpty(childName);
			foreach (XmlData child in Children)
			{
				if (anyDescendant || child.Name.Compare(childName))
					if (child.ContainsAttributes(attributes))
						return child;
				XmlData childOfChild = child.GetDescendant(attributes, childName);
				if (childOfChild != null)
					return childOfChild;
			}
			return null;
		}

		private bool ContainsAttributes(IEnumerable<XmlAttribute> attributes)
		{
			bool matches = true;
			foreach (var attribute in attributes)
				if (GetAttributeValue(attribute.Name) != attribute.Value)
					matches = false;
			return matches;
		}

		public int GetTotalNodeCount()
		{
			return 1 + Children.Sum(t => t.GetTotalNodeCount());
		}

		public void Remove()
		{
			if (Parent != null)
				Parent.RemoveChild(this);
		}

		public bool RemoveChild(XmlData child)
		{
			return Children.Remove(child);
		}

		public void RemoveAttribute(string attributeName)
		{
			Attributes.RemoveAll(attribute => attribute.Name == attributeName);
		}

		public void ClearAttributes()
		{
			Attributes.Clear();
		}

		public XElement CreateRootXElement(bool createWithDocumentHeader = false)
		{
			var root = new XElement(Name);
			if (!string.IsNullOrEmpty(Value))
				root.Value = Value;
			XDocument doc = createWithDocumentHeader
				? new XDocument(new XDeclaration("1.0", "utf-8", null)) : new XDocument();
			doc.Add(root);
			AddXAttributes(root);
			AddXChildren(root, doc);
			return root;
		}

		private void AddXAttributes(XElement root)
		{
			foreach (XmlAttribute attribute in Attributes)
				root.SetAttributeValue(attribute.Name, attribute.Value);
		}

		private void AddXChildren(XContainer root, XDocument doc)
		{
			foreach (XmlData child in Children)
				root.Add(child.GetXRootElement(doc));
		}

		private XElement GetXRootElement(XDocument doc)
		{
			var root = new XElement(Name);
			if (!string.IsNullOrEmpty(Value))
				root.Value = Value;
			AddXAttributes(root);
			AddXChildren(root, doc);
			return root;
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool createWithDocumentHeader)
		{
			XDocument doc = CreateRootXElement(createWithDocumentHeader).Document;
			return doc == null ? "" : doc.ToString();
		}
	}
}