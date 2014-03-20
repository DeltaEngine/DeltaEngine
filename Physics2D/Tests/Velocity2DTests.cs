using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class Velocity2DTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void InitializeVelocity()
		{
			velocity = new Velocity2D.Data(Vector2D.Zero, 5);
		}

		private Velocity2D.Data velocity;

		[Test]
		public void ApplyUpdateWithVelocity()
		{
			var entity2D = new Entity2D(new Rectangle(0, 0, 1, 1));
			entity2D.Add(velocity);
			entity2D.Start<Velocity2D.PositionUpdate>();
			var originalPosition = entity2D.Center;
			entity2D.Get<Velocity2D.Data>().Accelerate(2.0f, 90.0f);
			AdvanceTimeAndUpdateEntities();
			Assert.AreNotEqual(originalPosition, entity2D.Center);
		}

		[Test]
		public void AccelerateByPoint()
		{
			velocity.Accelerate(Vector2D.One);
			Assert.AreEqual(Vector2D.One, velocity.Velocity);
		}

		[Test]
		public void AccelerateByPointExceedingMaximum()
		{
			velocity.Accelerate(new Vector2D(6, 0));
			Assert.AreEqual(5, velocity.Velocity.X);
			Assert.AreEqual(0, velocity.Velocity.Y);
		}

		[Test]
		public void AccelerateByMagnitudeAngle()
		{
			velocity.Accelerate(4, 0);
			Assert.AreEqual(-4, velocity.Velocity.Y);
			Assert.AreEqual(0, velocity.Velocity.X);
		}

		[Test]
		public void AccelerateByMagnitudeAngleExceedingMaximum()
		{
			velocity.Accelerate(6, 0);
			Assert.AreEqual(-5, velocity.Velocity.Y);
			Assert.AreEqual(0, velocity.Velocity.X);
		}

		[Test]
		public void AccelerateByScalarFactor()
		{
			velocity.Accelerate(Vector2D.One);
			velocity.Accelerate(2);
			Assert.AreEqual(2, velocity.Velocity.X);
			Assert.AreEqual(2, velocity.Velocity.Y);
		}

		[Test]
		public void AccelerateByScalarFactorExceedingMaximum()
		{
			velocity.Accelerate(Vector2D.UnitX);
			velocity.Accelerate(-7.0f);
			Assert.AreEqual(-5, velocity.Velocity.X);
			Assert.AreEqual(0, velocity.Velocity.Y);
		}
	}
}