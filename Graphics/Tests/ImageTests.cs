using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Graphics.Vertices;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	/// <summary>
	/// The image tests here are limited to loading and integration tests, not visual tests, which
	/// you can find in DeltaEngine.Rendering2D.Tests.SpriteTests.
	/// </summary>
	public class ImageTests : TestWithMocksOrVisually
	{
		[Test, ApproveFirstFrameScreenshot]
		public void DrawOpaqueImageWithVertexColors()
		{
			Resolve<Window>().BackgroundColor = Color.CornflowerBlue;
			new ColoredSprite(ContentLoader.Load<Image>("DeltaEngineLogoOpaque"));
			RunAfterFirstFrame(
				() => Assert.AreEqual(4, Resolve<Drawing>().NumberOfDynamicVerticesDrawnThisFrame));
		}

		private class ColoredSprite : Sprite
		{
			public ColoredSprite(Image image)
				: base(image) {}

			private readonly Color[] colors = { Color.Yellow, Color.Red, Color.Blue, Color.Teal };
			public override Color[] Colors { get { return colors; } }
		}

		private abstract class Sprite : DrawableEntity
		{
			protected Sprite(Image image)
			{
				Material = new Material(ContentLoader.Create<Shader>(
					new ShaderCreationData(ShaderFlags.Position2DColoredTextured)), image);
				OnDraw<DrawSprite>();
			}

			public Material Material { get; private set; }

			private readonly Rectangle pixelDrawArea = new Rectangle(175.0f, 25.0f, 300.0f, 300.0f);
			public Rectangle PixelDrawArea { get { return pixelDrawArea; } }

			public abstract Color[] Colors { get; }
		}

		private class DrawSprite : DrawBehavior
		{
			public DrawSprite(Drawing drawing)
			{
				this.drawing = drawing;
			}

			private readonly Drawing drawing;

			public void Draw(List<DrawableEntity> visibleEntities)
			{
				foreach (var sprite in visibleEntities.OfType<Sprite>())
					drawing.Add(sprite.Material, GetQuadVertices(sprite), new short[] { 0, 2, 1, 0, 3, 2 });
			}

			private static VertexPosition2DColorUV[] GetQuadVertices(Sprite sprite)
			{
				float left = sprite.PixelDrawArea.Left;
				float right = sprite.PixelDrawArea.Right;
				float top = sprite.PixelDrawArea.Top;
				float bottom = sprite.PixelDrawArea.Bottom;
				return new[]
				{
					new VertexPosition2DColorUV(new Vector2D(left, top), sprite.Colors[0], Vector2D.Zero),
					new VertexPosition2DColorUV(new Vector2D(right, top), sprite.Colors[1], Vector2D.UnitX),
					new VertexPosition2DColorUV(new Vector2D(right, bottom), sprite.Colors[2], Vector2D.One),
					new VertexPosition2DColorUV(new Vector2D(left, bottom), sprite.Colors[3], Vector2D.UnitY)
				};
			}
		}

		[Test, CloseAfterFirstFrame]
		public void LoadExistingImage()
		{
			var image = ContentLoader.Load<Image>("DeltaEngineLogoOpaque");
			Assert.AreEqual("DeltaEngineLogoOpaque", image.Name);
			Assert.IsFalse(image.IsDisposed);
			Assert.AreEqual(new Size(128, 128), image.PixelSize);
		}

		[Test, CloseAfterFirstFrame]
		public void ShouldThrowIfImageNotLoadedWithDebuggerAttached()
		{
			//ncrunch: no coverage start
			if (Debugger.IsAttached)
				Assert.Throws<ContentLoader.ContentNotFound>(
					() => ContentLoader.Load<Image>("UnavailableImage"));
			//ncrunch: no coverage end
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawDefaultTexture()
		{
			Resolve<Window>().BackgroundColor = Color.CornflowerBlue;
			new ColoredSprite(ContentLoader.Load<Image>("UnavailableImage"));
			RunAfterFirstFrame(
				() => Assert.AreEqual(4, Resolve<Drawing>().NumberOfDynamicVerticesDrawnThisFrame));
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawCustomImageFromColorClickToChangeIt()
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(8, 8)));
			customImage.Fill(Color.Purple);
			new SolidSprite(customImage);
			new Command(Command.Click, () => customImage.Fill(Color.GetRandomColor()));
		}

		private class SolidSprite : Sprite
		{
			public SolidSprite(Image image)
				: base(image) {}

			private readonly Color[] colors = { Color.White, Color.White, Color.White, Color.White };
			public override Color[] Colors { get { return colors; } }
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawCustomImageFromBytes()
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(8, 8)));
			var bytes = new byte[8 * 8 * 4];
			var color = Color.Purple;
			for (int i = 0; i < 8 * 8; i++)
			{
				bytes[i * 4] = color.B;
				bytes[i * 4 + 1] = color.G;
				bytes[i * 4 + 2] = color.R;
			}
			customImage.FillRgbaData(bytes);
			new SolidSprite(customImage);
		}

		/// <summary>
		/// From http://forum.deltaengine.net/yaf_postsm6203_Dynamic-textures---v--0-9-8-2.aspx#post6203
		/// </summary>
		[Test, ApproveFirstFrameScreenshot]
		public void DrawCustomImageHalfRedHalfGold()
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(64, 64)));
			var colors = new Color[64 * 64];
			for (int i = 0; i < colors.Length; i++)
				colors[i] = i < colors.Length / 2 ? Color.Red : Color.Gold;
			customImage.Fill(colors);
			new ColoredSprite(customImage);
		}

		[Test, CloseAfterFirstFrame]
		public void FillCustomImageWitDifferentSizeThanImageCausesException()
		{
			var customImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(8, 9)));
			var colors = new Color[8 * 8];			
			Assert.Throws<Image.InvalidNumberOfColors>(() => customImage.Fill(colors));
			var byteArray = new byte[8 * 8];
			Assert.Throws<Image.InvalidNumberOfBytes>(() => customImage.FillRgbaData(byteArray));
			var goodByteArray = new byte[8 * 9 * 4];
			customImage.FillRgbaData(goodByteArray);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void BlendModes()
		{
			new DrawableEntity().OnDraw<RenderBlendModes>();
		}

		private class RenderBlendModes : DrawBehavior
		{
			public RenderBlendModes(Drawing drawing)
			{
				this.drawing = drawing;
				logoOpaque = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoOpaque");
				logoAlpha = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAlpha");
				additive = new Material(ShaderFlags.Position2DTextured, "CoronaAdditive");
			}

			private readonly Drawing drawing;
			private readonly Material logoOpaque;
			private readonly Material logoAlpha;
			private readonly Material additive;

			public void Draw(List<DrawableEntity> visibleEntities)
			{
				DrawAlphaImageTwice(25, 80, BlendMode.Opaque);
				DrawAlphaImageTwice(225, 80, BlendMode.Normal);
				DrawAlphaImageTwice(425, 80, BlendMode.Additive);
			}

			private void DrawAlphaImageTwice(int x, int y, BlendMode blendMode)
			{
				drawing.Add(logoOpaque, GetVertices(x, y));
				Material top = blendMode == BlendMode.Normal
					? logoAlpha : (blendMode == BlendMode.Additive ? additive : logoOpaque);
				drawing.Add(top, GetVertices(x + Size / 2, y + Size / 2));
			}

			private const int Size = 120;

			private static VertexPosition2DUV[] GetVertices(int x, int y)
			{
				return new[]
				{
					new VertexPosition2DUV(new Vector2D(x, y), Vector2D.Zero),
					new VertexPosition2DUV(new Vector2D(x + Size, y), Vector2D.UnitX),
					new VertexPosition2DUV(new Vector2D(x + Size, y + Size), Vector2D.One),
					new VertexPosition2DUV(new Vector2D(x, y + Size), Vector2D.UnitY)
				};
			}
		}
	}
}