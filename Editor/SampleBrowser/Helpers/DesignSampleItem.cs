namespace DeltaEngine.Editor.SampleBrowser.Helpers
{
	public class DesignSampleItem : Sample
	{
		public DesignSampleItem()
			: base(TestName, SampleCategory.Test, Solution, Project, Exe)
		{
			EntryClass = "GraphicsTests";
			EntryMethod = "ShowRedBackground";
		}

		private const string TestName =
			"TestMethodWithTheMaximumAmountOfLettersInItsNameToFitIntoTheDeltaEngineLineWidth";
		private const string Solution = @"C:\Code\DeltaEngine\Samples\Asteroids\Asteroids.sln";
		private const string Project = @"C:\Code\DeltaEngine\Samples\Asteroids\Asteroids.csproj";
		private const string Exe = @"C:\Code\DeltaEngine\Samples\Asteroids\bin\Debug\Asteroids.exe";
	}
}