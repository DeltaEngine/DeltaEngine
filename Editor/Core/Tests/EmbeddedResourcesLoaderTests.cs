using System;
using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	[Category("Slow")]
	public class EmbeddedResourcesLoaderTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void CopyEditorExeToTestOutput()
		{
			var currentDirectory = Directory.GetCurrentDirectory();
			var editorExePath = GetEditorOutputPath(currentDirectory);
			outputPath = Path.Combine(currentDirectory, EditorExeName);
			File.Copy(editorExePath, outputPath, true);
		}

		private static string GetEditorOutputPath(string directory)
		{
			var editorDirectory = GetEditorDirectoryInfo(new DirectoryInfo(directory));
			var configuration = ExceptionExtensions.IsDebugMode ? "Debug" : "Release";
			return Path.Combine(editorDirectory.FullName, "bin", configuration, EditorExeName);
		}

		private static DirectoryInfo GetEditorDirectoryInfo(DirectoryInfo directoryInfo)
		{
			if (directoryInfo == null)
				throw new EditorDirectoryNotFound();
			if (directoryInfo.Name == "Editor")
				return directoryInfo;
			return GetEditorDirectoryInfo(directoryInfo.Parent);
		}

		private class EditorDirectoryNotFound : Exception {}

		private const string EditorExeName = "DeltaEngine.Editor.exe";

		private string outputPath;

		[Test]
		public void GetEmbeddedResourceThatDoesNotExistShouldThrowException()
		{
			Assert.Throws<EmbeddedResourcesLoader.EmbeddedResourceNotFound>(
				() => EmbeddedResourcesLoader.GetFullResourceName("NotAvailable"));
		}

		[Test]
		public void GetExistingEmbeddedResourceName()
		{
			const string ExpectedResourcePath = "DeltaEngine.Editor.Images.Emulators.Devices.xml";
			Assert.AreEqual(ExpectedResourcePath,
				EmbeddedResourcesLoader.GetFullResourceName(ResourceName));
		}

		private const string ResourceName = "Images.Emulators.Devices.xml";

		[Test]
		public void GetEmbeddedResourceStream()
		{
			Stream fileStream = EmbeddedResourcesLoader.GetEmbeddedResourceStream(ResourceName);
			Assert.Greater(fileStream.Length, 0);
			fileStream.Dispose();
		}
	}
}