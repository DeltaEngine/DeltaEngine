using System.Collections.Generic;
using CreepyTowers.Stats;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D;

namespace $safeprojectname$
{
	/// <summary>
	/// Creeps and Towers are both Agents
	/// </summary>
	public abstract class Agent : Actor3D, Updateable
	{
		protected Agent()
			: base(Vector3D.Zero)
		{
			Stats = new Dictionary<string, Stat>();
		}

		public string Name { get; protected set; }
		protected Dictionary<string, Stat> Stats { get; set; }

		public void CreateStat(string name, float value)
		{
			Stats.Add(name, new Stat(value));
		}

		public void AdjustStat(StatAdjustment adjustment)
		{
			Stat stat;
			if (!Stats.TryGetValue(adjustment.Attribute, out stat))
				return;
			if (adjustment.Adjustment > 0)
				BoostStat(stat, adjustment.Adjustment);
			else
				ReduceStat(stat, adjustment.Resist, adjustment.Adjustment);
		}

		private static void BoostStat(Stat stat, float adjustment)
		{
			stat.Adjust(adjustment);
		}

		private void ReduceStat(Stat stat, string resistance, float adjustment)
		{
			float resistValue = GetResistValue(resistance);
			var netAdjustment = adjustment + resistValue;
			if (netAdjustment < 0)
				stat.Adjust(netAdjustment);
		}

		private float GetResistValue(string resistance)
		{
			Stat resist;
			return Stats.TryGetValue(resistance, out resist) ? resist.Value : 0;
		}

		public void ApplyBuff(BuffEffect effect)
		{
			if (effect.Attribute == null)
				return;
			Stat buffedStat;
			if (!Stats.TryGetValue(effect.Attribute, out buffedStat))
				return;
			buffedStat.ApplyBuff(effect);
			buffs.Add(new Buff(buffedStat, effect));
		}

		private readonly List<Buff> buffs = new List<Buff>();

		public float GetStatValue(string attribute)
		{
			Stat stat;
			return Stats.TryGetValue(attribute, out stat) ? stat.Value : -1.0f;
		}

		public float GetStatBaseValue(string attribute)
		{
			Stat stat;
			return Stats.TryGetValue(attribute, out stat) ? stat.BaseValue : -1.0f;
		}

		public float GetStatPercentage(string attribute)
		{
			Stat stat;
			return Stats.TryGetValue(attribute, out stat) ? stat.Percentage : -1.0f;
		}

		public virtual void Update()
		{
			IncrementBuffElapsedTime();
			RemoveExpiredBuffs();
		}

		private void IncrementBuffElapsedTime()
		{
			foreach (Buff buff in buffs)
				buff.Elapsed += Time.Delta;
		}

		private void RemoveExpiredBuffs()
		{
			for (int i = buffs.Count - 1; i >= 0; i--)
				if (buffs[i].IsExpired)
					RemoveExpiredBuff(buffs[i]);
		}

		private void RemoveExpiredBuff(Buff buff)
		{
			buff.Stat.RemoveBuff(buff.Effect);
			buffs.Remove(buff);
		}

		public void ClearBuffs()
		{
			foreach (Buff buff in buffs)
				buff.Stat.RemoveBuff(buff.Effect);
			buffs.Clear();
		}

		public override void RenderModel()
		{
			Model = new Model(Name, Position)
			{
				Scale = Scale,
				Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, RotationZ)
			};
			InitializeModel();
		}

		public Model Model { get; protected set; }
		protected abstract void InitializeModel();

		public override void Dispose()
		{
			if (Model != null)
				Model.Dispose();
			base.Dispose();
		}
	}
}