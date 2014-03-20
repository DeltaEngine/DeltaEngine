using System;
using DeltaEngine.Core;
using DeltaEngine.Logging.Tests.LogService;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Ignore]
	public class LogServiceTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void CreateLogService()
		{
			service = new LogServiceSoapClient();
		}

		private LogServiceSoapClient service;

		[TearDown]
		public void DisposeLogService()
		{
			service.Close();
		}

		[Test]
		public void WriteInfo()
		{
			service.Log(Logger.MessageType.Info.ToString(), "Hello", "DeltaEngine.Logging.Tests", "");
		}

		[Test]
		public void LogWarning()
		{
			service.Log(Logger.MessageType.Warning.ToString(), "Ohoh", "DeltaEngine.Logging.Tests", "");
		}

		[Test]
		public void LogError()
		{
			service.Log(Logger.MessageType.Error.ToString(), new NotSupportedException().ToString(),
				"DeltaEngine.Logging.Tests", "");
		}
	}
}