using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;

namespace DeltaEngine.Content.Xml.Tests
{
	public class ContentTypeIdentifierTests
	{
		[TestCase("Image.png", ContentType.Image)]
		[TestCase("Sound.wav", ContentType.Sound)]
		[TestCase("Music.wma", ContentType.Music)]
		[TestCase("Video.mp4", ContentType.Video)]
		[TestCase("Json.json", ContentType.Json)]
		[TestCase("Particle.deltaparticle", ContentType.ParticleEmitter)]
		[TestCase("Shader.deltashader", ContentType.Shader)]
		[TestCase("Material.deltamaterial", ContentType.Material)]
		[TestCase("Geometry.deltageometry", ContentType.Geometry)]
		[TestCase("Mesh.deltamesh", ContentType.Mesh)]
		[TestCase("Model.fbx", ContentType.Model)]
		[TestCase("Scene.deltascene", ContentType.Scene)]
		[TestCase("Animation.gif", ContentType.JustStore)]
		public void UniqueFileExtensionShouldHaveRespectiveContentTypeAssociated(string filename,
			ContentType expectedContentType)
		{
			ContentType contentTypeFromFileExtension = ContentTypeIdentifier.ExtensionToType(filename);
			Assert.AreEqual(expectedContentType, contentTypeFromFileExtension);
		}

		[TestCase("Font", ContentType.Font)]
		[TestCase("InputCommands", ContentType.InputCommand)]
		[TestCase("Level", ContentType.Level)]
		public void ContentTypeOfXmlFileDependsOnRootElement(string tag,
			ContentType expectedContentType)
		{
			string xmlData = CreateXmlDataFromTag(tag);
			ContentType contentTypeFromXmlRoot = GetContentTypeOfXmlData(xmlData);
			Assert.AreEqual(expectedContentType, contentTypeFromXmlRoot);
		}

		private static string CreateXmlDataFromTag(string tag)
		{
			return "<" + tag + "></" + tag + ">";
		}

		private static ContentType GetContentTypeOfXmlData(string xmlData)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlData)))
			{
				XDocument file = XDocument.Load(stream);
				return ContentTypeIdentifier.DetermineTypeForXmlFile(file);
			}
		}

		[Test]
		public void ContentTypeOfUndefinedRootElementsIsXml()
		{
			string xmlData = CreateXmlDataFromTag("Test");
			ContentType type = GetContentTypeOfXmlData(xmlData);
			Assert.AreEqual(ContentType.Xml, type);
		}

		[Test]
		public void ThrowExceptionIfInvalidXmlData()
		{
			const string InvalidXmlData = "<Opening></Closing>";
			Assert.Throws<XmlException>(() => GetContentTypeOfXmlData(InvalidXmlData));
		}

#if DEBUG
		[Test]
		public void UnsupportedTypeLogsAndThrowsExceptionInDebug()
		{
			Assert.Throws<ContentTypeIdentifier.UnsupportedContentFileFoundCannotParseType>(
				() => ContentTypeIdentifier.ExtensionToType(UnsupportedFileName));
		}
#else
		[Test]
		public void UnsupportedTypeReturnsJustStoreInRelease()
		{
			Assert.AreEqual(ContentType.JustStore,
				ContentTypeIdentifier.ExtensionToType(UnsupportedFileName));
		}
#endif

		private const string UnsupportedFileName = "unsupported";
	}
}