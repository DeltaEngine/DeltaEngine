using System;
using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Entities;

namespace $safeprojectname$.Enemy.Creeps
{
	public class CreepState : EnemyState, IEquatable<CreepState>
	{
		public CreepState()
		{
			VulnerabilityState = new Vulnerability[NumberOfTowerTypes];
			SetVulnerabilitiesToNormal();
		}

		public bool Burn { get; set; }
		public float BurnTimer { get; set; }
		public bool Burst { get; set; }
		public float BurstTimer { get; set; }
		public bool Paralysed { get; set; }
		public float ParalysedTimer { get; set; }
		public bool Frozen { get; set; }
		public float FrozenTimer { get; set; }
		public bool Unfreezable { get; set; }
		public float UnfreezableTimer { get; set; }
		public bool Rust { get; set; }
		public float RustTimer { get; set; }
		public bool Healing { get; set; }
		public bool Sudden { get; set; }

		public bool Equals(CreepState other)
		{
			return Slow == other.Slow && SlowTimer == other.SlowTimer && Delayed == other.Delayed &&
				DelayedTimer == other.DelayedTimer && Burn == other.Burn && BurnTimer == other.BurnTimer &&
				Burst == other.Burst && BurstTimer == other.BurstTimer && Paralysed == other.Paralysed &&
				ParalysedTimer == other.ParalysedTimer && Frozen == other.Frozen &&
				FrozenTimer == other.FrozenTimer && Unfreezable == other.Unfreezable &&
				UnfreezableTimer == other.UnfreezableTimer && SlowImmune == other.SlowImmune &&
				SlowImmuneTimer == other.SlowImmuneTimer && Fast == other.Fast &&
				FastTimer == other.FastTimer && Enfeeble == other.Enfeeble &&
				EnfeebleTimer == other.EnfeebleTimer && Melt == other.Melt && MeltTimer == other.MeltTimer &&
				Rust == other.Rust && RustTimer == other.RustTimer && Wet == other.Wet &&
				WetTimer == other.WetTimer && Healing == other.Healing && Sudden == other.Sudden &&
				MaxTimeShort == other.MaxTimeShort && MaxTimeMedium == other.MaxTimeMedium &&
				MaxTimeLong == other.MaxTimeLong;
		}

		public override void UpdateStateAndTimers(Agent agent)
		{
			var creep = (Creep)agent;
			if (Paralysed)
				UpdateParalyzedState();
			if (Frozen)
				UpdateFrozenState(creep);
			if (Melt)
				UpdateMeltState(creep);
			if (Wet)
				UpdateWetState(creep);
			if (Slow)
				UpdateSlowState(creep);
			if (Unfreezable)
				UpdateUnfreezableState();
			if (SlowImmune)
				UpdateSlowImmuneState();
		}

		private void UpdateParalyzedState()
		{
			ParalysedTimer += Time.Delta;
			Paralysed = !(ParalysedTimer > MaxTimeShort);
		}

		private void UpdateFrozenState(Creep creep)
		{
			FrozenTimer += Time.Delta;
			Frozen = !(FrozenTimer > MaxTimeShort);
			if (Frozen)
				return;
			creep.ResetStateIcon(); //ncrunch: no coverage start
			Paralysed = false;
			Unfreezable = true;
			UnfreezableTimer = 0;
			Wet = true;
			WetTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.WetMat.ToString());
		}

		//ncrunch: no coverage end

		private void UpdateMeltState(Creep creep)
		{
			MeltTimer += Time.Delta;
			Melt = !(MeltTimer > MaxTimeShort);
			if (Melt)
				return;
			creep.ResetStateIcon(); //ncrunch: no coverage start
			creep.ResetTimeIcon();
			Slow = false;
			Enfeeble = false;
		}

		//ncrunch: no coverage end

		private void UpdateWetState(Creep creep)
		{
			WetTimer += Time.Delta;
			Wet = !(WetTimer > MaxTimeShort);
			if (Wet)
				return;
			Slow = false; //ncrunch: no coverage start
			creep.ResetTimeIcon();
			creep.ResetStateIcon();
		}

		//ncrunch: no coverage end

		private void UpdateSlowState(Creep creep)
		{
			SlowTimer += Time.Delta;
			Slow = !(SlowTimer > MaxTimeShort);
			if (Slow)
				return;
			creep.ResetTimeIcon(); //ncrunch: no coverage start
			SlowImmune = true;
			SlowImmuneTimer = 0;
		}

		//ncrunch: no coverage end

		private void UpdateUnfreezableState()
		{
			UnfreezableTimer += Time.Delta;
			Unfreezable = !(UnfreezableTimer > MaxTimeMedium);
		}

		private void UpdateSlowImmuneState()
		{
			SlowImmuneTimer += Time.Delta;
			SlowImmune = !(SlowImmuneTimer > MaxTimeShort);
		}
	}
}