using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D.Shapes;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class LevelPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			new LevelDebugRenderer(ContentLoader.Load<Level>(contentName));
		}
	}
}