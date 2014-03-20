using System.Collections.Generic;
using System.Threading;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Profiling.Tests
{
	public class CodeProfilerTests
	{
		[Test]
		public void VerifyStaticProfilerCurrentIsFalse()
		{
			var profiler = CodeProfiler.Current as CodeProfiler;
			Assert.IsFalse(profiler.IsActive);
		}

		[Test]
		public void VerifyStaticProfilerDefaultProperties()
		{
			var profiler = CodeProfiler.Current as CodeProfiler;
			Assert.AreEqual(10, profiler.MaximumPollsPerSecond);
			Assert.AreEqual(0, profiler.ResetInterval);
			Assert.IsFalse(profiler.IsActive);
		}

		[Test]
		public void ChangeMaximumPollsPerSecond()
		{
			var profiler = new CodeProfiler(5);
			Assert.AreEqual(5, profiler.MaximumPollsPerSecond);
			profiler.MaximumPollsPerSecond = 2;
			Assert.AreEqual(2, profiler.MaximumPollsPerSecond);
		}

		[Test]
		public void ChangeResetInterval()
		{
			var profiler = new CodeProfiler(5, 10);
			Assert.AreEqual(10, profiler.ResetInterval);
			profiler.ResetInterval = 2;
			Assert.AreEqual(2, profiler.ResetInterval);
		}

		[Test]
		public void StartingCreatesASection()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section);
			const int SectionNumber = 2;
			CodeProfilerSection section = profiler.Sections[SectionNumber][0];
			Assert.AreEqual(Section, section.Name);
			Assert.AreEqual(0, section.Calls);
			Assert.AreEqual(0.0f, section.TotalTime);
			Assert.AreEqual(1, profiler.SectionMaps[SectionNumber].Count);
			Assert.AreEqual(1, profiler.Sections[SectionNumber].Count);
		}

		private const string Section = "Section 1";

		[Test]
		public void StoppingUpdatesSectionTotalTime()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section);
			Thread.Sleep(1);
			profiler.Stop(ProfilingMode.Rendering, Section);
			CodeProfilerSection section = profiler.Sections[2][0];
			Assert.AreEqual(1, section.Calls);
			Assert.IsTrue(section.TotalTime > 0.0f);
		}

		[Test]
		public void StartingTwiceWithoutStoppingThrowsException()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section);
			Assert.Throws<CodeProfilerSection.AlreadyStarted>(
				() => profiler.Start(ProfilingMode.Rendering, Section));
		}

		[Test]
		public void StoppingWithoutHavingStartedThrowsException()
		{
			var profiler = new CodeProfiler();
			Assert.Throws<CodeProfiler.SectionNeverStarted>(
				() => profiler.Stop(ProfilingMode.Rendering, Section));
		}

		[Test]
		public void OnlyProfilesOnceIfTooShortATimeHasPassed()
		{
			var profiler = new CodeProfiler(1);
			int count = 0;
			profiler.Updated += () => count++;
			RunOneFrameOfProfiling(profiler);
			RunOneFrameOfProfiling(profiler);
			Assert.AreEqual(1, count);
		}

		private static void RunOneFrameOfProfiling(CodeProfiler profiler)
		{
			profiler.BeginFrame();
			profiler.Start(ProfilingMode.Rendering, Section);
			profiler.Stop(ProfilingMode.Rendering, Section);
			profiler.EndFrame();
		}

		[Test]
		public void ProfilesTwiceIfEnoughTimeHasPassed()
		{
			var profiler = new CodeProfiler(1000);
			int count = 0;
			profiler.Updated += () => count++;
			RunOneFrameOfProfiling(profiler);
			Thread.Sleep(2);
			RunOneFrameOfProfiling(profiler);
			Assert.AreEqual(2, count);
		}

		[Test]
		public void DoesNotResetIfTooShortATimeHasPassed()
		{
			var profiler = new CodeProfiler(1, 1);
			Thread.Sleep(2);
			profiler.BeginFrame();
			Assert.AreEqual(0.0f, profiler.lastResetTime);
		}

		[Test]
		public void ResetsIfEnoughTimeHasPassed()
		{
			var profiler = new CodeProfiler(1, 0.0001f);
			Thread.Sleep(2);
			RunOneFrameOfProfiling(profiler);
			profiler.BeginFrame();
			Assert.IsTrue(profiler.lastResetTime > 0.0f);
		}

		[Test]
		public void EmptyProfilingSummary()
		{
			var profilingResults = new CodeProfilingResults(new List<CodeProfilerSection>());
			var profilingResultsFormatter = new CodeProfilingResultsFormatter(profilingResults);
			var profiler = new CodeProfiler();
			Assert.AreEqual(profilingResultsFormatter.Summary,
				profiler.GetProfilingResultsSummary(ProfilingMode.Rendering));
		}

		[Test]
		public void ProfilingWhenInactiveDoesNothing()
		{
			var profiler = new CodeProfiler { IsActive = false };
			profiler.BeginFrame();
			profiler.Start(ProfilingMode.Rendering, Section);
			profiler.Stop(ProfilingMode.Rendering, Section);
			profiler.EndFrame();
			Assert.AreEqual(0, profiler.Sections[(int)ProfilingMode.Rendering].Count);
		}
	}
}