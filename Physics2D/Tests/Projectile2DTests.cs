using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics2D.Tests
{
	public class Projectile2DTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void CreateProjectile()
		{
			var bulletImpulse = new Vector2D(0.3f, 0.3f);
			const int BulletDamage = 10;
			var bullet = CreateBullet(bulletImpulse, BulletDamage);
			Assert.IsTrue(bullet.Get<PhysicsBody>().LinearVelocity.IsNearlyEqual(bulletImpulse / 1000000f));
			Assert.AreEqual(BulletDamage, bullet.Damage);
		}

		private Projectile2D CreateBullet(Vector2D impulse, int damage)
		{
			return new Projectile2D(Resolve<Physics>(), impulse,
				Rectangle.FromCenter(Vector2D.Half, new Size(0.1f)), damage);
		}

		//ncrunch: no coverage start, fails randomly, not good for CI server
		[Test, CloseAfterFirstFrame, Ignore]
		public void ExceedingLifeTime()
		{
			var exceeded = false;
			var bullet = CreateBullet(Vector2D.One, 0);
			bullet.LifeTime = 0.1f;
			bullet.OnLifeTimeExceeded += () => exceeded = true;
			AdvanceTimeAndUpdateEntities(0.4f);
			Assert.IsFalse(bullet.IsActive);
			Assert.IsTrue(exceeded);
		} //ncrunch: no coverage end

		[Test, CloseAfterFirstFrame]
		public void ObjectAffixedToPhysicsBody()
		{
			var bullet = CreateBullet(Vector2D.Zero, 0);
			var physicsBody = bullet.Get<PhysicsBody>();
			physicsBody.Position = Vector2D.One;
			physicsBody.Rotation = 10;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(10, bullet.Rotation);
			Assert.AreEqual(1.0f, bullet.Center.X, 0.01f);
			Assert.AreEqual(1.0f, bullet.Center.Y, 0.01f);
		}
	}
}