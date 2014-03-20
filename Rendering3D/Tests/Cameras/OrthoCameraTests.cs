using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class OrthoCameraTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitializeEntityRunner()
		{
			new MockEntitiesRunner(typeof(MockUpdateBehavior));
			camera = Camera.Use<OrthoCamera>();
		}

		private OrthoCamera camera;

		[Test, CloseAfterFirstFrame]
		public void UpdateCameraPosition()
		{
			Assert.IsTrue(Camera.IsInitialized);
			Assert.AreEqual(Vector3D.Zero, camera.Position);
			camera.Position += Vector3D.One * 2;
			Assert.AreEqual(new Vector3D(2, 2, 2), camera.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateCameraZoom()
		{
			camera.ZoomLevel = 2.0f;
			camera.MaxZoom = 10.0f;
			camera.MinZoom = 1.0f;
			Assert.AreEqual(2.0f, camera.ZoomLevel);
			camera.ZoomSmoothFactor = 1.0f;
			camera.Zoom(1.5f);
			Assert.AreEqual(3.5f, camera.ZoomLevel);
			camera.Zoom(10f);
			Assert.AreEqual(3.5f, camera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateCameraTarget()
		{
			Assert.AreEqual(Vector3D.Zero, camera.Target);
			camera.Target += Vector3D.One * 2;
			Assert.AreEqual(new Vector3D(2, 2, 2), camera.Target);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateViewportSize()
		{
			var window = Resolve<Window>();
			var previousSize = window.ViewportPixelSize;
			window.ViewportPixelSize = previousSize / 2;
			Assert.AreEqual(1.0f, camera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateProjectionMatrix()
		{
			var device = Resolve<Device>();
			device.Set3DMode();
			Assert.AreEqual(1.0f, camera.ZoomLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void TestViewPanning()
		{
			camera.Position = Vector3D.UnitX;
			camera.Target = Vector3D.Zero;
			camera.ViewPanning(Vector2D.UnitX);
			Assert.IsTrue(new Vector3D(-1, 0, 0).IsNearlyEqual(camera.Target));
			Assert.IsTrue(new Vector3D(0, 0, 0).IsNearlyEqual(camera.Position));
		}
	}
}