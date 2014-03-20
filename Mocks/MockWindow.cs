using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Mock a window for unit tests.
	/// </summary>
	public class MockWindow : Window
	{
		public MockWindow()
		{
			Title = "MockWindow";
			currentSize = Settings.DefaultResolution;
		}

		public void Present() {}
		public void CloseAfterFrame() {}

		public string Title { get; set; }

		public bool IsVisible
		{
			get { return true; }
		}

		public IntPtr Handle
		{
			get { return IntPtr.Zero; }
		}

		public Size ViewportPixelSize
		{
			get { return currentSize; }
			set
			{
				if (currentSize == value)
					return;
				currentSize = value;
				if (ViewportSizeChanged != null)
					ViewportSizeChanged(currentSize);
				if (OrientationChanged != null)
					OrientationChanged(Orientation);
			}
		}

		public Vector2D ViewportPixelPosition
		{
			get { return Vector2D.Zero; } //ncrunch: no coverage
		}

		public Orientation Orientation
		{
			get { return Orientation.Landscape; }
		}

		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;
		public event Action<Size, bool> FullscreenChanged;

		public Size TotalPixelSize
		{
			get { return currentSize; }
			set
			{
				currentSize = value;
				if (ViewportSizeChanged == null)
					return;
				ViewportSizeChanged(currentSize);
			}
		}

		private Size currentSize;
		public Vector2D PixelPosition { get; set; }
		public Color BackgroundColor { get; set; }
		public bool IsFullscreen { get; private set; }

		public void SetFullscreen(Size displaySize)
		{
			IsFullscreen = true;
			rememberSizeBeforeFullscreen = TotalPixelSize;
			TotalPixelSize = displaySize;
			if (FullscreenChanged != null)
				FullscreenChanged(TotalPixelSize, true);
		}

		private Size rememberSizeBeforeFullscreen;

		public void SetWindowed()
		{
			IsFullscreen = false;
			TotalPixelSize = rememberSizeBeforeFullscreen;
		}

		public bool IsClosing
		{
			get { return true; }
		}

		//ncrunch: no coverage start
		public bool ShowCursor { get; set; }
		public bool IsWindowsFormAndNotJustAPanel { get { return true; } }
		public void SetCursorIcon(string iconFilePath) { }

		public string ShowMessageBox(string title, string message, string[] buttons)
		{
			return "OK";
		}

		public void CopyTextToClipboard(string text)
		{
			Logger.Info("Copied to mock clipboard: " + text);
		}

		public void Dispose() {}
	}
}