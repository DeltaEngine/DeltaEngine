using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Allows classes and methods to be profiled as to how long they took to run.
	/// </summary>
	public class CodeProfiler : CodeProfilingProvider
	{
		static CodeProfiler()
		{
			Current = new CodeProfiler { IsActive = false };
		}

		public static CodeProfilingProvider Current { get; set; }
		public bool IsActive { get; set; }

		public CodeProfiler(int maximumPollsPerSecond = 10, float resetInterval = 0)
		{
			MaximumPollsPerSecond = maximumPollsPerSecond;
			ResetInterval = resetInterval;
			CreateSectionMaps();
			CreateSections();
			IsActive = true;
		}

		public int MaximumPollsPerSecond
		{
			get { return pollingInterval == 0.0f ? 0 : (int)(0.5f + 1.0f / pollingInterval); }
			set { pollingInterval = value == 0 ? 0.0f : 1.0f / value; }
		}

		private float pollingInterval;

		public float ResetInterval { get; set; }

		private void CreateSectionMaps()
		{
			SectionMaps = new Dictionary<string, int>[CategoryCount];
			for (int i = 0; i < CategoryCount; i++)
				SectionMaps[i] = new Dictionary<string, int>();
		}

		private static readonly int CategoryCount = Enum.GetNames(typeof(ProfilingMode)).Length;
		internal Dictionary<string, int>[] SectionMaps;

		private void CreateSections()
		{
			Sections = new List<CodeProfilerSection>[CategoryCount];
			for (int i = 0; i < CategoryCount; i++)
				Sections[i] = new List<CodeProfilerSection>();
		}

		internal List<CodeProfilerSection>[] Sections;

		public void Start(ProfilingMode profilingMode, string sectionName)
		{
			if (!IsActive)
				return;
			var index = ProfilingModeToIndex(profilingMode);
			int sectionIndex;
			if (SectionMaps[index].TryGetValue(sectionName, out sectionIndex))
				Sections[index][sectionIndex].Start(pollingInterval);
			else
				AddAndStartNewSection(index, sectionName);
		}

		private static int ProfilingModeToIndex(ProfilingMode profilingMode)
		{
			var profilingModeFlags = (int)profilingMode;
			int profilingModeInt = -1;
			while (profilingModeFlags > 0)
			{
				profilingModeFlags = profilingModeFlags >> 1;
				profilingModeInt++;
			}
			return profilingModeInt;
		}

		private void AddAndStartNewSection(int category, string sectionName)
		{
			SectionMaps[category].Add(sectionName, Sections[category].Count);
			var section = new CodeProfilerSection(sectionName);
			Sections[category].Add(section);
			section.Start(pollingInterval);
		}

		public void Stop(ProfilingMode profilingMode, string sectionName)
		{
			if (!IsActive)
				return;
			var index = ProfilingModeToIndex(profilingMode);
			int sectionIndex;
			if (!SectionMaps[index].TryGetValue(sectionName, out sectionIndex))
				throw new SectionNeverStarted(profilingMode, sectionName);
			didProfilingOccur |= Sections[index][sectionIndex].StopIfProfiling();
		}

		public class SectionNeverStarted : Exception
		{
			public SectionNeverStarted(ProfilingMode profilingMode, string name)
				: base("Profiling Mode '" + profilingMode + "', Section '" + name + "'") {}
		}

		public void BeginFrame()
		{
			if (!IsActive)
				return;
			float time = GlobalTime.Current.GetSecondsSinceStartToday();
			if (ResetInterval == 0 || time - lastResetTime < ResetInterval)
				return;
			foreach (CodeProfilerSection section in Sections.SelectMany(sections => sections))
				section.Reset();
			lastResetTime = time;
		}

		private bool didProfilingOccur;

		internal float lastResetTime;

		public void EndFrame()
		{
			if (!didProfilingOccur || Updated == null)
				return;
			Updated();
			didProfilingOccur = false;
		}

		public event Action Updated;

		public string GetProfilingResultsSummary(ProfilingMode profilingMode)
		{
			return new CodeProfilingResultsFormatter(GetProfilingResults(profilingMode)).Summary;
		}

		public CodeProfilingResults GetProfilingResults(ProfilingMode profilingMode)
		{
			var index = ProfilingModeToIndex(profilingMode);
			return new CodeProfilingResults(new List<CodeProfilerSection>(Sections[index]));
		}
	}
}