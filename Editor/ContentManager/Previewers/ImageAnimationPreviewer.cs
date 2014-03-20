using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public sealed class ImageAnimationPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material(ShaderFlags.Position2DTextured, contentName),
				new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			try
			{
				TryPreview();
			}
			catch (Exception) //ncrunch: no coverage start
			{
				LogUnableToLoadContentMessage(contentName);
				currentDisplayAnimation.IsActive = false;
			} //ncrunch: no coverage end
		}

		private Sprite currentDisplayAnimation;

		private void TryPreview()
		{
			foreach (var frame in currentDisplayAnimation.Material.Animation.Frames)
				ContentLoader.Load<Image>(frame.Name);
		}
	}
}