using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace CreepyTowers.Triggers
{
	public class LifeIsLessThanZero : GameTrigger
	{
		public LifeIsLessThanZero(string value) {}

		protected override void GameOver()
		{
			new FontText(Font.Default, "Game Over", ScreenSpace.Current.Viewport.Increase(Size.Half));
		}
	}
}