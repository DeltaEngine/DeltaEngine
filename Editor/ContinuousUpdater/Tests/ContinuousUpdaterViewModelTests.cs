using NUnit.Framework;

namespace DeltaEngine.Editor.ContinuousUpdater.Tests
{
	public class ContinuousUpdaterViewModelTests
	{
		[SetUp]
		public void CreateViewModel()
		{
			updater = new ContinuousUpdaterViewModel();
		}

		private ContinuousUpdaterViewModel updater;

		[Test]
		public void GetLatestVisualStudioObject()
		{
			var dte = updater.GetLatestVisualStudioObject();
			Assert.IsNotNull(dte);
			Assert.AreEqual("Microsoft Visual Studio", dte.Name);
		}

		[Test]
		public void GetVisualStudioPath()
		{
			var idePath = updater.GetLatestVisualStudioBinPath();
			Assert.IsTrue(idePath.EndsWith(@"Common7\IDE\"));
			Assert.AreEqual(
				idePath.Contains("11")
					? @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\"
					: @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\", idePath);
		}

		[Test]
		public void CheckCommandLineArgumentsForProjectAndSourceCodeFile()
		{
			//Assert.AreEqual("DeltaEngine.Rendering2D.Shapes.Tests Line2DTests.RenderRedLine",8 updater.BuildCommandLineArguments()));
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void OpenVisualStudioAndShowLines2DTestSourceCode()
		{
			updater.OpenCurrentTestInVisualStudio();
		}
	}
}