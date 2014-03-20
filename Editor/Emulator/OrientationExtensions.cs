using DeltaEngine.Core;

namespace DeltaEngine.Editor.Emulator
{
	public static class OrientationExtensions
	{
		public static bool IsLandscape(this Orientation orientation)
		{
			return orientation == Orientation.Landscape;
		}

		public static bool IsPortrait(this Orientation orientation)
		{
			return orientation == Orientation.Portrait;
		}
	}
}