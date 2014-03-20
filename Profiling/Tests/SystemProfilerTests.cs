using DeltaEngine.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Profiling.Tests
{
	public class SystemProfilerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUpSystemInformation()
		{
			systemInformation = Resolve<SystemInformation>();
		}

		private SystemInformation systemInformation;

		[Test, CloseAfterFirstFrame]
		public void VerifyDefaultProperties()
		{
			Assert.IsFalse(SystemProfiler.Current.IsActive);
			Assert.AreEqual(10, SystemProfiler.Current.MaximumPollsPerSecond);
			Assert.IsTrue(new SystemProfiler().IsActive);
			Assert.AreEqual(10, new SystemProfiler().MaximumPollsPerSecond);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaximumPollsPerSecond()
		{
			var profiler = new SystemProfiler();
			profiler.MaximumPollsPerSecond = 2;
			Assert.AreEqual(2, profiler.MaximumPollsPerSecond);
		}

		[Test, CloseAfterFirstFrame]
		public void LogInfo()
		{
			var profiler = new SystemProfiler();
			profiler.Log(ProfilingMode.Fps, systemInformation);
			SystemProfilerSection results = profiler.GetProfilingResults(ProfilingMode.Fps);
			Assert.IsTrue(results.TotalValue > 0.0f);
			Assert.AreEqual(1, results.Calls);
		}

		[Test, CloseAfterFirstFrame]
		public void LogMultipleSetsOfInfo()
		{
			var profiler = new SystemProfiler();
			profiler.Log(ProfilingMode.Fps | ProfilingMode.AvailableRam, systemInformation);
			Assert.AreEqual(1, profiler.GetProfilingResults(ProfilingMode.Fps).Calls);
			Assert.AreEqual(1, profiler.GetProfilingResults(ProfilingMode.AvailableRam).Calls);
		}

		[Test, CloseAfterFirstFrame]
		public void OnlyProfilesOnceIfTooShortATimeHasPassed()
		{
			int count = 0;
			var profiler = new SystemProfiler();
			profiler.Updated += () => count++;
			profiler.Log(ProfilingMode.Fps, systemInformation);
			profiler.Log(ProfilingMode.Fps, systemInformation);
			Assert.AreEqual(1, profiler.GetProfilingResults(ProfilingMode.Fps).Calls);
			Assert.AreEqual(1, count);
		}

		[Test, CloseAfterFirstFrame]
		public void ProfilesTwiceIfEnoughTimeHasPassed()
		{
			int count = 0;
			var profiler = new SystemProfiler(1000000);
			profiler.Updated += () => count++;
			profiler.Log(ProfilingMode.Fps, systemInformation);
			profiler.Log(ProfilingMode.Fps, systemInformation);
			Assert.AreEqual(2, profiler.GetProfilingResults(ProfilingMode.Fps).Calls);
			Assert.AreEqual(2, count);
		}

		[Test, CloseAfterFirstFrame]
		public void ProfilingWhenInactiveDoesNothing()
		{
			int count = 0;
			var profiler = new SystemProfiler { IsActive = false };
			profiler.Updated += () => count++;
			profiler.Log(ProfilingMode.Fps, systemInformation);
			Assert.AreEqual(0, profiler.GetProfilingResults(ProfilingMode.Fps).Calls);
			Assert.AreEqual(0, count);
		}

		[Test, CloseAfterFirstFrame]
		public void ResetProfilingResult()
		{
			var profiler = new SystemProfiler();
			profiler.Log(ProfilingMode.Fps, systemInformation);
			SystemProfilerSection results = profiler.GetProfilingResults(ProfilingMode.Fps);
			results.Reset();
			Assert.AreEqual(0, results.Calls);
			Assert.AreEqual(0, results.TotalValue);
		}

		[Test, CloseAfterFirstFrame]
		public void CompareProfilerToSameResult()
		{
			var profiler = new SystemProfiler();
			profiler.Log(ProfilingMode.Fps, systemInformation);
			SystemProfilerSection results = profiler.GetProfilingResults(ProfilingMode.Fps);
			Assert.AreEqual(0, results.CompareTo(results));
		}

		[Test, CloseAfterFirstFrame]
		public void CompareProfilerToOtherResult()
		{
			var profiler1 = new SystemProfiler();
			profiler1.Log(ProfilingMode.Fps, systemInformation);
			SystemProfilerSection results1 = profiler1.GetProfilingResults(ProfilingMode.Fps);
			var profiler2 = new SystemProfiler();
			profiler2.Log(ProfilingMode.Engine, systemInformation);
			SystemProfilerSection results2 = profiler2.GetProfilingResults(ProfilingMode.Fps);
			Assert.AreNotEqual(0, results1.CompareTo(results2));
		}
	}
}