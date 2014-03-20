using System;
using System.Xml;
using DeltaEngine.Editor.Helpers;

namespace DeltaEngine.Editor.Tests.Helpers
{
	public class MockFileReader : FileReader
	{
		public override byte[] ReadAllBytes(string filePath)
		{
			if (filePath.Contains("Large"))
				throw new ContentUploadQueue.FileExceedsMaximumSize(filePath, 16 * 1024 * 1024);
			if (!filePath.Contains("Corrupt"))
				return new byte[] { 1, 2, 3, 4, 5 };
			if (filePath.EndsWith(".xml"))
				throw new XmlException(filePath);
			throw new FileCorrupt(filePath);
		}

		private class FileCorrupt : Exception
		{
			public FileCorrupt(string contentFilePath)
				: base(contentFilePath) {}
		}
	}
}