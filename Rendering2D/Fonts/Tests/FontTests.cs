using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	internal class FontTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			verdana = Font.Default;
			tahoma = ContentLoader.Load<Font>("Tahoma30");
		}

		private Font verdana;
		private Font tahoma;

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSmallFont()
		{
			new FontText(verdana, "Hi there", Rectangle.One);
		}

		[Test, CloseAfterFirstFrame]
		public void FontTextHas5Components()
		{
			var text = new FontText(verdana, "Hi there", Rectangle.One);
			Assert.AreEqual(6, text.NumberOfComponents);
		}

		/// <summary>
		/// Lerp is used for its draw size, nothing else (material and glyphs are not lerped).
		/// </summary>
		[Test, CloseAfterFirstFrame]
		public void FontTextShouldNeverHaveMoreThan2LerpComponents()
		{
			var text = new DerivedFontText(verdana, "Hi there", Rectangle.One);
			AdvanceTimeAndUpdateEntities(1);
			RunAfterFirstFrame(() => Assert.AreEqual(1, text.NumberOfLastTickLerpComponents));
		}

		private class DerivedFontText : FontText
		{
			public DerivedFontText(Font font, string text, Rectangle drawArea)
				: base(font, text, drawArea) {}

			public int NumberOfLastTickLerpComponents { get { return lastTickLerpComponents.Count; } }
		}

		[Test]
		public void CreateFontText()
		{
			var fontText = new FontText(verdana, "Verdana12",
				Rectangle.FromCenter(Vector2D.Half, new Size(0.3f, 0.1f)));
			Assert.AreEqual("Verdana12", fontText.Text);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawBigFont()
		{
			new FontText(tahoma, "Big Font!", Rectangle.One);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawColoredFonts()
		{
			new FontText(tahoma, "Red", Top) { Color = Color.Red };
			new FontText(tahoma, "Yellow", Bottom) { Color = Color.Yellow };
		}

		private static readonly Rectangle Top = new Rectangle(0.5f, 0.4f, 0.0f, 0.0f);
		private static readonly Rectangle Bottom = new Rectangle(0.5f, 0.6f, 0.0f, 0.0f);

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFontAndLines()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Red) { RenderLayer = -1 };
			new FontText(tahoma, "Delta Engine", Rectangle.One);
			new Line2D(Vector2D.UnitX, Vector2D.UnitY, Color.Red) { RenderLayer = 1 };
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFontOverSprite()
		{
			new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"), Rectangle.HalfCentered)
			{
				Color = Color.PaleGreen,
				RenderLayer = -1
			};
			new FontText(tahoma, "Delta Engine", Rectangle.One);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawTwoDifferentFonts()
		{
			new FontText(tahoma, "Delta Engine", Top);
			new FontText(verdana, "Delta Engine", Bottom);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var text = new FontText(tahoma, "Delta Engine", Top);
			var stream = BinaryDataExtensions.SaveToMemoryStream(text);
			var loadedText = (FontText)stream.CreateFromMemoryStream();
			Assert.AreEqual(Top, loadedText.DrawArea);
			Assert.AreEqual("Delta Engine", loadedText.Text);
			Assert.AreEqual(text.CachedMaterial.Name, loadedText.CachedMaterial.Name);
			Assert.AreEqual(text.description.FontFamilyName, loadedText.description.FontFamilyName);
		}

		[Test]
		public void DrawLoadedFont()
		{
			var text = new FontText(tahoma, "Original", Top);
			var stream = BinaryDataExtensions.SaveToMemoryStream(text);
			var loadedText = (FontText)stream.CreateFromMemoryStream();
			loadedText.Text = "Loaded";
			loadedText.DrawArea = Bottom;
		}
	}
}