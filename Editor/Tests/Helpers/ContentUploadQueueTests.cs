using System.Collections.Generic;
using System.IO;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests.Helpers
{
	public class ContentUploadQueueTests
	{
		[SetUp]
		public void CreateContentUploadQueue()
		{
			FileReader fileReader = StackTraceExtensions.IsStartedFromNCrunch()
				? new MockFileReader() : new FileReader();
			contentUploadQueue = new MockContentUploadQueue(fileReader,
				(metaData, fileBytes) => uploadedContentFiles++);
		}

		private MockContentUploadQueue contentUploadQueue;
		private int uploadedContentFiles;

		[TearDown]
		public void ResetNumberOfSentFiles()
		{
			uploadedContentFiles = 0;
		}

		[Test]
		public void EnqueueOneFileAndUploadToServer()
		{
			Assert.AreEqual(0, contentUploadQueue.QueuedFiles);
			Assert.AreEqual(0, uploadedContentFiles);
			contentUploadQueue.EnqueueFile(filePaths[0]);
			Assert.AreEqual(0, contentUploadQueue.QueuedFiles);
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[0]));
			Assert.AreEqual(1, uploadedContentFiles);
		}

		private readonly string[] filePaths =
		{
			@"C:\code\DeltaEngine\Samples\LogoApp\bin\Debug\Content\DeltaEngineLogo.png",
			@"C:\code\DeltaEngine\Samples\GhostWars\bin\Debug\Content\Verdana12.xml",
			@"C:\code\DeltaEngine\Samples\GhostWars\bin\Debug\Content\Verdana12Font.png"
		};

		[Test]
		public void EnqueueMultipleFilesAndUploadFirstToServer()
		{
			Assert.AreEqual(0, contentUploadQueue.QueuedFiles);
			Assert.AreEqual(0, uploadedContentFiles);
			contentUploadQueue.EnqueueFiles(filePaths);
			Assert.AreEqual(filePaths.Length - 1, contentUploadQueue.QueuedFiles);
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[0]));
			Assert.AreEqual(1, uploadedContentFiles);
		}

		[Test]
		public void UploadSecondAndFurtherFilesAfterServerHasFinishedProcessingThePreviousFile()
		{
			contentUploadQueue.EnqueueFiles(filePaths);
			ProcessFilesOnService();
			Assert.AreEqual(filePaths.Length - 2, contentUploadQueue.QueuedFiles);
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[1]));
			Assert.AreEqual(2, uploadedContentFiles);
			ProcessFilesOnService();
			Assert.AreEqual(filePaths.Length - 3, contentUploadQueue.QueuedFiles);
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[2]));
			Assert.AreEqual(3, uploadedContentFiles);
		}

		private void ProcessFilesOnService(int numberOfFilesToProcess = 1)
		{
			for (int i = 0; i < numberOfFilesToProcess; i++)
				contentUploadQueue.Next();
		}

		[Test]
		public void AfterAllFilesHaveBeenUploadedAndProcessedTheQueueIsReset()
		{
			Assert.IsTrue(contentUploadQueue.IsReset);
			contentUploadQueue.EnqueueFile(filePaths[0]);
			Assert.IsFalse(contentUploadQueue.IsReset);
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.IsReset);
			contentUploadQueue.EnqueueFiles(filePaths);
			Assert.IsFalse(contentUploadQueue.IsReset);
			ProcessFilesOnService();
			Assert.IsFalse(contentUploadQueue.IsReset);
			ProcessFilesOnService();
			Assert.IsFalse(contentUploadQueue.IsReset);
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.IsReset);
			Assert.AreEqual(4, uploadedContentFiles);
		}

		[Test]
		public void ExceedingEnqueuedFilesThrows()
		{
			contentUploadQueue.EnqueueFile(filePaths[0]);
			ProcessFilesOnService();
			Assert.Throws<ContentUploadQueue.NoMoreFilesToUpload>(() => contentUploadQueue.Next());
			contentUploadQueue.EnqueueFiles(filePaths);
			ProcessFilesOnService(3);
			Assert.Throws<ContentUploadQueue.NoMoreFilesToUpload>(() => contentUploadQueue.Next());
			Assert.AreEqual(4, uploadedContentFiles);
		}

		[Test]
		public void CorruptedFilesAreSkippedAndNextFileIsUploaded()
		{
			var pathsWithCorruptedFiles = new List<string>
			{
				filePaths[0],
				CorruptFilePath,
				filePaths[1],
				CorruptXmlFilePath,
				filePaths[2]
			};
			contentUploadQueue.EnqueueFiles(pathsWithCorruptedFiles);
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[0]));
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[1]));
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.IsLastFileUploaded(filePaths[2]));
			ProcessFilesOnService();
			Assert.AreEqual(3, uploadedContentFiles);
		}

		private const string CorruptFilePath = @"C:\code\CorruptDeltaEngineLogo.png";
		private const string CorruptXmlFilePath = @"C:\code\CorruptFont.xml";

		[Test]
		public void MaximumFileSizeIs16Mb()
		{
			contentUploadQueue.EnqueueFile(LargeFilePath);
			Assert.AreEqual(0, contentUploadQueue.QueuedFiles);
			Assert.IsFalse(contentUploadQueue.IsLastFileUploaded(LargeFilePath));
			Assert.AreEqual(0, uploadedContentFiles);
		}

		private const string LargeFilePath = @"C:\code\LargeDeltaEngineLogo.png";

		[Test]
		public void ServerErrorContainsFilenameOfRejectedContentByService()
		{
			var filePathsWithRejectedFiles = new List<string>
			{
				filePaths[0],
				RejectedLogo,
				filePaths[1],
				RejectedFont,
				filePaths[2]
			};
			contentUploadQueue.EnqueueFiles(filePathsWithRejectedFiles);
			Assert.IsFalse(contentUploadQueue.ContainsLastFileUploaded(CreateSuccessfulMessage()));
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.ContainsLastFileUploaded(BuildRejectedMessage(RejectedLogo)));
			ProcessFilesOnService();
			Assert.IsFalse(contentUploadQueue.ContainsLastFileUploaded(CreateSuccessfulMessage()));
			ProcessFilesOnService();
			Assert.IsTrue(contentUploadQueue.ContainsLastFileUploaded(BuildRejectedMessage(RejectedFont)));
			ProcessFilesOnService();
			Assert.IsFalse(contentUploadQueue.ContainsLastFileUploaded(CreateSuccessfulMessage()));
			ProcessFilesOnService();
			Assert.AreEqual(5, uploadedContentFiles);
		}

		private const string RejectedLogo = @"C:\code\RejectedDeltaEngineLogo.png";
		private const string RejectedFont = @"C:\code\RejectedVerdana12.xml";

		private static string CreateSuccessfulMessage()
		{
			return "File has successfully been processed";
		}

		private static string BuildRejectedMessage(string filePath)
		{
			return "File " + Path.GetFileName(filePath) + " has been rejected by the Content Server";
		}
	}
}