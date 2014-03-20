using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Holds frames inside a single image as opposed to ImageAnimation for rendering animated sprites
	/// </summary>
	public class SpriteSheetAnimation : ContentData
	{
		protected SpriteSheetAnimation(string contentName)
			: base(contentName) {}

		public SpriteSheetAnimation(SpriteSheetAnimationCreationData creationData)
			: base("<GeneratedSpriteSheetAnimation>")
		{
			Image = creationData.Image;
			DefaultDuration = creationData.DefaultDuration;
			SubImageSize = creationData.SubImageSize;
			CreateUVs();
		}

		public Image Image { get; private set; }
		public float DefaultDuration { get; private set; }
		public Size SubImageSize { get; private set; }

		private void CreateUVs()
		{
			UVs = new List<Rectangle>();
			for (int y = 0; y < Image.PixelSize.Height / SubImageSize.Height; y++)
				for (int x = 0; x < Image.PixelSize.Width / SubImageSize.Width; x++)
					UVs.Add(Rectangle.BuildUVRectangle(CalculatePixelRect(x, y), Image.PixelSize));
		}

		public List<Rectangle> UVs { get; private set; }

		private Rectangle CalculatePixelRect(int x, int y)
		{
			return new Rectangle(x * SubImageSize.Width, y * SubImageSize.Height, SubImageSize.Width,
				SubImageSize.Height);
		}

		protected override void LoadData(Stream fileData)
		{
			var imageName = MetaData.Get("ImageName", "");
			if (string.IsNullOrEmpty(imageName))
				throw new NeedValidImageName(); //ncrunch: no coverage
			Image = ContentLoader.Load<Image>(imageName);
			DefaultDuration = MetaData.Get("DefaultDuration", 0.0f);
			SubImageSize = MetaData.Get("SubImageSize", Size.Zero);
			if (SubImageSize == Size.Zero)
				throw new NeedValidSubImageSize(); //ncrunch: no coverage
			CreateUVs();
		}

		private class NeedValidImageName : Exception { }
		private class NeedValidSubImageSize : Exception { }

		protected override void DisposeData()
		{
			if (Image != null)
				Image.Dispose();
			Image = null;
		}

		public Material CreateMaterial(Shader shader)
		{
			return new Material(shader, null) { SpriteSheet = this };
		}
	}
}