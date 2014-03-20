using System;
using System.IO;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	public class MyDocumentsExtensionsTests
	{
		[SetUp]
		public void Init()
		{
			myDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}

		private string myDocumentsDirectory;

		[Test]
		public void CheckCurrentVisualStudioMyDocumentsFolder()
		{
			string myVisualStudioDirectory = Path.Combine(myDocumentsDirectory, LatestVisualStudioName);
			Assert.AreEqual(myVisualStudioDirectory,
				MyDocumentsExtensions.GetSupportedVisualStudioFolders()[0]);
		}

		private const string LatestVisualStudioName = "Visual Studio 2013";

		[Test]
		public void CheckVisualStudioProjectsFolder()
		{
			string expectedPath = Path.Combine(myDocumentsDirectory, LatestVisualStudioName, "Projects");
			string actualPath = MyDocumentsExtensions.GetVisualStudioProjectsFolder();
			Assert.AreEqual(expectedPath, actualPath);
		}

		[Test]
		public void CheckVisualStudioDeltaEngineTemplateZip()
		{
			string expectedFilePath = Path.Combine(myDocumentsDirectory, LatestVisualStudioName,
				"Templates", "ProjectTemplates", "Visual C#", "Delta Engine", "EmptyApp.zip");
			string actualFilePath =
				MyDocumentsExtensions.GetVisualStudioDeltaEngineTemplateZip("EmptyApp");
			Assert.AreEqual(expectedFilePath, actualFilePath);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void BothProjectsAndDeltaEngineProjectTemplatesShouldBeAvailable()
		{
			Assert.DoesNotThrow(() => MyDocumentsExtensions.GetVisualStudioProjectsFolder());
			Assert.DoesNotThrow(() => MyDocumentsExtensions.GetVisualStudioDeltaEngineTemplateZip(""));
		}

		[Test, Ignore]
		public void LogAvailableMyDocumentsFoldersForVisualStudio()
		{
			new ConsoleLogger();
			foreach (var supportedVsFolder in MyDocumentsExtensions.GetSupportedVisualStudioFolders())
				if (Directory.Exists(supportedVsFolder))
					Logger.Info(supportedVsFolder + " exists");
				else
					Logger.Info(supportedVsFolder + " does not exist");
		}
	}
}