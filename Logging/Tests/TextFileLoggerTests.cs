using System;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Category("Slow")]
	public class TextFileLoggerTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void CreateProvider()
		{
			if (File.Exists(LogFilePath))
				File.Delete(LogFilePath);

			logger = new TextFileLogger();
		}

		private static string LogFilePath
		{
			get { return Path.Combine(Settings.GetMyDocumentsAppFolder(), "Log.txt"); }
		}

		private TextFileLogger logger;

		[TearDown]
		public void DisposeProvider()
		{
			logger.Dispose();
		}

		[Test]
		public void WhenThereWasNoLoggingNoFileIsCreated()
		{
			Assert.IsFalse(File.Exists(LogFilePath));
		}

		[Test]
		public void LogInfoAndOpenFile()
		{
			logger.Write(Logger.MessageType.Info, "Test for logging info");
		}

		[Test]
		public void LogTwoMessagesAndOpenFile()
		{
			logger.Write(Logger.MessageType.Info, "Info message 1");
			logger.Write(Logger.MessageType.Info, "Info message 2");
		}

		[Test]
		public void LogWarningAndOpenFile()
		{
			logger.Write(Logger.MessageType.Warning, "Something strange happened");
		}

		[Test]
		public void LogErrorAndOpenFile()
		{
			logger.Write(Logger.MessageType.Error, new InsufficientMemoryException().ToString());
		}
	}
}