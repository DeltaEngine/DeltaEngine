using System;
using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Effects;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.Rendering3D.Shapes;

namespace CreepyTowers.Towers
{
	/// <summary>
	/// Towers shoot at creeps. Their stats and behavior vary depending on their type
	/// </summary>
	public sealed class Tower : Agent
	{
		public Tower(TowerType type, Vector3D position, float rotationZ = 0.0f)
		{
			Type = type;
			CreateStats(type, ContentLoader.Load<TowerPropertiesXml>(Xml.TowerProperties.ToString())); 
			Position = position;
			ScaleFactor = 1.0f;
			RotationZ = rotationZ;
			SetMultipliersFromAvatar();
			Cooldown = 1 / GetStatValue("AttackFrequency");
		}

		private float Cooldown { get; set; }
		public TowerType Type { get; private set; }

		public bool IsOnCooldown
		{
			get { return Cooldown > 0; }
		}

		private void CreateStats(TowerType type, TowerPropertiesXml properties)
		{
			TowerData towerData = properties.Get(type);
			Name = towerData.Name;
			AttackType = towerData.AttackType;
			CreateStat("Range", towerData.Range);
			CreateStat("AttackFrequency", towerData.AttackFrequency);
			CreateStat("Power", towerData.BasePower);
			CreateStat("Cost", towerData.Cost);
		}

		public AttackType AttackType { get; private set; }

		private void SetMultipliersFromAvatar()
		{
			var avatar = Player.Current.Avatar;
			ApplyBuff(new BuffEffect(avatar.GetType().Name + "AttackFrequencyMultiplier"));
			ApplyBuff(new BuffEffect(avatar.GetType().Name + "RangeMultiplier"));
			ApplyBuff(new BuffEffect(avatar.GetType().Name + "PowerMultiplier"));
		}

		protected override void InitializeModel()
		{
			ContentLoader.Load<Sound>("Tower" + Type + "Built").Play();
			GetAndAttachAttackEffect();
			//RenderDebugAttackRange();
			if (WasBuilt != null)
				WasBuilt();
		}

		public static event Action WasBuilt;

		private void GetAndAttachAttackEffect()
		{
			if (AttackType == AttackType.DirectShot)
				return;
			attackEffect = EffectLoader.GetAttackEffect(Type, AttackType);
			AddChild(attackEffect);
		}

		public ParticleSystem attackEffect;

		private void RenderDebugAttackRange()
		{
			var range = GetStatValue("Range");
			if (AttackType == AttackType.Circle)
				DebugRange.Add(new Circle3D(Position, range, Color.Red));
			else if (AttackType == AttackType.DirectShot)
				DebugRange.Add(new Line3D(Position, Position + Vector3D.UnitX * range, Color.Red));
			else
			{
				var leftPoint = (Vector3D.UnitX * range).RotateAround(Vector3D.UnitZ, 15);
				var rightPoint = (Vector3D.UnitX * range).RotateAround(Vector3D.UnitZ, -15);
				DebugRange.Add(new Line3D(Position, Position + leftPoint, Color.Red));
				DebugRange.Add(new Line3D(Position, Position + rightPoint, Color.Red));
			}
		}

		public readonly List<Entity3D> DebugRange = new List<Entity3D>();

		protected override void OnOrientationChanged()
		{
			if (Model != null)
				Model.Orientation = Orientation;
			base.OnOrientationChanged();
		}

		public override void Update()
		{
			base.Update();
			if (Cooldown > 0)
				Cooldown -= Time.Delta;
		}

		public void FireAtCreep(Creep creep)
		{
			PlayFireSound();
			VisualizeAttackDependingOnType(creep);
			TimeOfLastAttack = Time.Total;
			Cooldown = 1 / GetStatValue("AttackFrequency");
		}

		public float TimeOfLastAttack { get; private set; }

		private void PlayFireSound()
		{
			string fileName = "Tower" + Type + "Attack";
			var sound = ContentLoader.Load<Sound>(fileName);
			sound.Play();
		}

		private void VisualizeAttackDependingOnType(Creep targetCreep)
		{
			if (attackEffect != null)
				attackEffect.FireBurstOfAllEmitters(); //ncrunch: no coverage
			if (AttackType == AttackType.DirectShot || Type == TowerType.Slice)
				new InterpolatingProjectile(this, targetCreep,
					EffectLoader.GetProjectileEffect(Type, AttackType.DirectShot));
		}

		public void AimToTarget(Vector3D target)
		{
			var direction = target - Position;
			RotationZ = MathExtensions.Atan2(direction.Y, direction.X) + 90;
			//if (AttackType == AttackType.DirectShot)
			//	((Line3D)DebugRange[0]).EndPoint = target;
			//else if (AttackType == AttackType.Cone)
			//	AddConeDebugLines(direction);
		}

		private void AddConeDebugLines(Vector3D direction)
		{
			direction.Normalize();
			var range = GetStatValue("Range");
			var leftPoint = (direction * range).RotateAround(Vector3D.UnitZ, 15);
			var rightPoint = (direction * range).RotateAround(Vector3D.UnitZ, -15);
			((Line3D)DebugRange[0]).EndPoint = Position + leftPoint;
			((Line3D)DebugRange[1]).EndPoint = Position + rightPoint;
		}

		public override void Dispose()
		{
			foreach (var entity in DebugRange)
				entity.Dispose();
			DebugRange.Clear();
			base.Dispose();
		}
	}
}