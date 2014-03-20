using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace $safeprojectname$
{
	internal class ColorFactory
	{
		public ColorFactory()
		{
			randomizer = Randomizer.Current;
		}

		private readonly Randomizer randomizer;

		public Color Generate(float minimum, float interval)
		{
			float r = GetRandomRoundedValue(minimum, interval);
			float g = GetRandomRoundedValue(minimum, interval);
			float b = GetRandomRoundedValue(minimum, interval);
			return new Color(r, g, b);
		}

		private float GetRandomRoundedValue(float minimum, float interval)
		{
			var maxValue = (int)((1.0f - minimum) / interval) + 1;
			var value = randomizer.Get(0, maxValue);
			return minimum + value * interval;
		}
	}
}