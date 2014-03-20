using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	/// <summary>
	/// Tests for the sprite sheet based animation
	/// </summary>
	public class SpriteSheetTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateMaterial()
		{
			material = new Material(ShaderFlags.Position2DTextured, "EarthSpriteSheet");
			Assert.IsNotNull(material.SpriteSheet);
			Assert.AreEqual(12, material.SpriteSheet.UVs.Count);
			Assert.AreEqual(new Size(107, 80), material.SpriteSheet.SubImageSize);
		}

		private Material material;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderAnimatedSprite()
		{
			new Sprite(material, new Rectangle(0.4f, 0.4f, 0.2f, 0.2f));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckDurationFromMetaData()
		{
			var animation = new Sprite(material, center);
			Assert.AreEqual(1, animation.Material.SpriteSheet.DefaultDuration);
			Assert.AreEqual(1, animation.Material.Duration);
		}

		private readonly Rectangle center = Rectangle.FromCenter(Vector2D.Half, new Size(0.2f, 0.2f));

		[Test, CloseAfterFirstFrame]
		public void PlayFullAnimation()
		{
			var animation = new Sprite(material, center);
			bool endedHasBeenRaised = false;
			animation.AnimationEnded += () => endedHasBeenRaised = true;
			animation.Elapsed = animation.Material.Duration;
			AdvanceTimeAndUpdateEntities();
			Assert.True(endedHasBeenRaised);
		}

		[Test]
		public void CreateSpriteSheetAnimationWithNewTexture()
		{
			var data = new ImageCreationData(new Size(8, 8)) { BlendMode = BlendMode.Opaque };
			var image = ContentLoader.Create<Image>(data);
			FillImage(image);
			var animationData = new SpriteSheetAnimationCreationData(image, 2, new Size(2, 2));
			var texturedShader = ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.Position2DTextured));
			var newMaterial = new SpriteSheetAnimation(animationData).CreateMaterial(texturedShader);
			new Sprite(newMaterial, Rectangle.HalfCentered);
		}

		private static void FillImage(Image customImage)
		{
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8 * 8; i++)
				colors[i] = Color.GetRandomColor();
			customImage.Fill(colors);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenSpriteSheetAnimationDoesNotThrowException()
		{
			new Sprite(material, Rectangle.One) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}
	}
}