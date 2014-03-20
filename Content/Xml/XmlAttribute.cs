using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Holds a name and value text pair
	/// </summary>
	public struct XmlAttribute
	{
		public XmlAttribute(string name, object value)
			: this()
		{
			Name = name;
			Value = StringExtensions.ToInvariantString(value);
		}

		public readonly string Name;
		public readonly string Value;
	}
}