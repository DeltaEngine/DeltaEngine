using System;
using DeltaEngine.Core;

namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Stores a section of profiling (eg. rendering, physics etc.)
	/// </summary>
	public class CodeProfilerSection : IComparable<CodeProfilerSection>
	{
		public CodeProfilerSection(string name)
		{
			Name = name;
			TimeCreated = GlobalTime.Current.GetSecondsSinceStartToday();
		}

		public string Name { get; private set; }
		public float TimeCreated { get; private set; }

		public void Start(float pollingInterval)
		{
			if (IsStarted)
				throw new AlreadyStarted();

			float time = GlobalTime.Current.GetSecondsSinceStartToday();
			if (time - lastTimeProfiled < pollingInterval)
				return;

			lastTimeProfiled = time;
			StartTime = time;
			IsStarted = true;
		}

		public class AlreadyStarted : Exception { }

		private float lastTimeProfiled = -1.0f;
		internal float StartTime { get; private set; }
		internal bool IsStarted { get; private set; }

		public bool StopIfProfiling()
		{
			if (!IsStarted)
				return false;

			TotalTime += GlobalTime.Current.GetSecondsSinceStartToday() - StartTime;
			Calls++;
			IsStarted = false;
			return true;
		}

		public float TotalTime { get; private set; }
		public int Calls { get; private set; }

		public void Reset()
		{
			TotalTime = 0;
			Calls = 0;
		}

		public int CompareTo(CodeProfilerSection other)
		{
			return (int)(other.TotalTime * 1000) - (int)(TotalTime * 1000);
		}
	}
}