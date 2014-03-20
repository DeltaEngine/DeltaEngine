namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// A message sent from the BuildService which informs about the current build progress.
	/// </summary>
	public class AppBuildProgress
	{
		/// <summary>
		/// Need empty constructor for BinaryDataExtensions class reconstruction
		/// </summary>
		protected AppBuildProgress() {}

		public AppBuildProgress(string text, int progressPercentage)
		{
			Text = text;
			ProgressPercentage = progressPercentage;
		}

		public string Text { get; private set; }
		public int ProgressPercentage { get; private set; }

		public override string ToString()
		{
			return ProgressPercentage + "% - " + Text;
		}
	}
}