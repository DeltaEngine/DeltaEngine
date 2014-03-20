using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Tests
{
	internal class Trigger2DTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			material = new Material(ShaderFlags.Position2DColoredTextured, "DeltaEngineLogo");
		}

		private Material material;

		[Test, CloseAfterFirstFrame]
		public void CreateTrigger()
		{
			var trigger = new Sprite(material, new Rectangle(Vector2D.Zero, (Size)Vector2D.One))
			{
				Color = Color.Red
			};
			trigger.Add(new TimeTrigger.Data(Color.Red, Color.Gray, 1));
			trigger.Start<CollisionTrigger>().Add(new CollisionTrigger.Data(Color.White, Color.Red));
			Assert.AreEqual(Vector2D.Zero, trigger.Get<Rectangle>().TopLeft);
			Assert.AreEqual(Vector2D.One, trigger.Get<Rectangle>().BottomRight);
		}

		[Test]
		public void ChangeColorIfTwoRectanglesCollide()
		{
			var sprite = new Sprite(material, new Rectangle(0.25f, 0.2f, 0.5f, 0.5f));
			sprite.Start<CollisionTrigger>().Add(new CollisionTrigger.Data(Color.Yellow, Color.Blue));
			sprite.Get<CollisionTrigger.Data>().SearchTags.Add("Creep");
			var sprite2 = new Sprite(material, new Rectangle(0.5f, 0.2f, 0.1f, 0.5f));
			sprite2.AddTag("Creep");
		}

		[Test]
		public void ChangeColorTwiceASecond()
		{
			var sprite = new Sprite(material, Rectangle.HalfCentered)
			{
				Color = Color.Green
			};
			sprite.Start<TimeTrigger>().Add(new TimeTrigger.Data(Color.Green, Color.Gold, 0.2f));
			AdvanceTimeAndUpdateEntities(0.03f);
			AdvanceTimeAndUpdateEntities(0.2f);
		}
	}
}