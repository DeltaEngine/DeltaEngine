using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class ImagePreviewer : ContentPreview
	{
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var image = ContentLoader.Load<Image>(contentName);
			var imageSize = image.PixelSize;
			var aspectRatio = imageSize.Height / imageSize.Width;
			if (currentDisplaySprite != null)
				currentDisplaySprite.IsActive = false;
			currentDisplaySprite = new Sprite(
				new Material(ShaderFlags.Position2DTextured, contentName),
				Rectangle.FromCenter(new Vector2D(0.5f, 0.5f), new Size(0.5f, 0.5f * aspectRatio)));
		}

		public Sprite currentDisplaySprite;
	}
}