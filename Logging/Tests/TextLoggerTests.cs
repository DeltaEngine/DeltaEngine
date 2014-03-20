using System;
using System.ComponentModel;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class TextLoggerTests
	{
		[Test]
		public void LogInfo()
		{
			using (var logger = new TextLogger())
			{
				bool newLogMessageArrived = false;
				logger.NewLogMessage += () => newLogMessageArrived = true;
				Assert.IsFalse(newLogMessageArrived);
				Logger.Info("Message");
				Assert.IsTrue(newLogMessageArrived);
				Assert.IsTrue(logger.Log.EndsWith("Message"), logger.Log);
			}
		}
		
		[Test]
		public void LogWarning()
		{
			using (var logger = new TextLogger())
			{
				Logger.Warning("Warning");
				Logger.Warning(new WarningException(""));
				Assert.IsTrue(logger.Log.EndsWith(typeof(WarningException).ToString()), logger.Log);
			}
		}

		[Test]
		public void LogError()
		{
			using (var logger = new TextLogger())
			{
				Logger.Error(new TestError(""));
				Assert.IsTrue(logger.Log.Contains(typeof(TestError).ToString()), logger.Log);
			}
		}

		private class TestError : Exception
		{
			public TestError(string empty)
				: base(empty) {}
		}
	}
}