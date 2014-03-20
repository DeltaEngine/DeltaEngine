namespace DeltaEngine.Editor.SpriteFontCreator
{
	public class FontCreatorDesign
	{
		public FontCreatorDesign()
		{
			ContentName = "MyFont";
			FamilyFilename = "Verdana20";
			FontColor = "1, 1, 0.5, 1";
			ShadowColor = "0, 0, 0, 0.2";
			OutlineColor = "0, 0, 0, 0.2";
			OutlineThickness = "0,033";
			BestFontSize = "12";
			BestFontLineHeight = "1.0";
			BestFontTracking = "1.0";
		}

		public string ContentName { get; set; }
		public string FamilyFilename { get; set; }
		public string FontColor { get; set; }
		public string ShadowColor { get; set; }
		public string OutlineColor { get; set; }
		public string OutlineThickness { get; set; }
		public string BestFontSize { get; set; }
		public string BestFontLineHeight { get; set; }
		public string BestFontTracking { get; set; }
	}
}