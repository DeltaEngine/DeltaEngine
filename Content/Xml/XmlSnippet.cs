using System.Xml.Linq;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Creates XmlData from a string
	/// </summary>
	public class XmlSnippet
	{
		public XmlSnippet(string xmlAsText)
		{
			xmlAsText = RemoveLeadingJunk(xmlAsText);
			XDocument xDocument = XDocument.Parse(xmlAsText);
			Root = new XmlData(xDocument.Root);
		}

		public XmlData Root { get; private set; }

		private static string RemoveLeadingJunk(string xmlAsText)
		{
			int cutOffIndex = xmlAsText.IndexOf('<');
			return cutOffIndex > 0 ? xmlAsText.Substring(cutOffIndex) : xmlAsText;
		}
	}
}