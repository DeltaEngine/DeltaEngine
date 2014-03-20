using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	public class AnimationTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateMaterial()
		{
			material = new Material(ShaderFlags.Position2DTextured, "ImageAnimation");
		}

		private Material material;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderAnimatedSprite()
		{
			new Sprite(material, Vector2D.Half);
		}

		[Test]
		public void RenderEarthSpriteSheet()
		{
			new Sprite(new Material(ShaderFlags.Position2DTextured, "EarthSpriteSheet"), Vector2D.Half);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void Render2AnimatedSpritesWithDifferentDuration()
		{
			new Sprite(material, new Rectangle(0.3f, 0.4f, 0.2f, 0.2f));
			var material2 = new Material(ShaderFlags.Position2DTextured, "ImageAnimation") { Duration = 1 };
			new Sprite(material2, new Rectangle(0.55f, 0.4f, 0.2f, 0.2f));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckAnimatedTime()
		{
			var animation = new Sprite(material, Vector2D.Half);
			RunAfterFirstFrame(() =>
			{
				float elapsed = animation.Elapsed;
				var currentFrame = (int)elapsed % 3.0f;
				Assert.AreEqual(currentFrame, animation.CurrentFrame);
			});
		}

		[Test, CloseAfterFirstFrame]
		public void CreateAnimatedSprite()
		{
			var images = CreateImages();
			var animation = new Sprite(material, Vector2D.Half);
			Assert.AreEqual(images, animation.Material.Animation.Frames);
			Assert.AreEqual(images[0], animation.Material.DiffuseMap);
			Assert.AreEqual(3, animation.Material.Duration);
		}

		private static List<Image> CreateImages()
		{
			var images = new List<Image>
			{
				ContentLoader.Load<Image>("ImageAnimation01"),
				ContentLoader.Load<Image>("ImageAnimation02"),
				ContentLoader.Load<Image>("ImageAnimation03")
			};
			return images;
		}

		[Test, CloseAfterFirstFrame]
		public void CreateAnimatedSpriteNoImages()
		{
			Assert.Throws<ImageAnimation.NoImagesGivenNeedAtLeastOne>(
				() => ContentLoader.Load<ImageAnimation>("ImageAnimationNoImages"));
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeDuration()
		{
			material.Duration = 2;
			var animation = new Sprite(material, Vector2D.Half);
			Assert.AreEqual(3, animation.Material.Animation.DefaultDuration);
			Assert.AreEqual(2, animation.Material.Duration);
		}

		[Test, CloseAfterFirstFrame]
		public void ResetResetsCurrentFrameAndElapsed()
		{
			var animation = new Sprite(material, Vector2D.Half) { CurrentFrame = 3, Elapsed = 3.0f };
			animation.Reset();
			RunAfterFirstFrame(() =>
			{
				Assert.AreEqual(0, animation.CurrentFrame);
				Assert.AreEqual(0.05f, animation.Elapsed);
			});
		}

		[Test, ApproveFirstFrameScreenshot]
		public void CreateAnimationWithNewTextures()
		{
			var imageList = new[]
			{
				CreateImageWithColor(Color.Red), CreateImageWithColor(Color.CornflowerBlue),
				CreateImageWithColor(Color.Purple)
			};
			var newMaterial = new ImageAnimation(imageList, 3).CreateMaterial(
				ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DTextured)));
			new Sprite(newMaterial, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
		}

		[Test, CloseAfterFirstFrame]
		public void CreateAnimationWithEmptyTextures()
		{
			var emptyImageList = new Image[0];
			Assert.Throws<ImageAnimation.NoImagesGivenNeedAtLeastOne>(
				() => new ImageAnimation(emptyImageList, 3));
		}

		private static Image CreateImageWithColor(Color color)
		{
			var data = new ImageCreationData(new Size(8, 8)) { BlendMode = BlendMode.Opaque };
			var image = ContentLoader.Create<Image>(data);
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8 * 8; i++)
				colors[i] = color;
			image.Fill(colors);
			return image;
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenAnimationDoesNotThrowException()
		{
			new Sprite(material, Rectangle.One) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test]
		public void AdvancingTillLastFrameGivesEvent()
		{
			var animation = new Sprite(material, Vector2D.Half);
			bool endReached = false;
			animation.AnimationEnded += () => { endReached = true; };
			AdvanceTimeAndUpdateEntities(animation.Material.Duration + 0.1f);
			Assert.IsTrue(endReached, animation.ToString());
		}

		[Test, CloseAfterFirstFrame]
		public void FramesWillNotAdvanceIfIsPlayingFalse()
		{
			var animation = new Sprite(material, Vector2D.Half);
			bool endReached = false;
			animation.AnimationEnded += () => { endReached = true; };
			animation.IsPlaying = false;
			AdvanceTimeAndUpdateEntities(animation.Material.Duration);
			Assert.IsFalse(endReached);
		}
	}
}