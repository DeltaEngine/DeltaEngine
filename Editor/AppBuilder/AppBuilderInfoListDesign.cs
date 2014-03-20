namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Helper class for an easier view modeling at design time.
	/// </summary>
	internal class AppBuilderInfoListDesign
	{
		public AppBuilderInfoListDesign()
		{
			TextOfBuiltApps = "0 Apps";
			TextOfErrorCount = "0 Errors";
			TextOfWarningCount = "0 Warnings";
		}

		public string TextOfBuiltApps { get; set; }
		public string TextOfErrorCount { get; set; }
		public string TextOfWarningCount { get; set; }
	}
}