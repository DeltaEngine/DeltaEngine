using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Editor.Core
{
	public static class ContentExtensions
	{
		public static Material CreateDefaultMaterial2D(Color color)
		{
			return new Material(CreateShader2D(), CreateImage(color));
		}

		private static Shader CreateShader2D()
		{
			return
				ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColoredTextured));
		}

		private static Image CreateImage(Color? color)
		{
			var imageData = new ImageCreationData(new Size(Width, Height));
			imageData.DisableLinearFiltering = true;
			var image = ContentLoader.Create<Image>(imageData);
			if (color.HasValue)
				image.Fill(color.Value);
			else
				image.Fill(GetCheckerColors());
			return image;
		}

		private const int Width = 8;
		private const int Height = 8;

		public static Color[] GetCheckerColors()
		{
			var colors = new Color[Width * Height];
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					if ((x + y) % 2 == 0)
						colors[x * Width + y] = Color.LightGray;
					else
						colors[x * Height + y] = Color.DarkGray;
			return colors;
		}

		public static Material CreateDefaultMaterial2D()
		{
			return new Material(CreateShader2D(), CreateImage(null));
		}
	}
}