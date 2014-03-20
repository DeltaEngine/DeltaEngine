using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Snake.Tests
{
	public class ChunkTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			gridSize = 10;
			blockSize = 1.0f / gridSize;
		}

		private int gridSize;
		private float blockSize;

		[Test]
		public void CreateFirstChunk()
		{
			var chunk = new Chunk(gridSize, blockSize, Color.Purple);
			Assert.IsTrue(chunk.IsActive);
			Assert.AreEqual(blockSize, chunk.Size.Width);
			Assert.AreEqual(blockSize, chunk.Size.Height);
			Assert.AreEqual(Color.Purple, chunk.Color);
			Assert.LessOrEqual(0.0f, chunk.DrawArea.Left);
			Assert.LessOrEqual(0.0f, chunk.DrawArea.Top);
			Assert.GreaterOrEqual(1.0f, chunk.DrawArea.Left);
			Assert.GreaterOrEqual(1.0f, chunk.DrawArea.Top);
		}

		[Test]
		public void DrawChunkAtRandomLocation()
		{
			Resolve<Window>().ViewportPixelSize = new Size(800, 600);
			var smallChunk = new Chunk(gridSize, blockSize, Color.Purple);
			smallChunk.SpawnAtRandomLocation();
		}

		[Test]
		public void CheckChunkSpawnWithinSnakeBody()
		{
			Resolve<Window>().ViewportPixelSize = new Size(200, 200);
			new Snake(gridSize, Color.Green);
			new Chunk(gridSize, blockSize, Color.Purple);
		}
	}
}