using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace CreepyTowers.Triggers
{
	public class TimerReachZero : GameTrigger
	{
		public TimerReachZero(string value) {}

		protected override void GameOver()
		{
			var rectangle = ScreenSpace.Current.Viewport.Increase(Size.Half);
			rectangle.Top += 0.1f;
			var text = new FontText(Font.Default, "Time Is Over", rectangle);
			text.HorizontalAlignment = HorizontalAlignment.Center;
			text.VerticalAlignment = VerticalAlignment.Center;
		}
	}
}