using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace $safeprojectname$.UI
{
	class PointsTip
	{
		public PointsTip(NumberFactory numberFactory, Vector2D center, int value)
		{
			Time = GlobalTime.Current.Milliseconds;
			number = numberFactory.CreateNumber(null, center.X, center.Y, 0.02f, Alignment.Center, 0,
				DefaultColors.Yellow, GameRenderLayer.Points);
			number.Show(value);
			plus = new Sprite("Plus", Rectangle.FromCenter(center.X - 0.01f - number.Width / 2f, center.Y + number.Height / 2f, 0.01f, 0.01f));
			plus.RenderLayer = (int)GameRenderLayer.Points;
			plus.Color = DefaultColors.Yellow;
		}

		public readonly long Time;
		private readonly Sprite plus;
		private readonly Number number;
		
		public void Fade()
		{
			plus.Alpha -= GameSettings.FadeStep;
			number.Fade();
		}

		public void Reset()
		{
			plus.IsActive = false;
			number.Hide();
		}
	}
}