using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.UIEditor
{
	public class SceneScreenSpace : QuadraticScreenSpace
	{
		public SceneScreenSpace(Window window, Size sceneResolution)
			: base(window)
		{
			viewportPixelSize = sceneResolution;
		}

		public Size SceneResolution
		{
			get { return viewportPixelSize; }
		}

		protected override void Update(Size newViewportSize)
		{
			base.Update(SceneResolution);
		}

		public void ForcedUpdate(Size newViewportSize)
		{
			viewportPixelSize = newViewportSize;
			Update(SceneResolution);
		}
	}
}