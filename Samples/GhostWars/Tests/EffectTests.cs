using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GhostWars.Tests
{
	public class EffectTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowArrow()
		{
			new Command(Command.Click, position => Effects.CreateArrow(Vector2D.Half, position));
		}

		[Test]
		public void ShowDeathEffect()
		{
			new Command(Command.Click, position => Effects.CreateDeathEffect(position));
		}

		[Test]
		public void ShowHitEffect()
		{
			new Command(Command.Click, position => Effects.CreateHitEffect(position));
		}

		[Test]
		public void ShowSparkleEffect()
		{
			var effect = Effects.CreateSparkleEffect(Team.HumanYellow, Vector2D.Half, 20);
		}

		[Test]
		public void CreateDeathEffect()
		{
			var effect = Effects.CreateDeathEffect(Vector2D.Half);
			Assert.IsNotNull(effect);
		}

		[Test]
		public void CreateHitEffect()
		{
			var effect = Effects.CreateHitEffect(Vector2D.Half);
			Assert.IsNotNull(effect);
		}
	}
}