using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Content.Xml
{
	public class ContentTypeIdentifier
	{
		public static ContentType ExtensionToType(string filename)
		{
			string extension = Path.GetExtension(filename);
			switch (extension.ToLower())
			{
			case ".png":
			case ".jpg":
			case ".bmp":
			case ".tif":
				return ContentType.Image;
			case ".wav":
				return ContentType.Sound;
			case ".mp3":
			case ".ogg":
			case ".wma":
				return ContentType.Music;
			case ".mp4":
			case ".avi":
			case ".wmv":
				return ContentType.Video;
			case ".xml":
				return DetermineTypeForXmlFile(XDocument.Load(filename)); //ncrunch: no coverage
			case ".json":
				return ContentType.Json;
			case ".deltaparticle":
				return ContentType.ParticleEmitter;
			case ".deltashader":
				return ContentType.Shader;
			case ".deltamaterial":
				return ContentType.Material;
			case ".deltageometry":
				return ContentType.Geometry;
			case ".deltamesh":
				return ContentType.Mesh;
			case ".fbx":
				return ContentType.Model;
			case ".deltascene":
				return ContentType.Scene;
			case ".gif":
			case ".atlas":
			case ".txt":
				return ContentType.JustStore;
			default:
				LogWarningAndThrowInDebug(filename, extension);
				return ContentType.JustStore; //ncrunch: no coverage
			}
		}

		[Conditional("DEBUG")]
		private static void LogWarningAndThrowInDebug(string filename, string extension)
		{
			Logger.Warning("Unknown content type, unable to proceed: " + Path.GetFileName(filename));
			throw new UnsupportedContentFileFoundCannotParseType(extension);
		}

		public class UnsupportedContentFileFoundCannotParseType : Exception
		{
			public UnsupportedContentFileFoundCannotParseType(string extension)
				: base(extension) {}
		}

		public static ContentType DetermineTypeForXmlFile(XDocument xmlData)
		{
			var rootName = xmlData.Root.Name.ToString();
			if (rootName == "Font")
				return ContentType.Font;
			if (rootName == "InputCommands")
				return ContentType.InputCommand;
			if (rootName == "Level")
				return ContentType.Level;
			return ContentType.Xml;
		}
	}
}