using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace DeltaNinja.Entities
{
	public class LogoFactory
	{
		public LogoFactory(ScreenSpace screen)
		{
			this.screen = screen;
		}

		private readonly ScreenSpace screen;

		public Logo Create()
		{
			Randomizer random = Randomizer.Current;
			Size size = new Size(random.Get(0.02f, 0.08f));
			var halfWidth = size.Width / 2f;
			var doubleWidth = size.Width * 2f;
			Rectangle view = screen.Viewport;
			Vector2D position = new Vector2D(random.Get(doubleWidth, view.Width - doubleWidth), view.Bottom - size.Height / 2);
			float direction = position.X > 0.5f ? -1 : 1;
			if (random.Get(1, 100) >= 30) direction *= -1;
			float r = direction > 0
				? random.Get(0, view.Width - position.X - doubleWidth)
				: random.Get(0, position.X - doubleWidth);
			var h = random.Get(0.3f, view.Height - 0.05f);
			var angle = Math.Atan((4 * h) / r);
			if (angle == 0)
				angle = 1.57079f;
			var v0 = Math.Sqrt(r * MovingSprite.Gravity / Math.Sin(2 * angle));
			var v_x = (float)(v0 * Math.Cos(angle));
			var v_y = (float)(v0 * Math.Sin(angle));
			v_x *= direction;
			var data = new SimplePhysics.Data()
			{
				Gravity = new Vector2D(0f, MovingSprite.Gravity),
				Velocity = new Vector2D(v_x, -v_y),
				RotationSpeed = random.Get(10, 50) * direction
			};
			return new Logo("DeltaEngineLogo", Color.GetRandomBrightColor(), position, size, data);
		}
	}
}