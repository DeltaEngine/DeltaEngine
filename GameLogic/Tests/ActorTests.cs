using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	public class ActorTests : TestWithMocksOrVisually
	{
		[Test]
		public void TestActorSpawnDespawnNotChangeAnything()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			actor.RenderModel();
			Assert.IsFalse(actor.Is2D());
			Assert.AreEqual(Vector3D.One, actor.Position);
		}

		[Test]
		public void CheckChangePosition()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			actor.PositionChanged += () => { CheckPositionHasChanged(actor); };
			actor.Position = new Vector3D(2, 2, 2);
		}

		private static void CheckPositionHasChanged(MockActor actor)
		{
			Assert.AreEqual(new Vector3D(2, 2, 2), actor.Position);
		}

		[Test]
		public void CheckChangeScale()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			actor.ScaleChanged += () => { CheckScaleHasChanged(actor); };
			actor.Scale = new Vector3D(2, 2, 2);
		}

		private static void CheckScaleHasChanged(MockActor actor)
		{
			Assert.AreEqual(new Vector3D(2, 2, 2), actor.Scale);
		}

		[Test]
		public void CheckChangeRotationZ()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			actor.OrientationChanged += () => { CheckRotationZHasChanged(actor); };
			actor.RotationZ = 90.0f;
		}

		private static void CheckRotationZHasChanged(MockActor actor)
		{
			Assert.AreEqual(90.0f, actor.RotationZ);
		}

		[Test]
		public void CheckChangeScaleFactor()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			actor.ScaleFactor = 2;
			Assert.AreEqual(2, actor.ScaleFactor);
		}

		[Test]
		public void TestDrawArea()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			Assert.AreEqual(new Rectangle(0.5f, 0.5f, 1, 1), actor.GetDrawArea());
		}

		[Test]
		public void TestBoundingBox()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			var box = new BoundingBox(new Vector3D(0.5f, 0.5f, 0.5f), new Vector3D(1.5f, 1.5f, 1.5f));
			Assert.AreEqual(box, actor.GetBoundingBox());
		}

		[Test]
		public void TestBoundingSphere()
		{
			var actor = new MockActor(Vector3D.One, 1.0f);
			var sphere = new BoundingSphere(Vector3D.One, 1.0f);
			Assert.AreEqual(sphere, actor.GetBoundingSphere());
		}

		[Test]
		public void TestIfTwoActorsAreCollidingIn3D()
		{
			var actor1 = new MockActor(Vector3D.One, 1.0f);
			var actor2 = new MockActor(Vector3D.Zero, 0.0f);
			Assert.IsTrue(actor1.IsColliding(actor2));
		}

		[Test]
		public void TestIfTwoActorsAreCollidingIn2D()
		{
			var actor1 = new MockActor(Vector2D.One, 0.0f, Vector2D.One);
			var actor2 = new MockActor(Vector2D.Zero, 0.0f, Vector2D.One);
			Assert.IsTrue(actor1.IsColliding(actor2));
		}
	}

	public class MockActor : Actor3D
	{
		public MockActor(Vector3D position, float rotationZ)
			: this(position, rotationZ, Vector3D.One) {}

		public MockActor(Vector3D position, float rotationZ, Vector3D scale)
			: base(position)
		{
			RotationZ = rotationZ;
			Scale = scale;
		}

		public override void RenderModel() {}
	}
}