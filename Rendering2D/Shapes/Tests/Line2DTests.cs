using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	public class Line2DTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedLine()
		{
			new Line2D(Vector2D.UnitX, Vector2D.UnitY, Color.Red);
		}

		[Test]
		public void RenderLineAndSprite()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Red);
			new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"),
				Rectangle.FromCenter(Vector2D.Half, new Size(0.1f)));
		}

		[Test]
		public void RenderSingleRotatingLine()
		{
			AddRotatingLine(0);
		}

		private static void AddRotatingLine(int num)
		{
			var line = new Line2D(Vector2D.Half, Vector2D.Half, Color.Orange);
			line.Rotation = num * 360 / 100.0f;
			line.EndPoint = line.StartPoint +
				new Vector2D(0.4f, 0).RotateAround(Vector2D.Zero, line.Rotation);
			line.Start<Rotate>();
		}

		private class Rotate : UpdateBehavior
		{
			public Rotate()
				: base(Priority.First) {}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var line in entities.OfType<Line2D>())
				{
					line.Rotation += 15 * Time.Delta;
					var length = line.StartPoint.DistanceTo(line.EndPoint);
					line.EndPoint = line.StartPoint +
						new Vector2D(length, 0).RotateAround(Vector2D.Zero, line.Rotation);
				}
			}
		}

		[Test]
		public void RenderManyRotatingLines()
		{
			for (int num = 0; num < 100; num++)
				AddRotatingLine(num);
		}

		[Test]
		public void AddLineToEntitySystem()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Red);
			RunAfterFirstFrame(
				() => Assert.AreEqual(2, Resolve<Drawing>().NumberOfDynamicVerticesDrawnThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void CreateLine()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.One, Color.Red);
			Assert.AreEqual(Vector2D.Zero, line.StartPoint);
			Assert.AreEqual(Vector2D.One, line.EndPoint);
			Assert.AreEqual(Color.Red, line.Color);
			Assert.AreEqual(2, line.Points.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveLineViaItsEndPoints()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.Zero, Color.Red)
			{
				StartPoint = Vector2D.Half,
				EndPoint = Vector2D.One
			};
			Assert.AreEqual(Vector2D.Half, line.StartPoint);
			Assert.AreEqual(Vector2D.One, line.EndPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveLineByAssigningListOfPoints()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.Zero, Color.Red);
			line.Points = new List<Vector2D> { Vector2D.Half, Vector2D.One };
			Assert.AreEqual(Vector2D.Half, line.StartPoint);
			Assert.AreEqual(Vector2D.One, line.EndPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeColor()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.One, Color.Red) { Color = Color.Green };
			Assert.AreEqual(Color.Green, line.Color);
			Assert.AreEqual(Color.Green, line.Get<Color>());
		}

		[Test]
		public void RenderRedSquareViaAddingLines()
		{
			var line = new Line2D(new Vector2D(0.4f, 0.4f), new Vector2D(0.6f, 0.4f), Color.Red);
			line.AddLine(new Vector2D(0.6f, 0.4f), new Vector2D(0.6f, 0.6f));
			line.AddLine(new Vector2D(0.6f, 0.6f), new Vector2D(0.4f, 0.6f));
			line.AddLine(new Vector2D(0.4f, 0.6f), new Vector2D(0.4f, 0.4f));
		}

		[Test]
		public void RenderRedSquareViaExtendingLine()
		{
			CreateRedBox();
		}

		private static Line2D CreateRedBox()
		{
			var line = new Line2D(new Vector2D(0.4f, 0.4f), new Vector2D(0.6f, 0.4f), Color.Red);
			line.ExtendLine(new Vector2D(0.6f, 0.6f));
			line.ExtendLine(new Vector2D(0.4f, 0.6f));
			line.ExtendLine(new Vector2D(0.4f, 0.4f));
			return line;
		}

		[Test]
		public void RenderRedSquareWithMissingTop()
		{
			var line = CreateRedBox();
			var points = line.Points;
			points.RemoveAt(0);
			points.RemoveAt(0);
		}

		[Test]
		public void RenderRedLineOverBlue()
		{
			new Line2D(new Vector2D(0.4f, 0.4f), new Vector2D(0.6f, 0.6f), Color.Red) { RenderLayer = 1 };
			new Line2D(new Vector2D(0.6f, 0.4f), new Vector2D(0.4f, 0.6f), Color.Blue) { RenderLayer = 0 };
		}

		[Test]
		public void RenderRedLineOverBlueRectOverYellowEllipseOverGreenLine()
		{
			new FilledRect(new Rectangle(0.48f, 0.48f, 0.04f, 0.04f), Color.Blue) { RenderLayer = 1 };
			new Line2D(new Vector2D(0.2f, 0.2f), new Vector2D(0.8f, 0.8f), Color.Red) { RenderLayer = 2 };
			new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Yellow) { RenderLayer = 0 };
			new Line2D(new Vector2D(0.8f, 0.2f), new Vector2D(0.2f, 0.8f), Color.Green) { RenderLayer = -1 };
		}

		[Test]
		public void DrawingTwoLinesWithTheSameRenderLayerOnlyIssuesOneDrawCall()
		{
			new Line2D(new Vector2D(0.2f, 0.2f), new Vector2D(0.8f, 0.8f), Color.Red);
			new Line2D(new Vector2D(0.8f, 0.2f), new Vector2D(0.2f, 0.8f), Color.Green);
			RunAfterFirstFrame(
				() => Assert.AreEqual(1, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test]
		public void DrawingTwoLinesWithDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new Line2D(new Vector2D(0.2f, 0.2f), new Vector2D(0.8f, 0.8f), Color.Red).RenderLayer = 1;
			new Line2D(new Vector2D(0.8f, 0.2f), new Vector2D(0.2f, 0.8f), Color.Green).RenderLayer = -1;
			RunAfterFirstFrame(
				() => Assert.AreEqual(2, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void LineInsideViewportIsNotClipped()
		{
			var line = new Line2D(new Vector2D(0.4f, 0.4f), new Vector2D(0.6f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(0.4f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.6f, 0.5f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineAboveViewportIsHidden()
		{
			var line = new Line2D(new Vector2D(0.2f, -1.0f), new Vector2D(0.6f, -1.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(0.2f, -1.0f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.6f, -1.5f), line.EndPoint);
			Assert.IsFalse(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineBelowViewportIsHidden()
		{
			var line = new Line2D(new Vector2D(-0.2f, 2.0f), new Vector2D(2.6f, 2.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(-0.2f, 2.0f), line.StartPoint);
			Assert.AreEqual(new Vector2D(2.6f, 2.5f), line.EndPoint);
			Assert.IsFalse(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineLeftOfViewportIsHidden()
		{
			var line = new Line2D(new Vector2D(-0.2f, 0.0f), new Vector2D(-0.6f, 1.0f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(-0.2f, 0.0f), line.StartPoint);
			Assert.AreEqual(new Vector2D(-0.6f, 1.0f), line.EndPoint);
			Assert.IsFalse(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineRightOfViewportIsHidden()
		{
			var line = new Line2D(new Vector2D(1.2f, 0.5f), new Vector2D(1.6f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(1.2f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Vector2D(1.6f, 0.5f), line.EndPoint);
			Assert.IsFalse(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineNotCrossingViewportIsHidden()
		{
			var line = new Line2D(new Vector2D(-1.0f, 0.2f), new Vector2D(0.2f, -1.0f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(-1.0f, 0.2f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.2f, -1.0f), line.EndPoint);
			Assert.IsFalse(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineEnteringLeftEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(-0.5f, 0.1f), new Vector2D(0.5f, 0.2f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(0.0f, 0.15f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.5f, 0.2f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineExitingLeftEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.5f, 0.4f), new Vector2D(-0.5f, 0.4f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Vector2D(0.5f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.25f, 0.4f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineEnteringRightEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.5f, 0.4f), new Vector2D(1.5f, 0.3f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Vector2D(0.5f, 0.4f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.75f, 0.375f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineExitingRightEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(1.5f, 0.1f), new Vector2D(0.5f, 0.2f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(1.0f, 0.15f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.5f, 0.2f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineEnteringTopEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.1f, -0.5f), new Vector2D(0.2f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(0.15f, 0.0f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.2f, 0.5f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineExitingTopEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.4f, 0.5f), new Vector2D(0.3f, -0.5f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Vector2D(0.4f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.375f, 0.25f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineEnteringBottomEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.4f, 0.5f), new Vector2D(0.3f, 1.5f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.AreEqual(new Vector2D(0.4f, 0.5f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.375f, 0.75f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineExitingBottomEdgeIsClipped()
		{
			var line = new Line2D(new Vector2D(0.1f, 1.5f), new Vector2D(0.2f, 0.5f), Color.Red);
			line.Clip(Rectangle.One);
			Assert.AreEqual(new Vector2D(0.15f, 1.0f), line.StartPoint);
			Assert.AreEqual(new Vector2D(0.2f, 0.5f), line.EndPoint);
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void LineCrossingViewportIsClippedAtStartAndEnd()
		{
			var line = new Line2D(new Vector2D(-1.0f, 0.4f), new Vector2D(2.0f, 0.6f), Color.Red);
			line.Clip(Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.5f));
			Assert.IsTrue(line.StartPoint.IsNearlyEqual(new Vector2D(0.25f, 0.4833f)));
			Assert.IsTrue(line.EndPoint.IsNearlyEqual(new Vector2D(0.75f, 0.5167f)));
			Assert.IsTrue(line.IsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenLineDoesNotThrowException()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.White) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test, CloseAfterFirstFrame]
		public void EvenWithoutPointsRenderingDoesNotCrash()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.One, Color.White);
			line.Points = new List<Vector2D>();
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingDrawAreaChangesStartAndEndPoints()
		{
			var line = new Line2D(new Vector2D(1, 2), new Vector2D(3, 5), Color.White);
			line.DrawArea = new Rectangle(11, 12, 5, 6);
			Assert.AreEqual(new Vector2D(11, 12), line.StartPoint);
			Assert.AreEqual(new Vector2D(16, 18), line.EndPoint);
			Assert.AreEqual(new Rectangle(11, 12, 5, 6), line.DrawArea);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangingStartAndEndPointsChangesDrawArea()
		{
			var line = new Line2D(new Vector2D(1, 2), new Vector2D(3, 5), Color.White);
			line.StartPoint = new Vector2D(11, 12);
			line.EndPoint = new Vector2D(16, 18);
			Assert.AreEqual(new Rectangle(11, 12, 5, 6), line.DrawArea);
		}
	}
}