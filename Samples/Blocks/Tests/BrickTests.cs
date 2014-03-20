using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Blocks.Tests
{
	/// <summary>
	/// Unit tests for Brick
	/// </summary>
	public class BrickTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			displayMode = ScreenSpace.Current.Viewport.Aspect >= 1.0f
				? Orientation.Landscape : Orientation.Portrait;
			content = new JewelBlocksContent();
			material = new Material(ShaderFlags.Position2DColoredTextured, "Block1");
		}

		private Orientation displayMode;
		private JewelBlocksContent content;
		private Material material;

		[Test, CloseAfterFirstFrame]
		public void Constructor()
		{
			var brick = new Brick(material, Vector2D.Half, displayMode);
			Assert.AreEqual(Vector2D.Half, brick.Offset);
		}

		[Test, CloseAfterFirstFrame]
		public void Constants()
		{
			Assert.AreEqual(new Vector2D(0.38f, 0.385f), Brick.OffsetLandscape);
			Assert.AreEqual(0.02f, Brick.ZoomLandscape);
		}

		[Test, CloseAfterFirstFrame]
		public void Offset()
		{
			var brick = new Brick(material, Vector2D.Zero, displayMode) { Offset = Vector2D.Half };
			Assert.AreEqual(Vector2D.Half, brick.Offset);
		}

		[Test, CloseAfterFirstFrame]
		public void TopLeft()
		{
			var brick = new Brick(material, Vector2D.Zero, displayMode) { TopLeftGridCoord = Vector2D.Half };
			Assert.AreEqual(Vector2D.Half, brick.TopLeftGridCoord);
		}

		[Test, CloseAfterFirstFrame]
		public void Position()
		{
			var brick = new Brick(material, new Vector2D(0.1f, 0.2f), displayMode)
			{
				TopLeftGridCoord = new Vector2D(0.4f, 0.8f)
			};
			Assert.AreEqual(new Vector2D(0.5f, 1.0f), brick.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderBrick()
		{
			var brick = new Brick(material, new Vector2D(5, 5), displayMode);
			brick.UpdateDrawArea();
		}

		[Test]
		public void RenderBrickInLandscapeMode()
		{
			Resolve<Window>().ViewportPixelSize = new Size(600, 800);
			var brick = new Brick(material, new Vector2D(5, 5), Orientation.Landscape);
			brick.UpdateDrawArea();
		}

		[Test]
		public void RenderBrickInPortraitMode()
		{
			Resolve<Window>().ViewportPixelSize = new Size(600, 800);
			var brick = new Brick(material, new Vector2D(5, 5), Orientation.Portrait);
			brick.UpdateDrawArea();
		}
	}
}