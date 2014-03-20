using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Editor.LevelEditor
{
	public class BackgroundImageHandler
	{
		public BackgroundImageHandler(Level level)
		{
			this.level = level;
		}

		private readonly Level level;

		public void SetBackgroundImage(string selectedImage)
		{
			if (backgroundImage != null)
				backgroundImage.Dispose();
			if (selectedImage == "")
				return;
			if (ContentLoader.Exists(selectedImage, ContentType.Image))
				backgroundImage =
					new Sprite(new Material(ShaderFlags.Position2DColoredTextured, selectedImage),
						new Rectangle(GetBackgroundPosition(), GetBackgroundSize())) { RenderLayer = -10 };
		}

		private Sprite backgroundImage;

		private Size GetBackgroundSize()
		{
			return new Size(level.Size.Width * ZoomFactor, level.Size.Height * ZoomFactor);
		}

		private const float ZoomFactor = 0.05f;

		private Vector2D GetBackgroundPosition()
		{
			return new Vector2D(Vector2D.Half.X - GetBackgroundSize().Width * PositionFactor,
				Vector2D.Half.Y - GetBackgroundSize().Height * PositionFactor);
		}

		private const float PositionFactor = 0.5f;

		public void IncreaseBgImageSize()
		{
			if (backgroundImage != null)
				backgroundImage.Size += new Size(0.05f);
		}

		public void DecreaseBgImageSize()
		{
			if (backgroundImage.Size.Width >= 0.0f && backgroundImage.Size.Height >= 0.0f)
				backgroundImage.Size -= new Size(0.05f);
		}

		public void ResetBgImageSize()
		{
			backgroundImage.Size = GetBackgroundSize();
		}
	}
}