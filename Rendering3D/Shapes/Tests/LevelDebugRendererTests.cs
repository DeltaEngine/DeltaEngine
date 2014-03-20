using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace DeltaEngine.Rendering3D.Shapes.Tests
{
	public class LevelDebugRendererTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowLevelDebugRendererIn2D()
		{
			InitializeWithLevel2D();
		}

		private void InitializeWithLevel2D()
		{
			MockLevel level = InitializeLevel(false);
			InitializeLevelDebugRenderer(level);
		}

		private MockLevel InitializeLevel(bool isLevel3D)
		{
			is3D = isLevel3D;
			var level = new MockLevel(new Size(5));
			level.RenderIn3D = is3D;
			level.SetTile(Vector2D.UnitX, LevelTileType.Red);
			level.SetTile(Vector2D.UnitY, LevelTileType.Green);
			level.SetTile(Vector2D.Zero, LevelTileType.Blue);
			return level;
		}

		private bool is3D;

		private void InitializeLevelDebugRenderer(Level level)
		{
			levelDebugRenderer = new LevelDebugRenderer(level);
		}

		private LevelDebugRenderer levelDebugRenderer;

		[Test]
		public void TestLevelDebugRendererIn3D()
		{
			SetUpCamera();
			InitializeWithLevel3D();
		}

		private static void SetUpCamera()
		{
			const float CameraPosition = 3.0f;
			var camera = Camera.Use<LookAtCamera>();
			camera.Position = new Vector3D(-CameraPosition, -CameraPosition, CameraPosition);
		}

		private void InitializeWithLevel3D()
		{
			MockLevel level = InitializeLevel(true);
			InitializeLevelDebugRenderer(level);
		}

		//ncrunch: no coverage start
		public void Dispose()
		{
			levelDebugRenderer.RemoveCommands();
			if (is3D)
				levelDebugRenderer.Dispose3D();
			else
				levelDebugRenderer.Dispose2D();
		}
	}
}