using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaNinja.UI
{
	class ErrorFlag : Sprite
	{
		public ErrorFlag(float left, float width, float bottom)
			: base("ErrorIcon", new Rectangle(left, bottom - width, width, width))
		{
			Color = DefaultColors.Red;
			Time = GlobalTime.Current.Milliseconds;
			RenderLayer = (int)GameRenderLayer.Points;			
		}

		public readonly long Time;

		public void Fade()
		{
			Alpha -= GameSettings.FadeStep;
		}
	}
}