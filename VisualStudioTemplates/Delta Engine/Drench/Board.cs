using DeltaEngine.Datatypes;

namespace $safeprojectname$
{
	public class Board
	{
		public Board(int width, int height)
		{
			Width = width;
			Height = height;
			Randomize();
			floodFiller = new FloodFiller(colors);
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		private readonly FloodFiller floodFiller;
		internal Color[,] colors;

		public void Randomize()
		{
			colors = new Color[Width,Height];
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					colors[x, y] = colorFactory.Generate(MinimumColorValue, MinimumColorInterval);
		}

		private readonly ColorFactory colorFactory = new ColorFactory();
		private const float MinimumColorValue = 0.5f;
		private const float MinimumColorInterval = 0.5f;

		// Used to populate the board in a networked game
		public Board(Data data)
		{
			Width = data.Width;
			Height = data.Height;
			SetColors(data.Colors);
			floodFiller = new FloodFiller(colors);
		}

		private void SetColors(Color[] boardColors)
		{
			colors = new Color[Width,Height];
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					colors[x, y] = boardColors[y * Width + x];
		}

		public class Data
		{
			protected Data() {} //ncrunch: no coverage

			public Data(int width, int height, Color[,] colors)
			{
				Width = width;
				Height = height;
				Colors = new Color[Width * Height];
				for (int x = 0; x < Width; x++)
					for (int y = 0; y < Height; y++)
						Colors[y * Width + x] = colors[x, y];
			}

			public int Width { get; private set; }
			public int Height { get; private set; }
			public Color[] Colors { get; private set; }
		}

		public Color GetColor(Vector2D square)
		{
			return GetColor((int)square.X, (int)square.Y);
		}

		public Color GetColor(int x, int y)
		{
			return colors[x, y];
		}

		public void SetColor(Vector2D square, Color color)
		{
			SetColor((int)square.X, (int)square.Y, color);
		}

		public void SetColor(int x, int y, Color color)
		{
			floodFiller.SetColor(x, y, color);
		}

		public int GetConnectedColorsCount(Vector2D square)
		{
			var testFloodFiller = new FloodFiller((Color[,])colors.Clone());
			testFloodFiller.SetColor((int)square.X, (int)square.Y, Color.TransparentWhite);
			return testFloodFiller.ProcessedCount;
		}

		public Board Clone()
		{
			var clone = new Board(Width, Height);
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Height; y++)
					clone.colors[x, y] = colors[x, y];
			return clone;
		}
	}
}