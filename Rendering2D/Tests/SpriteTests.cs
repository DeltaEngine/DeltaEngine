using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering2D.Tests
{
	public class SpriteTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateMaterial()
		{
			logoMaterial = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAlpha");
		}

		private Material logoMaterial;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderSprite()
		{
			new Sprite(logoMaterial, Rectangle.HalfCentered);
		}

		[Test]
		public void RenderManySprites()
		{
			var random = Randomizer.Current;
			for (int num = 0; num < 20; num++)
				new Sprite(logoMaterial,
					new Rectangle(random.Get(0.0f, 0.8f), random.Get(0.2f, 0.8f), 0.2f, 0.2f));
		}

		[Test]
		public void RenderManyNonImageGrayColoredSprites()
		{
			var nonImageMaterial = new Material(Color.Gray);
			var random = Randomizer.Current;
			for (int num = 0; num < 20; num++)
				new Sprite(nonImageMaterial,
					new Rectangle(random.Get(0.0f, 0.8f), random.Get(0.2f, 0.8f), 0.2f, 0.2f));
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderGeneratedSpriteWithRedGreenBlueAndYellowColors()
		{
			var customMaterial = new Material(new Size(4, 1));
			customMaterial.DiffuseMap.Fill(new[] { Color.Red, Color.Green, Color.Blue, Color.Yellow });
			new Sprite(customMaterial, Rectangle.HalfCentered);
		}

		[Test, CloseAfterFirstFrame]
		public void ResetNonAnimationSprite()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			sprite.Elapsed = 4f;
			sprite.Reset();
			Assert.AreEqual(0f, sprite.Elapsed);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderSpriteWithRedOutline()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			sprite.Add(new OutlineColor(Color.Red));
			sprite.OnDraw<DrawPolygon2DOutlines>();
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderInactivatedAndReactivatedSprite()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			sprite.IsActive = false;
			sprite.IsActive = true;
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRedSpriteOverBlue()
		{
			var colorLogoMaterial =
				new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogoAlpha");
			colorLogoMaterial.DefaultColor = Color.Red;
			new Sprite(colorLogoMaterial, Rectangle.HalfCentered) { RenderLayer = 1 };
			colorLogoMaterial.DefaultColor = Color.Blue;
			new Sprite(colorLogoMaterial, screenTopLeft) { RenderLayer = 0 };
		}

		private readonly Rectangle screenTopLeft = Rectangle.FromCenter(0.3f, 0.3f, 0.5f, 0.5f);

		[Test, CloseAfterFirstFrame]
		public void CreateSprite()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			Assert.AreEqual(Color.White, sprite.Color);
			Assert.AreEqual("DeltaEngineLogoAlpha", sprite.Material.DiffuseMap.Name);
			Assert.IsTrue(sprite.Material.DiffuseMap.PixelSize == DiskContentSize ||
				sprite.Material.DiffuseMap.PixelSize == MockContentSize);
		}

		private static readonly Size DiskContentSize = new Size(128, 128); //ncrunch: no coverage
		private static readonly Size MockContentSize = new Size(4, 4); //ncrunch: no coverage

		[Test, CloseAfterFirstFrame]
		public void ChangingMaterialChangesImageAndBlendMode()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			Assert.AreEqual("DeltaEngineLogoAlpha", sprite.Material.DiffuseMap.Name);
			Assert.AreEqual(BlendMode.Normal, sprite.BlendMode);
			var material = new Material(ShaderFlags.Position2DTextured, "Verdana12Font");
			material.DiffuseMap.BlendMode = BlendMode.Opaque;
			sprite.Material = material;
			Assert.AreEqual("Verdana12Font", sprite.Material.DiffuseMap.Name);
			Assert.AreEqual(BlendMode.Opaque, sprite.BlendMode);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawLinesUnderAndOverSprite()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Blue) { RenderLayer = -1 };
			new Sprite(logoMaterial, Rectangle.HalfCentered);
			new Line2D(Vector2D.UnitX, Vector2D.UnitY, Color.Purple) { RenderLayer = 1 };
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingTwoSpritesWithTheSameImageAndRenderLayerOnlyIssuesOneDrawCall()
		{
			new Sprite(logoMaterial, Rectangle.HalfCentered);
			new Sprite(logoMaterial, Rectangle.HalfCentered);
			RunAfterFirstFrame(
				() => Assert.AreEqual(1, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingTwoSpritesWithTheSameImageButDifferentRenderLayersIssuesTwoDrawCalls()
		{
			new Sprite(logoMaterial, Rectangle.HalfCentered).RenderLayer = 1;
			new Sprite(logoMaterial, Rectangle.HalfCentered).RenderLayer = 2;
			RunAfterFirstFrame(
				() => Assert.AreEqual(2, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, CloseAfterFirstFrame]
		public void DrawingTwoSpritesWithDifferentImagesIssuesTwoDrawCalls()
		{
			new Sprite(logoMaterial, Rectangle.HalfCentered);
			new Sprite(
				new Material(ShaderFlags.Position2DTextured, "EarthSpriteSheet"), Rectangle.HalfCentered);
			RunAfterFirstFrame(
				() => Assert.AreEqual(2, Resolve<Drawing>().NumberOfDynamicDrawCallsThisFrame));
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSpritesWithDifferentBlendModes()
		{
			Resolve<Window>().Title =
				"Blend modes: Opaque, Normal, Additive, AlphaTest, LightEffect, Subtractive";
			var opaqueMaterial = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAlpha");
			var alphaMaterial = new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAlpha");
			var drawAreas = CreateDrawAreas(3, 2);
			new Sprite(opaqueMaterial, drawAreas[0]) { BlendMode = BlendMode.Opaque };
			new Sprite(alphaMaterial, drawAreas[1]) { BlendMode = BlendMode.Opaque };
			new Sprite(opaqueMaterial, drawAreas[2]) { BlendMode = BlendMode.Normal };
			new Sprite(alphaMaterial, drawAreas[3]) { BlendMode = BlendMode.Normal };
			new Sprite(opaqueMaterial, drawAreas[4]) { BlendMode = BlendMode.Additive };
			new Sprite(alphaMaterial, drawAreas[5]) { BlendMode = BlendMode.Additive };
			new Sprite(opaqueMaterial, drawAreas[6]) { BlendMode = BlendMode.AlphaTest };
			new Sprite(alphaMaterial, drawAreas[7]) { BlendMode = BlendMode.AlphaTest };
			new Sprite(opaqueMaterial, drawAreas[8]) { BlendMode = BlendMode.LightEffect };
			new Sprite(alphaMaterial, drawAreas[9]) { BlendMode = BlendMode.LightEffect };
			new Sprite(opaqueMaterial, drawAreas[10]) { BlendMode = BlendMode.Subtractive };
			new Sprite(alphaMaterial, drawAreas[11]) { BlendMode = BlendMode.Subtractive };
		}

		private static Rectangle[] CreateDrawAreas(int cols, int rows)
		{
			var drawAreas = new Rectangle[cols * rows * 2];
			var size = new Size(0.2f, 0.2f);
			var position1 = new Vector2D(0.2f, 0.35f);
			for (int y = 0; y < rows; y++)
			{
				for (int x = 0; x < cols; x++)
				{
					var index = x * 2 + (y * cols * rows);
					drawAreas[index] = Rectangle.FromCenter(position1, size);
					var position2 = new Vector2D(position1.X + 0.04f, position1.Y + 0.04f);
					drawAreas[index + 1] = Rectangle.FromCenter(position2, size);
					position1.X += 0.3f;
				}
				position1 = new Vector2D(0.2f, position1.Y + 0.275f);
			}
			return drawAreas;
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawSpritesWithBlendModeFromContentMetaData()
		{
			var drawAreas = CreateDrawAreas(3, 1);
			new Sprite(logoMaterial, drawAreas[0]);
			new Sprite(
				new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoOpaque"), drawAreas[1]);
			new Sprite(logoMaterial, drawAreas[2]);
			new Sprite(
				new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAlpha"), drawAreas[3]);
			new Sprite(logoMaterial, drawAreas[4]);
			new Sprite(
				new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogoAdditive"), drawAreas[5]);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingHiddenSpriteDoesNotThrowException()
		{
			new Sprite(logoMaterial, Rectangle.One) { IsVisible = false };
			Assert.DoesNotThrow(() => AdvanceTimeAndUpdateEntities());
		}

		[Test]
		public void ResizeViewportAndThenRenderFullscreenSprite()
		{
			Resolve<Window>().ViewportPixelSize = new Size(800, 600);
			new Sprite(logoMaterial, Rectangle.One);
		}

		[Test]
		public void RenderFullscreenSpriteAndThenResizeViewport()
		{
			new Sprite(logoMaterial, Rectangle.One);
			Resolve<Window>().ViewportPixelSize = new Size(800, 600);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderRotatedSprite()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.FromCenter(Vector2D.Half, new Size(0.5f)));
			sprite.Rotation = 60;
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawFlippedSprites()
		{
			new Sprite(logoMaterial, Rectangle.FromCenter(new Vector2D(0.2f, 0.35f), new Size(0.2f)));
			var flippedX = new Sprite(logoMaterial,
				Rectangle.FromCenter(new Vector2D(0.6f, 0.35f), new Size(0.2f)));
			flippedX.FlipMode = FlipMode.Horizontal;
			var flippedY = new Sprite(logoMaterial,
				Rectangle.FromCenter(new Vector2D(0.2f, 0.65f), new Size(0.2f)));
			flippedY.FlipMode = FlipMode.Vertical;
			var flippedXy = new Sprite(logoMaterial,
				Rectangle.FromCenter(new Vector2D(0.6f, 0.65f), new Size(0.2f)));
			flippedXy.FlipMode = FlipMode.HorizontalAndVertical;
		}

		[Test]
		public void RenderPanAndZoomIntoLogo()
		{
			ScreenSpace.Current = new Camera2DScreenSpace(Resolve<Window>());
			var logo = new Sprite(logoMaterial, Rectangle.FromCenter(Vector2D.One, new Size(0.25f)));
			logo.Start<PanAndZoom>();
		}

		private class PanAndZoom : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				var camera = ScreenSpace.Current as Camera2DScreenSpace;
				camera.LookAt = Vector2D.Half.Lerp(Vector2D.One, Time.Total / 2);
				camera.Zoom = 1.0f.Lerp(2.0f, Time.Total / 4);
			}
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawColoredSprite()
		{
			var sprite = new Sprite(
				new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogoAlpha"),
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.2f)));
			sprite.Color = Color.Red;
		}

		[Test]
		public void SettingRenderLayerOnInvisibleSpriteLeavesItInvisible()
		{
			var sprite = new Sprite(logoMaterial, Rectangle.HalfCentered);
			sprite.IsVisible = false;
			sprite.RenderLayer = 1;
			sprite.DrawArea = Rectangle.One;
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawModifiedUVSprite()
		{
			var sprite = new Sprite(
				new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogoAlpha"),
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.2f)));
			sprite.LastUV = sprite.UV = new Rectangle(0, 0, 0.5f, 0.5f);
		}

		[Test, CloseAfterFirstFrame]
		public void AddingUVCalculatorResultsThrowsException()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", Rectangle.One);
			Assert.Throws<Sprite.RenderingDataComponentAddingIsNotSupported>(
				() => sprite.Add(new RenderingData()));
		}

		[Test, CloseAfterFirstFrame]
		public void SetDrawAreaWithoutInterpolation()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", Rectangle.Zero);
			sprite.SetWithoutInterpolation(Rectangle.One);
			sprite.SetWithoutInterpolation(sprite.renderingData);
			Assert.AreEqual(Rectangle.One, sprite.DrawArea);
			Assert.AreEqual(Rectangle.One, sprite.LastDrawArea);
			Assert.AreEqual(sprite.renderingData, sprite.Get<RenderingData>());
		}

		[Test, CloseAfterFirstFrame]
		public void GetComponentsForSavingIncludesUvCalculatorResults()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", Rectangle.Zero);
			Assert.IsTrue(sprite.GetComponentsForSaving().Any(c => c is RenderingData));
		}

		//ncrunch: no coverage start (Time.IsPaused can mess up tests being run in parallel)
		[Test, Ignore]
		public void ChangingDrawAreaWhenPausedDrawsCorrectly()
		{
			Time.IsPaused = true;
			var sprite = new Sprite(logoMaterial, new Rectangle(0.1f, 0.1f, 0.2f, 0.2f));
			sprite.Center = new Vector2D(0.6f, 0.6f);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.6f, 0.6f), sprite.Get<RenderingData>().DrawArea.Center);
			Time.IsPaused = false;
		} //ncrunch: no coverage end

		[Test]
		public void SettingUVDoesNotSetLastUV()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", Rectangle.Zero) { UV = Rectangle.HalfCentered };
			Assert.AreEqual(Rectangle.One, sprite.LastUV);
		}

		[Test]
		public void ChangeLastUV()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", Rectangle.Zero);
			sprite.LastUV = Rectangle.HalfCentered;
			Assert.AreEqual(Rectangle.HalfCentered, sprite.LastUV);
		}

		//ncrunch: no coverage start (rarely fails on CI server, test manually)
		[Test, CloseAfterFirstFrame, Ignore]
		public void AddBehaviorRemovedPreviously()
		{
			var sprite = new Sprite("DeltaEngineLogoAlpha", new Rectangle(0.0f, 0.0f, 0.1f, 0.1f));
			sprite.Start<SimpleSizeUpdater>();
			AdvanceTimeAndUpdateEntities();
			sprite.Start<SimpleSizeUpdater>();
			AdvanceTimeAndUpdateEntities();
			Assert.That(sprite.Size.IsNearlyEqual(new Size(0.4f)));
		}

		private class SimpleSizeUpdater : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Sprite sprite in entities.OfType<Sprite>())
				{
					sprite.DrawArea = new Rectangle(sprite.TopLeft, sprite.Size * 2);
					sprite.Stop<SimpleSizeUpdater>();
				}
			}
		}
	}
}