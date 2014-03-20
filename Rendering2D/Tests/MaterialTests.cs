using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	public class MaterialTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void LoadSimpleMaterial()
		{
			Assert.IsNotNull(ContentLoader.Load<Material>("Earth"));
		}

		[Test]
		public void SkipImageDataLoadingIfNoImageNameWasSpecified()
		{
			var noImageMaterial = ContentLoader.Load<Material>("NoImageMaterial");
			Assert.IsNull(noImageMaterial.DiffuseMap);
			Assert.IsNull(noImageMaterial.Animation);
			Assert.IsNull(noImageMaterial.SpriteSheet);
		}

		[Test]
		public void LoadMaterialWithAnimation()
		{
			ContentLoader.Load<ImageAnimation>("MyImageAnimation");
			var noImageMaterial = ContentLoader.Load<Material>("MaterialWithImageAnimation");
			Assert.IsNotNull(noImageMaterial.Animation);
			Assert.IsNull(noImageMaterial.SpriteSheet);
		}

		[Test]
		public void LoadMaterialWithSpriteSheet()
		{
			ContentLoader.Load<SpriteSheetAnimation>("MySpriteSheet");
			var noImageMaterial = ContentLoader.Load<Material>("MaterialWithSpriteSheet");
			Assert.IsNotNull(noImageMaterial.SpriteSheet);
		}

		[Test]
		public void Load3DMaterial()
		{
			var material = ContentLoader.Load<Material>("Material3D");
			var shader = material.Shader as ShaderWithFormat;
			Assert.IsTrue(shader.Format.Is3D);
		}

		[Test]
		public void SaveAndLoadContentMaterial()
		{
			SaveAndLoadMaterialAndCompare(ContentLoader.Load<Material>("Earth"));
		}

		private static void SaveAndLoadMaterialAndCompare(Material material)
		{
			var data = BinaryDataExtensions.SaveDataIntoMemoryStream(material);
			var loadedMaterial = BinaryDataExtensions.LoadDataWithKnownTypeFromMemoryStream<Material>(data);
			Assert.AreEqual(material, loadedMaterial);
		}

		[Test]
		public void SaveAndLoadImageMaterial()
		{
			SaveAndLoadMaterialAndCompare(new Material(ShaderFlags.Position2DTextured, "EarthSpriteSheet"));
		}

		[Test]
		public void SaveAndLoadCustomMaterial()
		{
			var shader = ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.Position2DTextured));
			var image = ContentLoader.Load<Image>("EarthImages");
			SaveAndLoadMaterialAndCompare(new Material(shader, image));
		}

		[Test]
		public void SaveAndLoadAnimationMaterial()
		{
			var shader = ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.Position2DTextured));
			var animation = ContentLoader.Load<ImageAnimation>("MyImageAnimation");
			SaveAndLoadMaterialAndCompare(new Material(shader, null)
			{
				Animation = animation
			});
		}

		[Test]
		public void SaveAndLoadSpriteSheetMaterial()
		{
			SaveAndLoadMaterialAndCompare(new Material(ShaderFlags.Position2DTextured, "MySpriteSheet"));
		}

		[Test]
		public void SaveAndLoadCustomImageMaterial()
		{
			var shader = ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.Position2DTextured));
			SaveAndLoadMaterialAndCompare(new Material(shader, null));
		}
	}
}
