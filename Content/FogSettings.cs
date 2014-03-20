using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	public class FogSettings
	{
		public static FogSettings Current
		{
			get
			{
				if (currentSettings == null)
					currentSettings = new FogSettings(Color.Gray, 10, 100);
				return currentSettings;
			}
		}

		private static FogSettings currentSettings;

		public FogSettings(Color fogColor, float fogStart, float fogEnd)
		{
			FogColor = fogColor;
			FogStart = fogStart;
			FogEnd = fogEnd;
			currentSettings = this;
		}

		public Color FogColor { get; set; }
		public float FogStart { get; set; }
		public float FogEnd { get; set; }
	}
}