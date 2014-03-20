using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.ScreenSpaces;

namespace SideScroller
{
	/// <summary>
	/// Abstract base airplane that holds the funtionality PlayerPlane and EnemyPlane have in common.
	/// </summary>
	public abstract class Plane : Sprite
	{
		protected Plane(Material texture, Vector2D initialPosition)
			: base(texture, Rectangle.FromCenter(initialPosition, new Size(0.1f, 0.05f)))
		{
			mgCadenceInverse = 0.02f;
			missileCadenceInverse = 2.0f;
			Start<TimeAndPositionUpdate>();

		}

		protected ParticleSystem machineGunAndLauncher;

		internal float verticalDecelerationFactor, verticalAccelerationFactor;
		protected const float MaximumSpeed = 2;

		public int Hitpoints
		{
			get { return hitpoints; }
			protected set
			{
				hitpoints = value;
				if (hitpoints <= 0)
					InvokeDestroyed();
			}
		}

		private void InvokeDestroyed()
		{
			if (defeated)
				return;
			defeated = true;
			if (Destroyed != null)
				Destroyed();
		}

		private int hitpoints;
		internal bool defeated;
		public event Action Destroyed;

		protected float elapsedSinceLastMGShot;
		protected float elapsedSinceLastMissile;

		protected float mgCadenceInverse;
		protected float missileCadenceInverse;

		public void FireMgShotIfAllowed()
		{
			if (elapsedSinceLastMGShot > mgCadenceInverse)
			{
				machineGunAndLauncher.AttachedEmitters[0].Spawn();
				elapsedSinceLastMGShot = 0;
			}
		}

		public void FireMissileIfAllowed()
		{
			if (elapsedSinceLastMissile > missileCadenceInverse)
			{
				machineGunAndLauncher.AttachedEmitters[1].Spawn();
				elapsedSinceLastMissile = 0;
			}
		}

		protected class TimeAndPositionUpdate : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var plane = entity as Plane;
					if (plane.defeated)
					{
						plane.AccelerateVertically(0.03f);
						if (plane.DrawArea.Top > ScreenSpace.Current.Viewport.Bottom)
							plane.IsActive = false;
					}
					plane.elapsedSinceLastMGShot += Time.Delta;
					plane.elapsedSinceLastMissile += Time.Delta;
					plane.machineGunAndLauncher.Position = new Vector3D(plane.Center);
					plane.machineGunAndLauncher.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ,
						plane.Rotation);
				}
			}
		}

		public void AccelerateVertically(float magnitude)
		{
			Get<Velocity2D>().Accelerate(new Vector2D(0, verticalAccelerationFactor * magnitude));
			verticalDecelerationFactor = 0.8f;
		}

		public void StopVertically()
		{
			verticalDecelerationFactor = 4.0f;
		}

		public float YPosition
		{
			get { return Get<Rectangle>().Center.Y; }
		}

		public void ReceiveAttack(int damage = 1, bool isExplosion = false)
		{
			Hitpoints -= damage;
		}

		public override bool IsActive
		{
			get { return base.IsActive; }
			set
			{
				if (!value)
					foreach (var emitter in machineGunAndLauncher.AttachedEmitters)
						emitter.DisposeAfterSeconds(3.0f);
				base.IsActive = value;
			}
		}
	}
}