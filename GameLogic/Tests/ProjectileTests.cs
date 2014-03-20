using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Physics3D;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.GameLogic.Tests
{
	internal class ProjectileTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateProjectileAndAddMissile()
		{
			projectileOwner = new MockActor(Vector3D.Zero, 0);
			projectileTarget = new MockActor(Vector3D.Zero, 0);
			var missile = new MockMissile();
			projectile = new Projectile(projectileOwner, projectileTarget, missile);
			Assert.AreEqual(missile, projectile.GetFirstChildOfType<MockMissile>());
		}

		private class MockMissile : PhysicalEntity3D {}

		private Projectile projectile;
		private Actor3D projectileOwner, projectileTarget;

		[Test, CloseAfterFirstFrame]
		public void PositionAndRotationOfProjectileAlsoModifiesParticleSystem()
		{
			projectile.Children[0].LocalPosition = Vector3D.UnitX;
			projectile.Orientation = Quaternion.FromAxisAngle(-Vector3D.UnitZ, 90);
			projectile.Position = Vector3D.UnitY;
			Assert.AreEqual(Quaternion.FromAxisAngle(-Vector3D.UnitZ, 90),
				projectile.Children[0].Orientation);
			Assert.AreEqual(0, projectile.Children[0].Position.X, 0.0001f);
			Assert.AreEqual(0, projectile.Children[0].Position.Y, 0.0001f);
			Assert.AreEqual(0, projectile.Children[0].Position.Z, 0.0001f);
		}

		[Test, CloseAfterFirstFrame]
		public void WithoutParentLocalEqualsGlobal()
		{
			projectile.UpdateGlobalsFromParent();
			projectile.LocalOrientation = Quaternion.Identity;
			Assert.AreEqual(Quaternion.Identity, projectile.Orientation);
			Assert.AreEqual(projectile.LocalPosition, projectile.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void RotatePhysicsProjectileWithSpeed()
		{
			projectile.RotationAxis = Vector3D.UnitZ;
			projectile.RotationSpeed = 60.0f;
			AdvanceTimeAndUpdateEntities();
			var expectedRotation = Quaternion.FromAxisAngle(Vector3D.UnitZ,
				60.0f / Settings.Current.UpdatesPerSecond);
			Assert.AreEqual(expectedRotation.W, projectile.Orientation.W, 0.001f);
			Assert.AreEqual(expectedRotation.X, projectile.Orientation.X, 0.001f);
			Assert.AreEqual(expectedRotation.Y, projectile.Orientation.Y, 0.001f);
			Assert.AreEqual(expectedRotation.Z, projectile.Orientation.Z, 0.01f);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromPositions()
		{
			var projectileFromPositions = new Projectile(Vector3D.Zero, Vector3D.UnitX,
				new PhysicalEntity3D());
			Assert.AreEqual(projectileFromPositions.Position, Vector3D.Zero);
			Assert.AreEqual(projectileFromPositions.Orientation, new Quaternion(-0.5f, 0.5f, 0.5f, 0.5f));
		}

		[Test, CloseAfterFirstFrame]
		public void DetonateGrenadeWithCustomAction()
		{
			CreateEnemies();
			var grenade = new Projectile(Vector3D.Zero, Vector3D.Zero, new PhysicalEntity3D(), 1, 0.2f);
			grenade.OnLifeTimeExceeded += () => { Detonate(grenade.Position); };
			AdvanceTimeAndUpdateEntities(grenade.LifeTime + 0.25f);
			foreach (EnemySpy enemySpy in enemies)
				if (enemySpy.Position.DistanceSquared(grenade.Position) < 9)
					Assert.AreEqual(80, enemySpy.HitPoints);
		}

		private void CreateEnemies()
		{
			enemies = new EnemySpy[3];
			enemies[0] = new EnemySpy(Vector3D.Zero);
			enemies[1] = new EnemySpy(Vector3D.UnitX);
			enemies[2] = new EnemySpy(Vector3D.UnitY);
		}

		private EnemySpy[] enemies;

		private void Detonate(Vector3D position)
		{
			foreach (EnemySpy enemy in enemies)
				if (enemy.Position.DistanceSquared(position) <= 9)
					enemy.DamageBy(20);
		}

		private class EnemySpy : PhysicalEntity3D
		{
			public EnemySpy(Vector3D position)
				: base(position, Quaternion.Identity)
			{
				HitPoints = 100;
			}

			public void DamageBy(int dmg)
			{
				HitPoints -= dmg;
			}

			public int HitPoints { get; private set; }
		}

		[Test, CloseAfterFirstFrame]
		public void DisposalDisposesMissile()
		{
			var missile = new PhysicalEntity3D();
			var grenade = new Projectile(Vector3D.Zero, Vector3D.Zero, missile, 1, 0.2f);
			grenade.IsActive = false;
			Assert.IsFalse(missile.IsActive);
		}
	}
}