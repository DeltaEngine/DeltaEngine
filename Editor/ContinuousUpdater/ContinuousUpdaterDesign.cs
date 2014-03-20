using System.Collections.Generic;

namespace DeltaEngine.Editor.ContinuousUpdater
{
	public sealed class ContinuousUpdaterDesign : ContinuousUpdaterViewModel
	{
		public ContinuousUpdaterDesign()
		{
			SelectedProject = new Project(
				@"C:\code\DeltaEngine\Rendering2D\Shapes\Tests\bin\Debug\DeltaEngine.Rendering2D.Shapes.Tests.dll");
			Tests = new List<string> { "Line2DTests.RenderRedLine", "Line2DTests.RenderGreenLine" };
			SelectedTest = Tests[0];
			SourceCode = "new Line2D(Vector2D.UnitX, Vector2D.UnitY, Color.Red);\n" +
				"new Circle(Vector2D.Center, 0.2f, Color.Green);";
			LastTimeUpdated = "2013-10-02 03:30:24";
			IsUpdating = true;
		}
	}
}