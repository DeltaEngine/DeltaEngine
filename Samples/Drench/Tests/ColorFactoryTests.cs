using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests
{
	public class ColorFactoryTests
	{
		[Test]
		public void RandomColorIsRounded()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.2f, 0.3f }));
			Color color = new ColorFactory().Generate(0.5f, 0.1f);
			Assert.AreEqual(new Color(0.5f, 0.6f, 0.6f), color);
		}
	}
}