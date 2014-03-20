using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Profiling.Tests
{
	public class ProfileResultsFormatterTests
	{
		[Test]
		public void NeverStartingReportsNothingProfiled()
		{
			var profiler = new CodeProfiler();
			Assert.AreEqual(NothingProfiled,
				profiler.GetProfilingResultsSummary(ProfilingMode.Rendering));
		}

		private const string NothingProfiled = "(Nothing profiled)";

		[Test]
		public void StartingOneSection()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section1);
			Assert.IsTrue(
				profiler.GetProfilingResultsSummary(ProfilingMode.Rendering).StartsWith(OneSection));
		}

		private const string Section1 = "Section 1";
		private const string OneSection = "We got 1 section that took in total:";

		[Test]
		public void StartingTwoSections()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section1);
			profiler.Start(ProfilingMode.Rendering, Section2);
			Assert.IsTrue(
				profiler.GetProfilingResultsSummary(ProfilingMode.Rendering).StartsWith(TwoSections));
		}

		private const string Section2 = "Section 2";
		private const string TwoSections = "We got 2 sections that took in total:";

		[Test]
		public void StartingAndStoppingTwice()
		{
			var profiler = StartAndStopTwoSections();
			string summary = profiler.GetProfilingResultsSummary(ProfilingMode.Rendering);
			Assert.IsTrue(summary.StartsWith(TwoSections));
			Assert.IsTrue(summary.Contains(Section1));
			Assert.IsTrue(summary.Contains(Section2));
			Assert.IsTrue(summary.Contains(OneCall));
		}

		private const string OneCall = "Calls: 1";

		private static CodeProfiler StartAndStopTwoSections()
		{
			var profiler = new CodeProfiler();
			profiler.Start(ProfilingMode.Rendering, Section1);
			Thread.Sleep(1);
			profiler.Start(ProfilingMode.Rendering, Section2);
			Thread.Sleep(1);
			profiler.Stop(ProfilingMode.Rendering, Section1);
			profiler.Stop(ProfilingMode.Rendering, Section2);
			return profiler;
		}
	}
}