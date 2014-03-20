using DeltaEngine.Extensions;

namespace $safeprojectname$.Stats
{
	/// <summary>
	/// This holds the value of a creep/tower statistic - eg. creep health, tower firing rate etc.
	/// It can be adjusted (eg a creep loses health on being shot) or buffed/debuffed (eg a tower
	/// increases firing rate due to being close to a booster tower)
	/// </summary>
	public class Stat
	{
		public Stat(float baseValue)
		{
			BaseValue = baseValue;
		}

		public float BaseValue { get; private set; }

		public void Adjust(float amount)
		{
			adjustment += amount;
			if (adjustment > 0)
				adjustment = 0;

			if (adjustment < -MaxValue)
				adjustment = -MaxValue;
		}

		private float adjustment;

		public void ApplyBuff(BuffEffect effect)
		{
			buffMultipliers *= effect.Multiplier;
			buffAdditions += effect.Addition;
		}

		private float buffMultipliers = 1.0f;
		private float buffAdditions;

		public void RemoveBuff(BuffEffect effect)
		{
			buffMultipliers /= effect.Multiplier;
			buffAdditions -= effect.Addition;
		}

		public float Percentage
		{
			get
			{
				var value = Value;
				var maxValue = MaxValue;
				return maxValue == 0 ? 0 : value / maxValue;
			}
		}

		public float Value
		{
			get { return MaxValue + adjustment; }
		}

		public float MaxValue
		{
			get { return MathExtensions.Max(0, BaseValue * buffMultipliers + buffAdditions); }
		}

		public override string ToString()
		{
			return Value + "/" + MaxValue;
		}
	}
}