using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class PictureTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			material = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo");
			var theme = new Theme { SliderPointer = material };
			picture = new Picture(theme, material, Rectangle.HalfCentered);
		}

		private Material material;
		private Picture picture;
		
		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(picture);
			var loadedPicture = (Picture)stream.CreateFromMemoryStream();
			Assert.AreEqual("DeltaEngineLogo", loadedPicture.Material.DiffuseMap.Name);
			Assert.AreEqual("DeltaEngineLogo", loadedPicture.Get<Theme>().SliderPointer.DiffuseMap.Name);
			Assert.AreEqual(Rectangle.HalfCentered, loadedPicture.DrawArea);
		}

		[Test]
		public void DrawLoadedPicture()
		{
			picture.IsActive = false;
			picture = new Picture(new Theme(), material, Rectangle.HalfCentered);
			var stream = BinaryDataExtensions.SaveToMemoryStream(picture);
			picture.IsActive = false;
			stream.CreateFromMemoryStream();
		}

		[Test]
		public void SetAppearance()
		{
			material = new Material(Color.Red);
			picture.SetAppearance(material);
			Assert.AreEqual(material, picture.Material);
			Assert.AreEqual(material.DefaultColor, picture.Color);
		}

		[Test]
		public void SetAppearanceWithNullShouldReturn()
		{
			Assert.DoesNotThrow(() => picture.SetAppearance(null));
		}

		[Test]
		public void SetAppearanceWithoutInterpolationWithNullShouldReturn()
		{
			Assert.DoesNotThrow(() => picture.SetAppearanceWithoutInterpolation(null));
		}

		[Test]
		public void SetAppearanceWithoutInterpolation()
		{
			var newMaterial = new Material(Color.Red);
			picture.SetAppearanceWithoutInterpolation(newMaterial);
			Assert.AreEqual(newMaterial, picture.Material);
			Assert.AreEqual(newMaterial.DefaultColor, picture.Color);
		}
	}
}