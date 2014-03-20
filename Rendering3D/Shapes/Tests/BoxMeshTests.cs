using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class BoxMeshTests : TestWithMocksOrVisually
	{
		[Test]
		public void DrawRedBox()
		{
			Camera.Use<LookAtCamera>();
			new Model(new ModelData(new BoxMesh(Vector3D.One, Color.Red)), Vector3D.Zero);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateBox()
		{
			var box = new BoxMesh(Vector3D.UnitY, Color.Red);
			Assert.AreEqual(Vector3D.UnitY, box.Size);
			Assert.AreEqual(Color.Red, box.Color);
			Assert.AreEqual(8, box.Geometry.NumberOfVertices);
			Assert.AreEqual(36, box.Geometry.NumberOfIndices);
			Assert.AreEqual(VertexFormat.Position3DColor,
				(box.Material.Shader as ShaderWithFormat).Format);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeSize()
		{
			var box = new BoxMesh(Vector3D.UnitX, Color.Red) { Size = Vector3D.UnitZ };
			Assert.AreEqual(Vector3D.UnitZ, box.Size);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeColor()
		{
			var box = new BoxMesh(Vector3D.UnitY, Color.Red) { Color = Color.Blue };
			Assert.AreEqual(Color.Blue, box.Color);
		}
	}
}