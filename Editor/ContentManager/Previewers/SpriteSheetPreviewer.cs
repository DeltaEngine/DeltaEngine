using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class SpriteSheetPreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			currentDisplayAnimation = new Sprite(new Material(ShaderFlags.Position2DTextured,
				contentName), new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			try
			{
				TryPreview();
			}
			catch (Exception)
			{
				LogUnableToLoadContentMessage(contentName);
				currentDisplayAnimation.IsActive = false;
			}
		}

		public Sprite currentDisplayAnimation;

		private void TryPreview()
		{
			ContentLoader.Load<Image>(currentDisplayAnimation.Material.SpriteSheet.Image.Name);
		}
	}
}