using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class LevelObjectHandlerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			objectHandler = new LevelObjectHandler(16);
		}

		private LevelObjectHandler objectHandler;

		[Test, CloseAfterFirstFrame]
		public void ObjectListShouldHaveSizeOfLevel()
		{
			AssertObjectListHasLevelSize(12, 12, 144);
			AssertObjectListHasLevelSize(5, 5, 25);
			AssertObjectListHasLevelSize(3, 4, 12);
		}

		private static void AssertObjectListHasLevelSize(int width, int height, int expected)
		{
			var level = new Level(new Size(width, height));
			var handler = new LevelObjectHandler(level);
			Assert.AreEqual(expected, handler.ObjectList.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void DoesNotPlaceLevelObjectWhenNoneIsSelected()
		{
			objectHandler.PlaceLevelObject(Vector2D.One, "None", 0);
		}

		[Test, CloseAfterFirstFrame]
		public void MoveObjectMovesTheObjectToArrayPosition()
		{
			objectHandler.ObjectList[0] = new DrawableEntity();
			objectHandler.MoveObject(Vector2D.One, 0, 1);
			Assert.IsNotNull(objectHandler.ObjectList[1]);
		}

		[Test, CloseAfterFirstFrame]
		public void GetLevelObjectName()
		{
			AddLevelObjectsToObjectList();
			var model = (ModelLevelObject)objectHandler.ObjectList[0];
			var particle = (ParticleLevelObject)objectHandler.ObjectList[1];
			var sprite = (SpriteLevelObject)objectHandler.ObjectList[2];
			objectHandler.MoveObject(Vector2D.One, 0, 10);
			objectHandler.MoveObject(Vector2D.One, 1, 11);
			objectHandler.MoveObject(Vector2D.One, 2, 12);
			Assert.AreEqual("Model", model.Name);
			Assert.AreEqual("Particle", particle.Name);
			Assert.AreEqual("Sprite", sprite.Name);
		}

		private void AddLevelObjectsToObjectList()
		{
			objectHandler.ObjectList[0] = new ModelLevelObject(Vector2D.One, "Model", 0);
			objectHandler.ObjectList[1] = new ParticleLevelObject(Vector2D.One, "Particle");
			objectHandler.ObjectList[2] = new SpriteLevelObject(Vector2D.One, "Sprite");
		}

		[Test, CloseAfterFirstFrame]
		public void PlaceModel()
		{
			objectHandler.PlaceLevelObject(Vector2D.One, "Model", 0);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void RecreateObjects()
		{
			objectHandler.ObjectList[0] = new ModelLevelObject(Vector2D.One, "None", 0);
			objectHandler.RecreateObjects();
		}

		[Test, Ignore]
		public void RotateLevelObject()
		{
			AddLevelObjectsToObjectList();
			objectHandler.RotateObject(0);
			objectHandler.RotateObject(2);
		}

		[Test, Ignore]
		public void FogShaderTest()
		{
			Camera.Use<LookAtCamera>();
			var material = new Material(ShaderFlags.Fog, "");
			var plane = new PlaneQuad(new Size(10, 10), material);
			new Model(new ModelData(plane), Vector3D.Zero, Quaternion.FromAxisAngle(Vector3D.UnitX, -90));
		}

		[Test, Ignore]
		public void TexturedLightMapFogShaderTest()
		{
			Camera.Use<LookAtCamera>();
			var modelData = ContentLoader.Load<ModelData>("N1C1ChildsRoom");
			foreach (var mesh in modelData.Meshes)
				mesh.Material = new Material(mesh.Material.Shader.Flags | ShaderFlags.Fog,
					mesh.Material.DiffuseMap.Name);
			new Model(modelData, Vector3D.Zero);
		}
	}
}