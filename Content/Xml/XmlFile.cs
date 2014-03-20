using System.IO;
using System.Text;
using System.Xml.Linq;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Loads and saves XmlData to file
	/// </summary>
	public class XmlFile
	{
		public XmlFile(XmlData xmlData)
		{
			Root = xmlData;
		}

		public XmlData Root { get; private set; }

		public XmlFile(string filePath)
		{
			using (var s = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				Root = new XmlData(XDocument.Load(s).Root);
		}

		public XmlFile(Stream fileStream)
		{
			Root = new XmlData(XDocument.Load(fileStream).Root);
		}

		public void Save(string filePath, bool createWithDocumentHeader = false)
		{
			using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
				FileShare.ReadWrite))
			using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
				Root.CreateRootXElement(createWithDocumentHeader).Document.Save(writer);
		}

		public MemoryStream ToMemoryStream()
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream, new UTF8Encoding(false));
			Root.CreateRootXElement().Document.Save(writer);
			stream.Position = 0;
			return stream;
		}
	}
}