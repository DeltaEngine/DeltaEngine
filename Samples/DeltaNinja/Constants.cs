using DeltaEngine.Datatypes;

namespace DeltaNinja
{
	public static class DefaultColors
	{
		public readonly static Color Gray = new Color(0xc8, 0xb7, 0xb7);
		public readonly static Color Red = new Color(0xff, 0x2a, 0x2a);
		public readonly static Color Yellow = new Color(0xff, 0xcc, 0x00);
		public readonly static Color Cyan = new Color(0x71, 0xB2, 0xB8);
		public readonly static Color HoverButton = Red;  
		public readonly static Color NormalButton = Gray;
	}

	public static class GameSettings
	{
		public const float InitialWaveTimeout = 0.8f;
		public const int CriticalBonus = 10;
		public const float CriticalHeight = 0.25f;
		public const float FadeStep = 0.0015f;
	}

	public enum MenuButton
	{
		Home,
		NewGame,
		Resume,
		Abort,
		Retry,
		About,
		Exit
	}

	public enum GameRenderLayer
	{
		Default,
		Background,
		Menu,
		Hud,
		Logos,		
		Segments,
		Points
	}

	public enum Alignment
	{
		Left,
		Center,
		Right
	}
}