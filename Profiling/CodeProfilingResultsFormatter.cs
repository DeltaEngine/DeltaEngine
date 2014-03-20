namespace DeltaEngine.Profiling
{
	/// <summary>
	/// Takes some profile results and turns them into an informative summary.
	/// </summary>
	public class CodeProfilingResultsFormatter
	{
		public CodeProfilingResultsFormatter(CodeProfilingResults results)
		{
			if (results.Sections.Count == 0)
				Summary = "(Nothing profiled)";
			else
				FormSummary(results);
		}

		public string Summary { get; private set; }

		private void FormSummary(CodeProfilingResults results)
		{
			results.Sections.Sort();
			string sections = results.Sections.Count == 1 ? " section" : " sections";
			Summary = "We got " + results.Sections.Count + sections + " that took in total: " +
				results.TotalSectionTime + "s (which seems to be about " +
				(100.0f * results.TotalSectionTime / results.TotalTime) +
				"% of the total application time: " + results.TotalTime + "s)";

			if (results.TotalSectionTime > 0)
				foreach (CodeProfilerSection section in results.Sections)
					Summary += "\n Section " + section.Name + " took: " + section.TotalTime + "s (" +
						(100.0f * section.TotalTime / results.TotalSectionTime) + "%), Calls: " + section.Calls +
						", first time called: " + section.TimeCreated + "s";
		}
	}
}