using System.Threading.Tasks;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace FractalZoomer
{
	/// <summary>
	/// Provides a fast Mandelbrot fractal zoomer, use the Fractal constructor parameters to customize
	/// the iterations and color scaling. Utilizes all your CPU cores to calculate new images. Fractal
	/// uses doubles, which is pretty good for zooming in about a million times, then it gets blurry.
	/// </summary>
	public class Zoomer : Entity
	{
		public Zoomer(Window window)
		{
			CreateFractalView();
			window.ViewportSizeChanged += ResizeSafelyToMultipleOfFour;
			UpdateFractalRegion(new Rectangle(-2, -1.5f, 3, 3));
			CreateCommands(window);
		}

		//ncrunch: no coverage start
		private void ResizeSafelyToMultipleOfFour(Size size)
		{
			Size = new Size(4 * (int)(size.Width / 4), 4 * (int)(size.Height / 4));
			if (Colors.Length == Size.Width * Size.Height)
				return;
			Colors = new Color[(int)(Size.Width * Size.Height)];
			view.Dispose();
			material = new Material(Size);
			view = new Sprite(material, Vector2D.Half);
			text.DrawArea = ScreenSpace.Current.Viewport;
		} //ncrunch: no coverage end

		private void CreateFractalView()
		{
			fractal = new Fractal();
			Size = Settings.Current.Resolution;
			Colors = new Color[(int)(Size.Width * Size.Height)];
			material = new Material(Size);
			view = new Sprite(material, Vector2D.Half);
			text = new FontText(Font.Default, "", ScreenSpace.Current.Viewport);
			text.HorizontalAlignment = HorizontalAlignment.Left;
			text.VerticalAlignment = VerticalAlignment.Bottom;
			CreateSmoothColors();
		}

		private Fractal fractal;
		public Size Size { get; private set; }
		public Color[] Colors { get; private set; }
		private Material material;
		private Sprite view;
		private FontText text;

		private void CreateSmoothColors()
		{
			for (int i = 0; i < NumberOfSmoothColors; i++)
			{
				byte red = 0;
				byte green = 0;
				byte blue = 0;
				if (i >= 768)
				{
					red = (byte)(1023 - i);
				}
				else if (i >= 512)
				{
					red = (byte)(i - 512);
					green = (byte)(255 - red);
				}
				else if (i >= 256)
				{
					green = (byte)(i - 256);
					blue = (byte)(255 - green);
				}
				else
					blue = (byte)i;
				smoothColors[i] = new Color(red, green, blue);
			}
		}

		private const int NumberOfSmoothColors = 1024;
		private readonly Color[] smoothColors  = new Color[NumberOfSmoothColors];

		private void UpdateFractalRegion(Rectangle setArea)
		{
			if (isUpdating)
				return; //ncrunch: no coverage
			isUpdating = true;
			area = setArea;
			text.Text =
				"X " + area.Left + ", Y " + area.Top + ", Width " + area.Width + ", Height " + area.Height;
			Parallel.For(0, (int)Size.Height, y =>
			{
				for (int x = 0; x < Size.Width; x++)
					Colors[y * (int)Size.Width + x] = GetColorAt(x, y);
			});
			UpdateColors();
			isUpdating = false;
		}

		private bool isUpdating;

		private Rectangle area;

		private Color GetColorAt(int x, int y)
		{
			var real = area.Left + x * area.Width / Size.Width;
			var imaginary = (area.Top + y * area.Height / Size.Height) / Size.AspectRatio;
			return smoothColors[fractal.GetColorIndexFromIterationsNeeded(real, imaginary)];
		}
		
		private void UpdateColors()
		{
			material.DiffuseMap.Fill(Colors);
		}

		private void CreateCommands(Window window)
		{
			new Command(Command.Exit, window.CloseAfterFrame);
			new Command(Command.Zoom, zoom => UpdateFractalRegion(area.Reduce(area.Size * zoom)));
			//ncrunch: no coverage start
			new Command(Command.Drag, (startPos, currentPos, end) =>
			{
				if (lastPos == Vector2D.Unused || (lastPos - currentPos).Length > 0.1f)
					lastPos = startPos;
				UpdateFractalRegion(area.Move(area.Width * (lastPos - currentPos)));
				lastPos = currentPos;
				if (end)
					lastPos = Vector2D.Unused;
			});
			new Command(Command.MoveDirectly, position =>
			{
				UpdateFractalRegion(area.Move(area.Width * position / 10.0f));
			}); //ncrunch: no coverage end
		}

		private Vector2D lastPos = Vector2D.Unused;
	}
}