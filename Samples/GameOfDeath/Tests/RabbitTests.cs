using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	public class RabbitTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowSingleRabbit()
		{
				CreateRabbitWith50Health(Vector2D.Half);
		}

		[Test]
		public void DamageSingleRabbitToHalfHealth()
		{
				var rabbit = CreateRabbitWith50Health(Vector2D.Half);
				rabbit.DoDamage(25);
		}

		private static Rabbit CreateRabbitWith50Health(Vector2D position)
		{
			var rabbit = new Rabbit(position, new Size(0.1f));
			rabbit.SetHealth(50);
			return rabbit;
		}

		[Test]
		public void ShowManyRabbits()
		{
			var viewport = Resolve<ScreenSpace>().Viewport;
			var size = new Size(0.1f);
			for (float x = viewport.Left + size.Width / 2; x <= viewport.Right; x += size.Width)
				for (float y = viewport.Top + size.Height / 2; y <= viewport.Bottom; y += size.Height)
					CreateRabbitWith50Health(new Vector2D(x, y));
		}
	}
}