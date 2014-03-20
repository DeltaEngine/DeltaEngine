using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	public class AnimationEditorDesign
	{
		public AnimationEditorDesign()
		{
			AnimationName = "MyMaterial";
			ImageList = new List<string>(new[] { "Image 1", "Image 2", "Image 3", "..." });
			LoadedImageList = new List<string>(new[] { "Image 1", "Image 2", "Image 3", "Image 4" });
			SelectedImageToAdd = "Image 4";
			IsDisplayingAnimation = true;
			SubImageSize = new Size(128, 128);
			Duration = "1";
		}

		public string AnimationName { get; set; }
		public List<string> ImageList { get; set; }
		public List<string> LoadedImageList { get; set; }
		public string SelectedImageToAdd { get; set; }
		public bool IsDisplayingAnimation { get; set; }
		public string Duration { get; set; }
		public Size SubImageSize { get; set; }
	}
}