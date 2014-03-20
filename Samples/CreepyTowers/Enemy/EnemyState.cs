using System;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;

namespace CreepyTowers.Enemy
{
	public abstract class EnemyState
	{
		public Vulnerability[] VulnerabilityState;
		public readonly int NumberOfTowerTypes = Enum.GetNames(typeof(TowerType)).Length;

		public void SetVulnerabilitiesToNormal()
		{
			for (int i = 0; i < NumberOfTowerTypes; i++)
				VulnerabilityState[i] = Vulnerability.Normal;
		}

		public Vulnerability GetVulnerability(TowerType type)
		{
			return VulnerabilityState[(int)type];
		}

		public void SetVulnerability(TowerType towerType, Vulnerability type)
		{
			VulnerabilityState[(int)towerType] = type;
		}

		public float GetVulnerabilityValue(TowerType type)
		{
			var vulnerability = GetVulnerability(type);
			switch (vulnerability)
			{
			case Vulnerability.Vulnerable:
				return 3;
			case Vulnerability.Weak:
				return 2;
			case Vulnerability.Normal:
				return 1;
			case Vulnerability.Resistant:
				return 0.5f;
			case Vulnerability.HardBoiled:
				return 0.25f;
			case Vulnerability.Immune:
				return 0.1f;
			default:
				return 9001; //ncrunch: no coverage
			}
		}

		public void SetVulnerabilityWithValue(TowerType towerType, float value)
		{
			if (value == 0.1f)
				SetVulnerability(towerType, Vulnerability.Immune);
			else if (value == 0.25f)
				SetVulnerability(towerType, Vulnerability.HardBoiled);
			else if (value == 0.5f)
				SetVulnerability(towerType, Vulnerability.Resistant);
			else if (value == 1.0f)
				SetVulnerability(towerType, Vulnerability.Normal);
			else if (value == 2.0f)
				SetVulnerability(towerType, Vulnerability.Weak);
			else if (value == 3.0f)
				SetVulnerability(towerType, Vulnerability.Vulnerable);
			else
				SetVulnerability(towerType, Vulnerability.Sudden);
		}

		public bool Slow { get; set; }
		public float SlowTimer { get; set; }
		public bool SlowImmune { get; set; }
		public float SlowImmuneTimer { get; set; }
		public bool Fast { get; set; }
		public float FastTimer { get; set; }
		public bool Delayed { get; set; }
		public float DelayedTimer { get; set; }
		public bool Wet { get; set; }
		public float WetTimer { get; set; }
		public bool Melt { get; set; }
		public float MeltTimer { get; set; }
		public bool Enfeeble { get; set; }
		public float EnfeebleTimer { get; set; }
		public float MaxTimeShort = 2;
		public float MaxTimeMedium = 5;
		public float MaxTimeLong = 10;

		public abstract void UpdateStateAndTimers(Agent creep);
	}
}