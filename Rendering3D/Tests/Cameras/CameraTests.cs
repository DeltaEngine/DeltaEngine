using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests.Cameras
{
	public class CameraTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateCamera()
		{
			usedDevice = Resolve<Device>();
			usedWindow = Resolve<Window>();
			Camera.Use<LookAtCamera>();
		}

		private Device usedDevice;
		private Window usedWindow;

		[Test, CloseAfterFirstFrame]
		public void AfterSetupACameraTheAmbientContextShouldBeInitialized()
		{
			Assert.IsTrue(Camera.IsInitialized);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void CameraShouldBeAbleToHandleViewportChanges()
		{
			new Grid3D(new Size(5));
			usedDevice = Resolve<Device>();
			usedWindow = Resolve<Window>();
			Matrix originalProjectionMatrix = usedDevice.CameraProjectionMatrix;
			usedWindow.ViewportPixelSize = new Size(400, 300);
			usedDevice.Set3DMode();
			Assert.AreNotEqual(originalProjectionMatrix, usedDevice.CameraProjectionMatrix);
		}
	}
}