using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	public class SpriteSheetAnimationCreationData : ContentCreationData
	{
		public SpriteSheetAnimationCreationData(Image image, float duration, Size subImageSize)
		{
			Image = image;
			DefaultDuration = duration;
			SubImageSize = subImageSize;
		}

		public Image Image { get; private set; }
		public float DefaultDuration { get; private set; }
		public Size SubImageSize { get; private set; }
	}
}