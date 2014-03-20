using System.Collections.Generic;

namespace DeltaEngine.Editor.MaterialEditor
{
	public class MaterialEditorDesign
	{
		public MaterialEditorDesign()
		{
			MaterialName = "MyMaterial";
			ImageList = new List<string> { "Image 1" };
			SelectedImage = "Image 1";
			BlendModeList = new List<string> { "Normal" };
			SelectedBlendMode = "Normal";
			RenderStyleList = new List<string> { "PixelBased" };
			SelectedRenderSize = "PixelBased";
			ColorStringList = new List<string> { "White" };
			SelectedColor = "White";
			ShaderList = new List<string> { "Position2DColored" };
			SelectedShader = "Position2DColored";
		}

		public string MaterialName { get; set; }
		public List<string> ImageList { get; set; }
		public string SelectedImage { get; set; }
		public List<string> BlendModeList { get; set; }
		public string SelectedBlendMode { get; set; }
		public List<string> RenderStyleList { get; set; }
		public string SelectedRenderSize { get; set; }
		public List<string> ColorStringList { get; set; }
		public string SelectedColor { get; set; }
		public List<string> ShaderList { get; set; }
		public string SelectedShader { get; set; }
	}
}