using System;
using DeltaEngine.Extensions;

namespace DeltaEngine.Core
{
	/// <summary>
	/// The definition for all random number generators.
	/// </summary>
	public abstract class Randomizer
	{
		public static Randomizer Current
		{
			get { return ThreadStaticRandomizer.Current; }
		}

		private static readonly ThreadStatic<Randomizer> ThreadStaticRandomizer =
			new ThreadStatic<Randomizer>(new PseudoRandom());

		public static IDisposable Use(Randomizer randomizer)
		{
			return ThreadStaticRandomizer.Use(randomizer);
		}

		/// <summary>
		/// Returns a float between min and max, by default a value between zero and one.
		/// </summary>
		public abstract float Get(float min = 0, float max = 1);

		/// <summary>
		/// Returns an integer greater than or equal to min and strictly less than max
		/// </summary>
		public abstract int Get(int min, int max);
	}
}