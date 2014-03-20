using System.Collections.Generic;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorDesign
	{
		//ncrunch: no coverage start 
		public UIEditorDesign()
		{
			ResolutionList = new List<string>(new[] { "20 x 20" });
			SelectedResolution = "20 x 20";
			NewGridWidth = "30";
			NewGridHeight = "30";
			IsShowingGrid = true;
			UIName = "MyScene";
			UIWidth = "1024";
			UIHeight = "768";
			ControlName = "NewButton0";
			Entity2DWidth = "0.2";
			Entity2DHeight = "0.1";
			ControlLayer = "0";
			BottomMargin = "0";
			TopMargin = "0";
			LeftMargin = "0";
			RightMargin = "0";
			MaterialList = new List<string>(new[]
			{
				"DefaultMaterial", "HoverMaterial", "PressedMaterial",
				"DisableMaterial"
			});
			SelectedMaterial = "DefaultMaterial";
			SelectedHoveredMaterial = "HoverMaterial";
			SelectedPressedMaterial = "PressedMaterial";
			SelectedDisabledMaterial = "DisableMaterial";
			ContentText = "Default Button";
			AvailableFontsInProject = new List<string>(new[] { "Verdana20" });
			SelectedFontName = "Verdana20";
		}


		public List<string> ResolutionList { get; set; }
		public string SelectedResolution { get; set; }
		public string NewGridWidth { get; set; }
		public string NewGridHeight { get; set; }
		public bool IsShowingGrid { get; set; }
		public string UIName { get; set; }
		public string UIWidth { get; set; }
		public string UIHeight { get; set; }
		public string ControlName { get; set; }
		public string Entity2DWidth { get; set; }
		public string Entity2DHeight { get; set; }
		public string ControlLayer { get; set; }
		public string BottomMargin { get; set; }
		public string TopMargin { get; set; }
		public string LeftMargin { get; set; }
		public string RightMargin { get; set; }
		public List<string> MaterialList { get; set; }
		public string SelectedMaterial { get; set; }
		public string SelectedHoveredMaterial { get; set; }
		public string SelectedPressedMaterial { get; set; }
		public string SelectedDisabledMaterial { get; set; }
		public string ContentText { get; set; }
		public List<string> AvailableFontsInProject { get; set; }
		public string SelectedFontName { get; set; }
	}
}