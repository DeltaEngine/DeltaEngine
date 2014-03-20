using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class PlayerShipTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			playerShip = new PlayerShip();
		}

		private PlayerShip playerShip;

		[Test, CloseAfterFirstFrame]
		public void Accelerate()
		{
			Vector2D originalVelocity = playerShip.Get<Velocity2D>().Velocity;
			playerShip.ShipAccelerate();
			Assert.AreNotEqual(originalVelocity, playerShip.Get<Velocity2D>().Velocity);
		}

		[Test, CloseAfterFirstFrame]
		public void TurnChangesAngleCorrectly()
		{
			float originalAngle = playerShip.Rotation;
			playerShip.SteerLeft();
			Assert.Less(playerShip.Rotation, originalAngle);
			originalAngle = playerShip.Rotation;
			playerShip.SteerRight();
			Assert.Greater(playerShip.Rotation, originalAngle);
		}

		[Test, CloseAfterFirstFrame]
		public void FireRocket()
		{
			if(IsMockResolver)
				return; //ncrunch: no coverage start
			bool firedRocket = false;
			playerShip.ProjectileFired += () => { firedRocket = true; };
			playerShip.IsFiring = true;
			AdvanceTimeAndUpdateEntities(1f);
			Assert.IsTrue(firedRocket);
		} //ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void HittingBordersTopLeft()
		{
			playerShip.Set(new Rectangle(ScreenSpace.Current.TopLeft - new Vector2D(0.1f, 0.1f), 
				new Size(.05f)));
		}

		[Test, CloseAfterFirstFrame]
		public void HittingBordersBottomRight()
		{
			playerShip.Set(new Rectangle(ScreenSpace.Current.BottomRight, new Size(.05f)));
		}

		[Test, CloseAfterFirstFrame]
		public void CreatedEntityIsPauseable()
		{
			Assert.IsTrue(playerShip.IsPauseable);
		}

		[Test, CloseAfterFirstFrame]
		public void VelocityCannotExceedMaximum()
		{
			var velocity = new Velocity2D(Vector2D.Zero, 0.5f);
			velocity.Accelerate(1.0f,0);
			Assert.AreEqual(-0.5f * Vector2D.UnitY, velocity.Velocity);
		}
	}
}