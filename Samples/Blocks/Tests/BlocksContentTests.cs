using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace Blocks.Tests
{
	public class BlocksContentTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void DoBricksSplitInHalfOnExit()
		{
			var content = new JewelBlocksContent();
			Assert.IsFalse(content.DoBricksSplitInHalfWhenRowFull);
			content.DoBricksSplitInHalfWhenRowFull = true;
			Assert.IsTrue(content.DoBricksSplitInHalfWhenRowFull);
		}

		[Test, CloseAfterFirstFrame]
		public void AreFiveBrickBlocksAllowed()
		{
			var content = new JewelBlocksContent();
			Assert.IsTrue(content.AreFiveBrickBlocksAllowed);
			content.AreFiveBrickBlocksAllowed = false;
			Assert.IsFalse(content.AreFiveBrickBlocksAllowed);
		}

		[Test, CloseAfterFirstFrame]
		public void DoBlocksStartInARandomColumn()
		{
			var content = new JewelBlocksContent();
			Assert.IsFalse(content.DoBlocksStartInARandomColumn);
			content.DoBlocksStartInARandomColumn = true;
			Assert.IsTrue(content.DoBlocksStartInARandomColumn);
		}

		[Test, CloseAfterFirstFrame]
		public void GetFilenameWithoutPrefix()
		{
			var content = new JewelBlocksContent { Prefix = "ABC" };
			Assert.AreEqual("DEF", content.GetFilenameWithoutPrefix("ABCDEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("ADEF"));
			Assert.Throws<BlocksContent.FilenameWrongPrefixException>(
				() => content.GetFilenameWithoutPrefix("AAADEF"));
		}

		[Test, CloseAfterFirstFrame]
		public void LoadContentWithNoPrefixSet()
		{
			var material = new Material(ShaderFlags.Position2DColoredTextured, "FruitBlocks_Block1");
			Assert.IsNotNull(material.DiffuseMap);
		}

		[Test, CloseAfterFirstFrame]
		public void LoadContentWithPrefixSet()
		{
			var content = new JewelBlocksContent { Prefix = "Mod1_" };
			var material = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo");
			new Sprite(material, new Rectangle(0.3f, 0.45f, 0.1f, 0.1f));
			content.Prefix = "Mod2_";
			material = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo");
			new Sprite(material, new Rectangle(0.6f, 0.45f, 0.1f, 0.1f));
		}

		[Test, CloseAfterFirstFrame]
		public void ContentWithPrefixSet()
		{
			var content = new JewelBlocksContent { Prefix = "FruitBlocks_" };
			var image = content.Load<Image>("Block1");
			Assert.IsTrue(image.PixelSize == new Size(4) || image.PixelSize == new Size(64) ||
				image.PixelSize == new Size(128));
			content.Prefix = "JewelBlocks_";
			image = content.Load<Image>("Block1");
			Assert.IsTrue(image.PixelSize == new Size(4) || image.PixelSize == new Size(64) ||
				image.PixelSize == new Size(128));
		}
	}
}