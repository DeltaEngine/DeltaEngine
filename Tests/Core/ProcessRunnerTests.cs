using System;
using DeltaEngine.Core;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Tests.Core
{
	[Ignore]
	public class ProcessRunnerTests
	{
		//ncrunch: no coverage start, dangerous to execute with ncrunch as we have to redirect output
		[Test]
		public void DefaultWorkingDirectory()
		{
			var processRunner = new ProcessRunner("cmd.exe", "/c dir");
			Assert.AreEqual(Environment.CurrentDirectory, processRunner.WorkingDirectory);
		}

		[Test]
		public void ChangingWorkingDirectory()
		{
			var processRunner = new ProcessRunner("cmd.exe", "/c dir");
			processRunner.Start();
			var outputWithDefaultWorkingDirectory = processRunner.Output;
			processRunner.WorkingDirectory = @"C:\";
			processRunner.Start();
			var outputWithDefinedWorkingDirectory = processRunner.Output;
			Assert.AreNotEqual(outputWithDefaultWorkingDirectory, outputWithDefinedWorkingDirectory);
		}

		[Test]
		public void StandardOutputEvent()
		{
			var logger = new MockLogger();
			var processRunner = new ProcessRunner("cmd.exe", "/c dir");
			processRunner.StandardOutputEvent +=
				outputMessage => logger.Write(Logger.MessageType.Info, outputMessage);
			processRunner.Start();
			Assert.IsTrue(
				logger.LastMessage.Contains("Dir(s)") || logger.LastMessage.Contains("Verzeichnis(se)"),
				logger.LastMessage);
		}

		[Test]
		public void TestTimeout()
		{
			var processRunner = new ProcessRunner("cmd.exe", "/c dir", 1);
			Assert.Throws<ProcessRunner.StandardOutputHasTimedOutException>(processRunner.Start);
		}
	}
}