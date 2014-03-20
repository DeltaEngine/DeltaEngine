using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	public class SimplePhysicsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateWindowForScreenSpace()
		{
			Resolve<Window>();
		}

		[Test, CloseAfterFirstFrame]
		public void FallingEffectIsRemovedAfterOneSecond()
		{
			var sprite = CreateFallingSpriteWhichExpires();
			CheckFallingEffectStateAfterOneSecond(sprite);
		}

		private static Sprite CreateFallingSpriteWhichExpires()
		{
			var sprite = new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"),
				Rectangle.One);
			sprite.Add(new SimplePhysics.Data
			{
				Velocity = Vector2D.Half,
				Gravity = new Vector2D(1.0f, 2.0f),
				Duration = 1.0f
			});
			sprite.Start<SimplePhysics.Move>();
			sprite.Color = Color.Red;
			return sprite;
		}

		private void CheckFallingEffectStateAfterOneSecond(Entity2D entity)
		{
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(1.534f, entity.DrawArea.Center.X, 0.01f);
			Assert.AreEqual(2.059f, entity.DrawArea.Center.Y, 0.01f);
		}

		[Test]
		public void RenderSlowlyFallingLogo()
		{
			var sprite = new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"),
				screenCenter);
			sprite.Add(new SimplePhysics.Data
			{
				Velocity = new Vector2D(0.0f, -0.3f),
				RotationSpeed = 100.0f,
				Gravity = new Vector2D(0.0f, 0.1f),
			});
			sprite.Color = Color.Red;
			sprite.Start<SimplePhysics.Move>();
		}

		private readonly Rectangle screenCenter = Rectangle.FromCenter(Vector2D.Half, new Size(0.2f));

		[Test]
		public void RenderFallingCircle()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Blue);
			ellipse.Add(new SimplePhysics.Data
			{
				Velocity = new Vector2D(0.1f, -0.1f),
				Gravity = new Vector2D(0.0f, 0.1f)
			});
			ellipse.Start<SimplePhysics.Move>();
		}

		[Test]
		public void RenderMovingCircleUsingExtension()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Blue);
			ellipse.StartMoving(new Vector2D(0.1f, -0.1f));
		}

		[Test]
		public void RenderFallingCircleUsingExtension()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.1f, 0.1f, Color.Blue);
			ellipse.StartFalling(new Vector2D(0.1f, -0.1f), new Vector2D(0.0f, 0.1f));
		}


		[Test]
		public void RenderRotatingRect()
		{
			var rect = new FilledRect(Rectangle.FromCenter(Vector2D.Half, new Size(0.2f)), Color.Orange)
			{
				Rotation = 0
			};
			rect.Add(new SimplePhysics.Data { Gravity = Vector2D.Zero, RotationSpeed = 5 });
			rect.Start<SimplePhysics.Rotate>();
		}

		[Test]
		public void RenderRotatingRectViaExtensionMethod()
		{
			new FilledRect(Rectangle.FromCenter(Vector2D.Half, new Size(0.2f)), Color.Red).StartRotating(5);
		}

		[Test]
		public void BounceOffScreenEdge()
		{
			var rect =
				new FilledRect(new Rectangle(ScreenSpace.Current.Viewport.TopLeft, new Size(0.2f)),
					Color.Red);
			rect.Add(new SimplePhysics.Data
			{
				Gravity = Vector2D.Zero,
				Velocity = new Vector2D(-0.1f, 0.0f)
			});
			rect.Start<SimplePhysics.Move>();
			rect.Start<SimplePhysics.BounceIfAtScreenEdge>();
			var collided = false;
			rect.Get<SimplePhysics.Data>().Bounced += () => collided = true;
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(collided);
			Assert.AreEqual(0.1f, rect.Get<SimplePhysics.Data>().Velocity.X);
		}

		[Test, CloseAfterFirstFrame]
		public void RotateAdvancesAngleCorrectly()
		{
			var rect = new FilledRect(new Rectangle(ScreenSpace.Current.Viewport.TopLeft, new Size(0.2f)),
				Color.Red);
			rect.Rotation = 0;
			rect.Add(new SimplePhysics.Data { Gravity = Vector2D.Zero, RotationSpeed = 0.1f });
			rect.Start<SimplePhysics.Rotate>();
			AdvanceTimeAndUpdateEntities();
			Assert.Greater(rect.Rotation, 0);
		}

		[Test]
		public void RenderMovingUVSprite()
		{
			var sprite = new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"),
				Rectangle.One);
			sprite.SetWithoutInterpolation(new Rectangle(0, 0, 0.1f, 0.1f));
			sprite.FlipMode = FlipMode.Vertical;
			sprite.StartMovingUV(Vector2D.One/10);
		}

		[Test]
		public void RenderBouncingOffScreenEdgesSprite()
		{
			var sprite = new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"), 
				new Rectangle(Vector2D.Zero, new Size(0.1f)));
			sprite.StartBouncingOffScreenEdges(Vector2D.Half, () => { });
		}

		[Test]
		public void KillSpriteAfterTimeout()
		{
			var enemyMovingOutOfScreen =
				new Sprite(new Material(ShaderFlags.Position2DTextured, "DeltaEngineLogo"),
					new Rectangle(Vector2D.Half, new Size(0.1f)));
			var data = new SimplePhysics.Data { Velocity = new Vector2D(0.5f, 0), Duration = 1 };
			enemyMovingOutOfScreen.Add(data);
			enemyMovingOutOfScreen.Start<SimplePhysics.Move>();
			enemyMovingOutOfScreen.Start<SimplePhysics.KillAfterDurationReached>();
			if (IsMockResolver)
				AdvanceTimeAndUpdateEntities(1.1f);
		}
	}
}