using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content.Disk
{
	/// <summary>
	/// Creates the ContentMetaData file to be used in ContentLoader based on the local files on disk.
	/// </summary>
	public class ContentMetaDataFileCreator
	{
		//ncrunch: no coverage start
		public ContentMetaDataFileCreator(XDocument lastXml)
		{
			this.lastXml = lastXml;
		}

		private readonly XDocument lastXml;

		public XDocument CreateAndLoad(string xmlFilePath)
		{
			filePath = xmlFilePath;
			WriteMetaDataDocument();
			var document = XDocument.Load(filePath);
			document = new CompoundContentCreator().AddCompoundElementsToDocument(document);
			document.Save(filePath);
			return document;
		}

		private string filePath;

		private void WriteMetaDataDocument()
		{
			using (var writer = CreateMetaDataXmlFile())
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("ContentMetaData");
				WriteAttribute(writer, "Name", AssemblyExtensions.GetEntryAssemblyForProjectName());
				WriteAttribute(writer, "Type", "Scene");
				WriteAttribute(writer, "LastTimeUpdated",
					Directory.GetLastWriteTime(Directory.GetCurrentDirectory()).GetIsoDateTime());
				WriteAttribute(writer, "ContentDeviceName", "auto-generated");
				foreach (var file in Directory.GetFiles(Path.GetDirectoryName(filePath)))
					if (!IsFilenameIgnored(Path.GetFileName(file)))
						CreateContentMetaDataEntry(writer, file);
				writer.WriteEndElement();
			}
		}

		private XmlTextWriter CreateMetaDataXmlFile()
		{
			Stream stream = File.Create(filePath);
			return new XmlTextWriter(stream, new UTF8Encoding()) { Formatting = Formatting.Indented };
		}

		private static void WriteAttribute(XmlWriter writer, string name, string value)
		{
			writer.WriteStartAttribute(name);
			writer.WriteValue(value);
			writer.WriteEndAttribute();
		}

		private bool IsFilenameIgnored(string fileName)
		{
			return ignoredFiles.Contains(Path.GetFileName(fileName).ToLower()) ||
				fileName.ToLower().EndsWith(".xnb");
		}

		private readonly List<string> ignoredFiles =
			new List<string>(new[] { "contentmetadata.xml", "thumbs.db", "desktop.ini", ".ds_store" });

		private void CreateContentMetaDataEntry(XmlWriter writer, string contentFile)
		{
			writer.WriteStartElement("ContentMetaData");
			var contentType = ContentTypeIdentifier.ExtensionToType(contentFile);
			var contentName = Path.GetFileNameWithoutExtension(contentFile);
			WriteAttribute(writer, "Name", contentName);
			WriteAttribute(writer, "Type", contentType.ToString());
			WriteMetaData(writer, contentFile, contentType);
			writer.WriteEndElement();
		}

		private void WriteMetaData(XmlWriter writer, string contentFile, ContentType type)
		{
			var info = new FileInfo(contentFile);
			WriteAttribute(writer, "LastTimeUpdated", info.LastWriteTime.GetIsoDateTime());
			WriteAttribute(writer, "PlatformFileId", ++generatedPlatformFileId + "");
			WriteAttribute(writer, "FileSize", "" + info.Length);
			WriteAttribute(writer, "LocalFilePath", Path.GetFileName(contentFile));
			if (type == ContentType.Image)
				GeneratePixelSizeAndBlendModeIfNeeded(writer, contentFile);
		}

		private int generatedPlatformFileId;

		private void GeneratePixelSizeAndBlendModeIfNeeded(XmlWriter writer, string imageFilePath)
		{
			if (lastXml != null &&
				CanWriteImageDataFromLastXml(writer, lastXml.Root, Path.GetFileName(imageFilePath)))
				return;
			WriteImageDataFromBitmap(writer, imageFilePath);
		}

		private static bool CanWriteImageDataFromLastXml(XmlWriter writer, XElement element,
			string imageFilename)
		{
			foreach (var child in element.Elements())
			{
				if (child.Attribute("LocalFilePath") != null &&
					child.Attribute("LocalFilePath").Value == imageFilename &&
					child.Attribute("PixelSize") != null && child.Attribute("BlendMode") != null)
				{
					WriteAttribute(writer, "PixelSize", child.Attribute("PixelSize").Value);
					WriteAttribute(writer, "BlendMode", child.Attribute("BlendMode").Value);
					return true;
				}

				if (CanWriteImageDataFromLastXml(writer, child, imageFilename))
					return true;
			}
			return false;
		}

		private static void WriteImageDataFromBitmap(XmlWriter writer, string filePath)
		{
			try
			{
				TryWriteImageDataFromBitmap(writer, filePath);
			}
			catch (Exception)
			{
				throw new UnknownImageFormatUnableToAquirePixelSize(filePath);
			}
		}

		private static void TryWriteImageDataFromBitmap(XmlWriter writer, string filePath)
		{
			using (var bitmap = new Bitmap(filePath))
			{
				WriteAttribute(writer, "PixelSize", bitmap.Width + ", " + bitmap.Height);
				if (GetBlendMode(bitmap, filePath).ToString() != "Normal")
					WriteAttribute(writer, "BlendMode", GetBlendMode(bitmap, filePath).ToString());
			}
		}

		private class UnknownImageFormatUnableToAquirePixelSize : Exception
		{
			public UnknownImageFormatUnableToAquirePixelSize(string message)
				: base(message) {}
		}

		private static BlendMode GetBlendMode(Bitmap bitmap, string filePath)
		{
			var mode = BlendMode.Opaque;
			if (!filePath.EndsWith(".jpg") && HasBitmapAlphaPixels(bitmap))
				mode = BlendMode.Normal;
			string contentName = Path.GetFileNameWithoutExtension(filePath);
			if (contentName.EndsWith("Additive"))
				mode = BlendMode.Additive;
			else if (contentName.EndsWith("AlphaTest"))
				mode = BlendMode.AlphaTest;
			else if (contentName.EndsWith("Subtractive"))
				mode = BlendMode.Subtractive;
			else if (contentName.EndsWith("LightEffect"))
				mode = BlendMode.LightEffect;
			return mode;
		}

		internal static unsafe bool HasBitmapAlphaPixels(Bitmap bitmap)
		{
			int width = bitmap.Width;
			int height = bitmap.Height;
			var bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
				PixelFormat.Format32bppArgb);
			var bitmapPointer = (byte*)bitmapData.Scan0.ToPointer();
			var foundAlphaPixel = HasImageDataAlpha(width, height, bitmapPointer);
			bitmap.UnlockBits(bitmapData);
			return foundAlphaPixel;
		}

		private static unsafe bool HasImageDataAlpha(int width, int height, byte* bitmapPointer)
		{
			for (int y = 0; y < height; ++y)
				for (int x = 0; x < width; ++x)
					if (bitmapPointer[(y * width + x) * 4 + 3] != 255)
						return true;
			return false;
		}
	}
}