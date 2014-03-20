using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class ContentExtensionsTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowDefaultMaterial2DWithSolidColor()
		{
			Material defaultMaterial = ContentExtensions.CreateDefaultMaterial2D(Color.Blue);
			new Sprite(defaultMaterial, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		[Test]
		public void ShowDefaultMaterial2DWithCheckerColors()
		{
			Material defaultMaterial = ContentExtensions.CreateDefaultMaterial2D();
			new Sprite(defaultMaterial, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckerColorsAreAlternatelyBrightAndDarkAsPattern()
		{
			Color[] checkerColors = ContentExtensions.GetCheckerColors();
			Assert.AreEqual(8 * 8, checkerColors.Length);
			Assert.AreEqual(Color.LightGray, checkerColors[0]);
			Assert.AreEqual(Color.DarkGray, checkerColors[1]);
			Assert.AreEqual(Color.DarkGray, checkerColors[checkerColors.Length - 2]);
			Assert.AreEqual(Color.LightGray, checkerColors[checkerColors.Length - 1]);
		}
	}
}