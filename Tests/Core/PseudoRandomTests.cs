using DeltaEngine.Core;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Tests.Core
{
	public class PseudoRandomTests
	{
		[Test]
		public void UseDefaultRandomizer()
		{
			Assert.Less(Randomizer.Current.Get(1, 100), 100);
		}

		[Test]
		public void GetRandomBrightColorUsingFixedRandomValues()
		{
			using (Randomizer.Use(new FixedRandom(new[] { 0.0f, 0.5f})))
			{
				Assert.AreEqual(0, Randomizer.Current.Get(0, 10));
				Assert.AreEqual(5, Randomizer.Current.Get(0, 10));
				Assert.AreEqual(2, Randomizer.Current.Get(2, 4));
			}
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void RandomIntSanityTest()
		{
			var random = new PseudoRandom();
			const int Max = 10;
			var wasChosen = new bool[Max];
			const int Trials = Max * 1000;
			for (int i = 0; i < Trials; i++)
				wasChosen[random.Get(0, Max)] = true;
			for (int i = 0; i < Max; i++)
				Assert.IsTrue(wasChosen[i]);
		}

		[Test, Category("Slow")]
		public void RandomFloatSanityTest()
		{
			var random = new PseudoRandom();
			const int Max = 10;
			var wasChosen = new bool[Max];
			const int Trials = Max * 1000;
			for (int i = 0; i < Trials; i++)
				wasChosen[(int)random.Get(0.0f, Max)] = true;
			for (int i = 0; i < Max; i++)
				Assert.IsTrue(wasChosen[i]);
		}
	}
}