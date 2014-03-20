using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class LevelEditorCameraTests : TestWithMocksOrVisually
	{
		[Test]
		public void RotateCameraByDragging()
		{
			var camera = Camera.Use<LevelEditorCamera>();
			camera.Position = new Vector3D(0, 3, 13);
			camera.Rotate(Vector3D.UnitX, 25);
			var material = new Material(ShaderFlags.Colored, "");
			var plane = new PlaneQuad(new Size(10, 10), material);
			new Model(new ModelData(plane), new Vector3D(0, 0, -1), Quaternion.FromAxisAngle(Vector3D.UnitX, -90));
		}
	}
}