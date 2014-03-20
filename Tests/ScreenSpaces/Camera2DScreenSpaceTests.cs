using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.ScreenSpaces
{
	public class Camera2DScreenSpaceTests
	{
		[Test]
		public void LookAt()
		{
			var camera = new Camera2DScreenSpace(window);
			Assert.AreEqual(Vector2D.Half, camera.LookAt);
			camera.LookAt = Vector2D.One;
			Assert.AreEqual(Vector2D.One, camera.LookAt);
		}

		private readonly Window window = new MockWindow();

		[Test]
		public void Zoom()
		{
			var camera = new Camera2DScreenSpace(window);
			Assert.AreEqual(1.0f, camera.Zoom);
			camera.Zoom = 2.5f;
			Assert.AreEqual(2.5f, camera.Zoom);
		}

		[Test]
		public void IfCameraNotAdjustedItBehavesIdenticallyToQuadraticScreenSpace()
		{
			var q = new QuadraticScreenSpace(window);
			var c = new Camera2DScreenSpace(window);
			Assert.IsTrue(
				c.FromPixelSpace(new Vector2D(1, 2)).IsNearlyEqual(q.FromPixelSpace(new Vector2D(1, 2))));
			Assert.IsTrue(
				c.FromPixelSpace(new Size(-3, 4)).IsNearlyEqual(q.FromPixelSpace(new Size(-3, 4))));
			Assert.IsTrue(
				c.ToPixelSpace(new Vector2D(2, 6)).IsNearlyEqual(q.ToPixelSpace(new Vector2D(2, 6))));
			Assert.IsTrue(c.ToPixelSpace(new Size(-2, 0)).IsNearlyEqual(q.ToPixelSpace(new Size(-2, 0))));
		}

		[Test]
		public void IfCameraNotAdjustedEdgesAreIdenticalToQuadraticScreenSpace()
		{
			var q = new QuadraticScreenSpace(window);
			var c = new Camera2DScreenSpace(window);
			Assert.AreEqual(q.TopLeft, c.TopLeft);
			Assert.AreEqual(q.BottomRight, c.BottomRight);
			Assert.AreEqual(q.Top, c.Top, 0.0001f);
			Assert.AreEqual(q.Left, c.Left, 0.0001f);
			Assert.AreEqual(q.Bottom, c.Bottom, 0.0001f);
			Assert.AreEqual(q.Right, c.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterZoomingIn()
		{
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DScreenSpace(window) { Zoom = 2.0f };
			Assert.AreEqual(new Vector2D(0.25f, 0.359375f), camera.TopLeft);
			Assert.AreEqual(new Vector2D(0.75f, 0.640625f), camera.BottomRight);
			Assert.AreEqual(0.359375f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.640625f, camera.Bottom, 0.0001f);
			Assert.AreEqual(0.75f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanning()
		{
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DScreenSpace(window)
			{
				LookAt = new Vector2D(0.75f, 0.6f)
			};
			Assert.IsTrue(camera.TopLeft.IsNearlyEqual(new Vector2D(0.25f, 0.31875f)));
			Assert.IsTrue(camera.BottomRight.IsNearlyEqual(new Vector2D(1.25f, 0.88125f)));
			Assert.AreEqual(0.31875f, camera.Top, 0.0001f);
			Assert.AreEqual(0.25f, camera.Left, 0.0001f);
			Assert.AreEqual(0.88125f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.25f, camera.Right, 0.0001f);
		}

		[Test]
		public void EdgesAfterPanningAndZooming()
		{
			Assert.AreEqual(16 / 9.0f, window.ViewportPixelSize.AspectRatio);
			var camera = new Camera2DScreenSpace(window)
			{
				LookAt = new Vector2D(0.4f, 0.5f),
				Zoom = 0.5f
			};
			Assert.AreEqual(new Vector2D(-0.6f, -0.0625f), camera.TopLeft);
			Assert.AreEqual(new Vector2D(1.4f, 1.0625f), camera.BottomRight);
			Assert.AreEqual(-0.0625f, camera.Top, 0.0001f);
			Assert.AreEqual(-0.6f, camera.Left, 0.0001f);
			Assert.AreEqual(1.0625f, camera.Bottom, 0.0001f);
			Assert.AreEqual(1.4f, camera.Right, 0.0001f);
		}

		[Test]
		public void LoopingToAndFromPixelSpaceLeavesAPointUnchanged()
		{
			var camera = new Camera2DScreenSpace(window)
			{
				LookAt = new Vector2D(-0.5f, 0.6f),
				Zoom = 3.0f
			};
			Assert.IsTrue(
				camera.ToPixelSpace(camera.FromPixelSpace(new Vector2D(1.2f, 3.4f))).IsNearlyEqual(
					new Vector2D(1.2f, 3.4f)));
			Assert.IsTrue(
				camera.FromPixelSpace(camera.ToPixelSpace(new Vector2D(1.2f, 3.4f))).IsNearlyEqual(
					new Vector2D(1.2f, 3.4f)));
		}

		[Test]
		public void ToPixelSpace()
		{
			var quadraticSize = new Size(window.ViewportPixelSize.Width);
			var camera = new Camera2DScreenSpace(window)
			{
				LookAt = new Vector2D(-0.5f, 0.6f),
				Zoom = 2.0f
			};
			Assert.AreEqual(quadraticSize.Width * 1.5f, camera.ToPixelSpace(Vector2D.Zero).X);
			Assert.IsTrue(camera.ToPixelSpace(Vector2D.Half).IsNearlyEqual(new Vector2D(800, 26)));
			Assert.IsTrue(camera.ToPixelSpace(Vector2D.One).IsNearlyEqual(new Vector2D(1120, 346)));
			Assert.AreEqual(quadraticSize, camera.ToPixelSpace(Size.Half));
		}

		[Test]
		public void ToPixelSpaceWithRotation()
		{
			var quadraticSize = new Size(window.ViewportPixelSize.Width);
			var camera = new Camera2DScreenSpace(window) { Rotation = 90.0f };
			Assert.AreEqual(0.0f, camera.ToPixelSpace(Vector2D.Zero).X);
			Assert.IsTrue(camera.ToPixelSpace(Vector2D.Half).IsNearlyEqual(new Vector2D(-160.0f, 90.0f)));
			Assert.IsTrue(
				camera.ToPixelSpace(Vector2D.One).IsNearlyEqual(new Vector2D(-320.0f, (160.0f + 90.0f))));
			Assert.AreEqual(quadraticSize, camera.ToPixelSpace(Size.One));
		}

		[Test]
		public void FromPixelSpace()
		{
			var camera = new Camera2DScreenSpace(window)
			{
				LookAt = new Vector2D(-0.5f, 0.6f),
				Zoom = 2.0f
			};
			Assert.IsTrue(
				camera.FromPixelSpace(Vector2D.Zero).IsNearlyEqual(new Vector2D(-0.75f, 0.459375f)));
			Assert.IsTrue(
				camera.FromPixelSpace((Vector2D)window.ViewportPixelSize).IsNearlyEqual(new Vector2D(
					-0.25f, 0.740625f)));
			Assert.AreEqual(camera.LookAt, camera.FromPixelSpace((Vector2D)window.ViewportPixelSize / 2));
		}
	}
}