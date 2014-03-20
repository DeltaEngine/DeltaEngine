using System;
using System.Collections.Generic;
using System.Linq;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Mostly used for testing, but also for deterministic values always returning the same sequence.
	/// </summary>
	public class FixedRandom : Randomizer
	{
		public FixedRandom()
			: this(new[] { 0.0f }) {}

		public FixedRandom(float[] fixedValues)
		{
			this.fixedValues = fixedValues.Clone() as float[];
			if (IsFixedValueOutOfRange(fixedValues))
				throw new FixedValueOutOfRange();
		}

		private static bool IsFixedValueOutOfRange(IEnumerable<float> fixedValues)
		{
			return fixedValues != null && fixedValues.Any(value => value < 0.0f || value >= 1.0f);
		}

		private readonly float[] fixedValues;

		public class FixedValueOutOfRange : Exception {}

		public override float Get(float min = 0.0f, float max = 1.0f)
		{
			var value = fixedValues[index++ % fixedValues.Length];
			return value * (max - min) + min;
		}

		private int index;

		public override int Get(int min, int max)
		{
			return (int)Get((float)min, max);
		}
	}
}