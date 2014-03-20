using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	class GradientFilledRectTests :TestWithMocksOrVisually
	{
		[Test]
		public void RenderRect()
		{
			new GradientFilledRect(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f), Color.Blue, Color.Purple);
		}

		[Test]
		public void TurnAndMoveGradientRect()
		{
			var gradientRect = new GradientFilledRect(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f), Color.Blue,
				Color.Purple);
			gradientRect.Rotation = 10;
			gradientRect.Center += new Vector2D(0.1f, 0.1f);
			AdvanceTimeAndUpdateEntities();
		}
	}
}
