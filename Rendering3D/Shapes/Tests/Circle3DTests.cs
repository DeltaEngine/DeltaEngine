using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class Circle3DTests : TestWithMocksOrVisually
	{
		[Test]
		public void RenderEllipseInOrigin()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			var ellipse = new Circle3D(Vector3D.Zero, 3.0f, Color.Red);
			Assert.AreEqual(Vector3D.Zero, ellipse.Center);
			Assert.AreEqual(3.0f, ellipse.Radius);
		}

		private static void CreateLookAtCamera(Vector3D position, Vector3D target)
		{
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = position;
			camera.Target = target;
		}

		[Test]
		public void UpdateCenterAndRadius()
		{
			CreateLookAtCamera(Vector3D.One * 4.0f, Vector3D.Zero);
			var ellipse = new Circle3D(Vector3D.Zero, 3.0f, Color.Red);
			Assert.AreEqual(Vector3D.Zero, ellipse.Center);
			Assert.AreEqual(3.0f, ellipse.Radius);
			ellipse.Center = Vector3D.UnitX;
			ellipse.Radius = 2.0f;
			Assert.AreEqual(Vector3D.UnitX, ellipse.Center);
			Assert.AreEqual(2.0f, ellipse.Radius);
		}
	}
}
