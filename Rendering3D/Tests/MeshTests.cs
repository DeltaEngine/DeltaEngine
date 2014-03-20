using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Tests
{
	public class MeshTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CreateMeshDynamically()
		{
			var mesh = new Mesh(ContentLoader.Load<Geometry>("AnyGeometry"),
				ContentLoader.Load<Material>("AnyMaterial"));
			Assert.IsNotNull(mesh.Geometry);
			Assert.IsNotNull(mesh.Material);
			Assert.AreEqual(Matrix.Identity, mesh.LocalTransform);
		}

		[Test, CloseAfterFirstFrame]
		public void LoadMeshFromContent()
		{
			var mesh = ContentLoader.Load<Mesh>("AnyMeshCustomTransform");
			Assert.IsNotNull(mesh.Geometry);
			Assert.IsNotNull(mesh.Material);
			Assert.AreNotEqual(new Matrix(), mesh.LocalTransform);
			Assert.AreNotEqual(Matrix.Identity, mesh.LocalTransform);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawRedLineYellowBoxAndSprite()
		{
			Camera.Use<LookAtCamera>();
			new Sprite(new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo"),
				new Rectangle(0.8f, 0.3f, 0.1f, 0.1f));
			new Line3D(new Vector3D(1, -1, 0), new Vector3D(-1, 1, 0), Color.Red);
			new Box(Vector3D.One, Color.Yellow);
		}
	}
}