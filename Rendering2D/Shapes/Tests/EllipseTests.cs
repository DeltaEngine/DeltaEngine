using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	public class EllipseTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedEllipse()
		{
			new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Red);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedOutlinedEllipse()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Red);
			ellipse.Add(new OutlineColor(Color.Yellow));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderingWithEntityHandlersInAnyOrder()
		{
			var ellipse1 = new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Blue) { RenderLayer = 0 };
			ellipse1.Add(new OutlineColor(Color.Red));
			ellipse1.OnDraw<DrawPolygon2D>();
			ellipse1.OnDraw<DrawPolygon2DOutlines>();
			var ellipse2 = new Ellipse(Vector2D.Half, 0.1f, 0.2f, Color.Green) { RenderLayer = 1 };
			ellipse2.Add(new OutlineColor(Color.Red));
			ellipse2.OnDraw<DrawPolygon2DOutlines>();
			ellipse2.OnDraw<DrawPolygon2D>();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedTransparentEllipse()
		{
			Resolve<Window>().BackgroundColor = Color.Yellow;
			new Ellipse(Vector2D.Half, 0.4f, 0.2f, new Color(1.0f, 0.0f, 0.0f, 0.5f));
		}

		[Test, CloseAfterFirstFrame, Timeout(2000)]
		public void ChangeRadius()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.Red) { RadiusX = 0.2f, RadiusY = 0.4f };
			Assert.AreEqual(0.2f, ellipse.RadiusX);
			Assert.AreEqual(0.4f, ellipse.RadiusY);
		}

		[Test, CloseAfterFirstFrame]
		public void BorderPointsAreCalculatedOnRunningEntitySystem()
		{
			var ellipse = new Ellipse(Rectangle.One, Color.White);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(49, ellipse.Get<List<Vector2D>>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenEllipseDoesNotThrowException()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Red);
			ellipse.Add(new OutlineColor(Color.Yellow));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test]
		public void RadiusIsAlwaysTheMaximumValue()
		{
			const float BigValue = 0.4f;
			const float SmallValue = 0.2f;
			var ellipseWidth = new Ellipse(Vector2D.Half, BigValue, SmallValue, Color.Red);
			var ellipseHeight = new Ellipse(Vector2D.Half, SmallValue, BigValue, Color.Red);
			Assert.AreEqual(BigValue, ellipseWidth.Radius);
			Assert.AreEqual(BigValue, ellipseHeight.Radius);
		}

		[Test]
		public void ManuallySetTheRadius()
		{
			const float OriginalRadius = 0.2f;
			const float NewRadius = 0.4f;
			var ellipse = new Ellipse(Vector2D.Half, OriginalRadius, OriginalRadius, Color.Red);
			Assert.AreEqual(OriginalRadius, ellipse.RadiusX);
			Assert.AreEqual(OriginalRadius, ellipse.RadiusY);
			ellipse.Radius = NewRadius;
			Assert.AreEqual(NewRadius, ellipse.RadiusX);
			Assert.AreEqual(NewRadius, ellipse.RadiusY);
		}
	}
}