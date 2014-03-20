using System;
using CreepyTowers.Content;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.Enemy.Creeps
{
	/// <summary>
	/// Enemy creature, its features determined by the type given to the constructor.
	/// </summary>
	public sealed class Creep : Enemy
	{
		public Creep(CreepType type, Vector3D position, float rotationZ = 0.0f)
			: base(position, rotationZ)
		{
			Type = type;
			CreateStats(type, ContentLoader.Load<CreepPropertiesXml>(Xml.CreepProperties.ToString()));
			SetupCreepHealthBar();
			SetupCreepIcons();
			InitializeModel();
		}

		public CreepType Type { get; private set; }

		private void CreateStats(CreepType type, CreepPropertiesXml properties)
		{
			CreepData creepData = properties.Get(type);
			Name = creepData.Name;
			CreateStat("Hp", creepData.MaxHp);
			CreateStat("Speed", creepData.Speed);
			CreateStat("Resistance", creepData.Resistance);
			CreateStat("Gold", creepData.GoldReward);
			if (Player.Current != null)
				ApplyBuff(new BuffEffect(Player.Current.Avatar.GetType().Name + "GoldMultiplier"));
			State = new CreepState();
			foreach (var modifier in creepData.TypeDamageModifier)
				State.SetVulnerabilityWithValue(modifier.Key, modifier.Value);
		}

		public CreepState State { get; private set; }

		private void SetupCreepHealthBar()
		{
			var drawArea = CalculateHealthBarDrawArea();
			HealthBar = new PercentageBar(drawArea, PercentileColors,
				PercentageBar.HorizontalAlignment.Center);
			HealthBar.RenderLayer = 10;
		}

		public PercentageBar HealthBar { get; private set; }
		private static readonly Color LowHealth = new Color(179, 2, 2);
		private static readonly Color MediumHealth = new Color(255, 157, 0);
		private static readonly Color HighHealth = new Color(35, 130, 38);
		private static readonly Color[] PercentileColors = { LowHealth, MediumHealth, HighHealth };

		private Rectangle CalculateHealthBarDrawArea()
		{
			var healthBar3DPos = new Vector3D(Position.X, Position.Y, Position.Z + 0.01f);
			var healthBar2DPos = Camera.Current.WorldToScreenPoint(healthBar3DPos);
			var barPos = new Vector2D(healthBar2DPos.X, healthBar2DPos.Y - 0.03f);
			var drawArea = Rectangle.FromCenter(barPos, HealthBarSize);
			return drawArea;
		}

		public Size HealthBarSize
		{
			get { return new Size(0.03f, 0.002f); }
		}

		private void SetupCreepIcons()
		{
			CreateDefaultMaterial();
			StateIcon = new Sprite(EmptyMaterial, CalculateIconDrawArea(StateIconOffset));
			TimeIcon = new Sprite(EmptyMaterial, CalculateIconDrawArea(TimeIconOffset));
		}

		public Sprite StateIcon { get; private set; }
		public Sprite TimeIcon { get; private set; }
		private const float StateIconOffset = -0.01f;
		private const float TimeIconOffset = 0.0f;

		public void CreateDefaultMaterial()
		{
			var defaultImage = ContentLoader.Create<Image>(new ImageCreationData(new Size(48)));
			defaultImage.BlendMode = BlendMode.Normal;
			defaultImage.Fill(Color.TransparentBlack);
			var defaultShader =
				ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColoredTextured));
			EmptyMaterial = new Material(defaultShader, defaultImage);
		}

		public Material EmptyMaterial { get; private set; }

		private Rectangle CalculateIconDrawArea(float offset)
		{
			var iconBar3DPos = new Vector3D(Position.X, Position.Y, Position.Z + 0.25f);
			var iconBar2DPos = Camera.Current.WorldToScreenPoint(iconBar3DPos);
			var iconPos = new Vector2D(iconBar2DPos.X + 0.01f + offset, iconBar2DPos.Y - 0.04f);
			var drawArea = Rectangle.FromCenter(iconPos, IconSize);
			return drawArea;
		}

		public Size IconSize
		{
			get { return new Size(0.01f); }
		}

		protected override void InitializeModel()
		{
			base.InitializeModel();
			if (WasSpawned != null)
				WasSpawned(this);
		}

		public static event Action<Creep> WasSpawned;

		public void RecalculateHitpointBar()
		{
			HealthBar.DrawArea = CalculateHealthBarDrawArea();
			HealthBar.Value = GetStatPercentage("Hp") * 100.0f;
		}

		public void ModifyStatusIcons()
		{
			StateIcon.DrawArea = CalculateIconDrawArea(StateIconOffset);
			TimeIcon.DrawArea = CalculateIconDrawArea(TimeIconOffset);
		}

		public void ResetStateIcon()
		{
			StateIcon.Material = EmptyMaterial;
		}

		public void ResetTimeIcon()
		{
			TimeIcon.Material = EmptyMaterial;
		}

		public override void ReceiveAttack(TowerType damageType, float rawDamage,
			float typeInteraction = 1.0f)
		{
			if (!IsActive)
				return;
			UpdateDamageState(damageType);
			SpawnHitSparks(damageType);
			var interactionEffect = State.GetVulnerabilityValue(damageType);
			float resistance = State.Enfeeble
				? GetStatValue("Resistance") * 0.5f : GetStatValue("Resistance");
			var damage = CalculateDamage(rawDamage, resistance, interactionEffect * typeInteraction);
			AdjustStat(new StatAdjustment("Hp", "", -damage));
			if (HealthBar != null)
				RecalculateHitpointBar();
			if (GetStatValue("Hp") <= 0.0f)
			{
				CreepDyingSoundSelector.PlayDestroyedSound(Type);
				Die();
			}
		}

		public override void UpdateDamageState(TowerType type)
		{
			StateChanger.CheckCreepState(type, this);
		}

		public override float CalculateDamage(float rawDamage, float resistance,
			float interactionEffect)
		{
			if (!State.Healing)
				return (rawDamage - resistance) * interactionEffect;
			State.Healing = false;
			return -(rawDamage * 0.5f);
		}

		public override void Update()
		{
			base.Update();
			if (State.Burst)
				if (Time.CheckEvery(EachSecond))
					ApplyDotEffect(TowerType.Fire, GetStatBaseValue("Hp") * BurstDamagePerSecond);
			if (State.Burn)
				if (Time.CheckEvery(EachSecond))
					ApplyDotEffect(TowerType.Fire, GetStatBaseValue("Hp") * BurnDamagePerSecond);
			State.UpdateStateAndTimers(this);
		}

		private void ApplyDotEffect(TowerType damageType, float damage)
		{
			AdjustStat(new StatAdjustment("Hp", "", -damage));
			SpawnHitSparks(damageType);
			if (GetStatValue("Hp") <= 0.0f)
				Die();
		}

		private const float EachSecond = 1.0f;
		private const float BurstDamagePerSecond = 1 / 12.0f;
		private const float BurnDamagePerSecond = 1 / 16.0f;

		public void Shatter()
		{
			var creepList = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			foreach (Creep creep in creepList)
				if (creep != this)
					if (creep.Position.DistanceSquared(Position) <= DistanceToReceiveShatterSquared)
						creep.AdjustStat(new StatAdjustment("Hp", "", -AmountHpHurtReceived));
		}

		private const float DistanceToReceiveShatterSquared = 1.0f;
		private const float AmountHpHurtReceived = 15.0f;

		public override void HasReachedExit()
		{
			PlaySound(GameSounds.CreepExit);
			GameTrigger.OnEnemyReachGoal();
			base.HasReachedExit();
		}

		private static void PlaySound(GameSounds soundName)
		{
			var sound = ContentLoader.Load<Sound>(soundName.ToString());
			if (sound != null)
				sound.Play();
		}

		public override void Dispose()
		{
			if (HealthBar != null)
				HealthBar.Dispose();
			if (StateIcon != null)
				StateIcon.Dispose();
			if (TimeIcon != null)
				TimeIcon.Dispose();
			base.Dispose();
		}

		public void ChangeCreepType(CreepType newType, float percentage)
		{
			if (Model != null)
				Model.Dispose(); //ncrunch: no coverage
			Type = newType;
			var properties = ContentLoader.Load<CreepPropertiesXml>(Xml.CreepProperties.ToString());
			CreepData creepData = properties.Get(newType);
			Name = creepData.Name;
			RestartStatsAndState(percentage, creepData);
			RenderModel();
		}

		protected override void RestartStatsAndState(float percentage, AgentData data)
		{
			var creepData = (CreepData)data;
			Stats.Clear();
			CreateStat("Hp", creepData.MaxHp);
			var amountToSubtract = (1 - percentage) * creepData.MaxHp;
			AdjustStat(new StatAdjustment("Hp", "", -amountToSubtract));
			CreateStat("Speed", creepData.Speed);
			CreateStat("Resistance", creepData.Resistance);
			CreateStat("Gold", creepData.GoldReward);
			State = new CreepState();
			foreach (var modifier in creepData.TypeDamageModifier)
				State.SetVulnerabilityWithValue(modifier.Key, modifier.Value);
		}
	}
}