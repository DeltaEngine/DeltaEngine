using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class CameraOrientationGizmoTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateCameraOrientationGizmo()
		{
			cameraOrientationGizmo = new CameraOrientationGizmo();
		}

		private CameraOrientationGizmo cameraOrientationGizmo;

		[Test]
		public void RenderGizmo()
		{
			var camera = Resolve<LookAtCamera>();
			const float CameraDistance = 1.5f;
			camera.Position = new Vector3D(-CameraDistance, -CameraDistance, CameraDistance);
			camera.Target = Vector3D.Zero;
		}

		[Test, CloseAfterFirstFrame]
		public void HideAndShowGizmo()
		{
			Assert.IsTrue(cameraOrientationGizmo.IsVisible);
			cameraOrientationGizmo.IsVisible = false;
			Assert.IsFalse(cameraOrientationGizmo.IsVisible);
			cameraOrientationGizmo.IsVisible = true;
			Assert.IsTrue(cameraOrientationGizmo.IsVisible);
		}
	}
}