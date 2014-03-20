using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Shapes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Editor.LevelEditor
{
	public class CameraSliders
	{
		public CameraSliders(LevelDebugRenderer renderer)
		{
			this.renderer = renderer;
		}

		private readonly LevelDebugRenderer renderer;

		public virtual void CreateSliders()
		{
			horizontalSlider = new Slider(new Theme(), new Rectangle(0, 0, 1, 1));
			horizontalSlider.MaxValue = 0;
			horizontalSlider.Value = 0;
			verticalSlider = new Slider(new Theme(), new Rectangle(0, 0, 1, 1)) { Rotation = 90 };
			verticalSlider.MaxValue = 0;
			verticalSlider.Value = 0;
			horizontalSlider.ValueChanged += value =>
			{
				SetCameraPosition();
				CalculateSliderPositionAndScale();
			};
			verticalSlider.ValueChanged += value =>
			{
				SetCameraPosition();
				CalculateSliderPositionAndScale();
			};
			CalculateSliderPositionAndScale();
			Hide();
		}

		public Slider horizontalSlider;
		public Slider verticalSlider;

		public void SetCameraPosition()
		{
			var gridBorder = renderer.GetBorder();
			var screen = (Camera2DScreenSpace)ScreenSpace.Current;
			var positionX = gridBorder.TopLeft.X +
				horizontalSlider.Value * (gridBorder.Width / horizontalSlider.MaxValue);
			var positionY = gridBorder.TopLeft.Y +
				verticalSlider.Value * (gridBorder.Height / verticalSlider.MaxValue);
			if (horizontalSlider.MaxValue > 0 && verticalSlider.MaxValue > 0)
				screen.LookAt = new Vector2D(positionX, positionY);
		}

		public void CalculateSliderPositionAndScale()
		{
			var gridBorder = renderer.GetBorder();
			SetScreenBorder();
			horizontalSlider.MaxValue = (int)(gridBorder.Width * 100 - screenBorder.Width * 100);
			verticalSlider.MaxValue = (int)(gridBorder.Height * 50 - screenBorder.Height * 50);
			if (horizontalSlider.MaxValue < 0)
				horizontalSlider.MaxValue = 0;
			if (verticalSlider.MaxValue < 0)
				verticalSlider.MaxValue = 0;
			DrawSliders();
		}

		private const float ScaleFactor = 0.03f;

		private Rectangle screenBorder;

		private void SetScreenBorder()
		{
			var screen = ScreenSpace.Current.Viewport;
			screenBorder = new Rectangle(screen.TopLeft, (Size)(screen.BottomRight - screen.TopLeft));
		}

		private void DrawSliders()
		{
			horizontalSlider.DrawArea = Rectangle.FromCenter(ScreenSpace.Current.Viewport.Center.X,
				ScreenSpace.Current.Viewport.Bottom, ScreenSpace.Current.Viewport.Width,
				ScreenSpace.Current.Viewport.Height * ScaleFactor);
			verticalSlider.DrawArea = Rectangle.FromCenter(ScreenSpace.Current.Viewport.Right,
				ScreenSpace.Current.Viewport.Center.Y, ScreenSpace.Current.Viewport.Height,
				ScreenSpace.Current.Viewport.Height * ScaleFactor);
		}

		public void Hide()
		{
			horizontalSlider.IsVisible = false;
			verticalSlider.IsVisible = false;
		}

		public void Show()
		{
			horizontalSlider.IsVisible = true;
			verticalSlider.IsVisible = true;
		}
	}
}