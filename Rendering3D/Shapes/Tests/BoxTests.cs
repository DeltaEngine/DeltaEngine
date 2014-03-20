using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class BoxTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateYellowBox()
		{
			var box = new Box(Vector3D.One, Color.Yellow);
			var mesh = (BoxMesh)box.Get<ModelData>().Meshes[0];
			Assert.AreEqual(Vector3D.One, mesh.Size);
		}

		[Test]
		public void DrawSeveralCubes()
		{
			CreateCubeOfColorAt(1.8f * Vector3D.One, Color.Gold);
			CreateCubeOfColorAt(Vector3D.One, Color.Orange);
			CreateCubeOfColorAt(0.2f * Vector3D.One, Color.Green);
			CreateCubeOfColorAt(-0.6f * Vector3D.One, Color.Red);
		}

		private static Box CreateCubeOfColorAt(Vector3D position, Color color)
		{
			var cube = new Box(Vector3D.One, color) { Position = position };
			return cube;
		}

		[Test, CloseAfterFirstFrame]
		public void CreateYellowBoxInOtherPosition()
		{
			var box = new Box(Vector3D.One, Color.Yellow, new Vector3D(0.5f, 0.0f, 0.0f));
			var mesh = (BoxMesh)box.Get<ModelData>().Meshes[0];
			Assert.AreEqual(new Vector3D(0.5f, 0.0f, 0.0f), box.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateBoxWithTheModelData()
		{
			var data = new ModelData(new BoxMesh(Vector3D.One, Color.Red));
			var box = new Box(data, Vector3D.Zero, Quaternion.Identity);
			var mesh = (BoxMesh)box.Get<ModelData>().Meshes[0];
			Assert.AreEqual(Vector3D.Zero, box.Position);
			Assert.AreEqual(Quaternion.Identity, box.Orientation);
			Assert.AreEqual(Color.Red, mesh.Color);
			Assert.AreEqual(Vector3D.One, mesh.Size);
		}
	}
}
