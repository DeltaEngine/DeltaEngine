using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Core
{
	/// <summary>
	/// Window form the application is running in. In Windows this is done with Windows Forms or WPF.
	/// </summary>
	public interface Window : IDisposable
	{
		string Title { get; set; }
		bool IsVisible { get; }
		IntPtr Handle { get; }
		Size ViewportPixelSize { get; set; }
		Vector2D ViewportPixelPosition { get; }
		Orientation Orientation { get; }
		event Action<Size> ViewportSizeChanged;
		event Action<Orientation> OrientationChanged;
		event Action<Size, bool> FullscreenChanged;
		Size TotalPixelSize { get; }
		Vector2D PixelPosition { get; set; }
		Color BackgroundColor { get; set; }
		bool IsFullscreen { get; }
		void SetFullscreen(Size displaySize);
		void SetWindowed();
		void CloseAfterFrame();
		void Present();
		bool IsClosing { get; }
		bool ShowCursor { get; set; }
		bool IsWindowsFormAndNotJustAPanel { get; }
		void SetCursorIcon(string iconFilePath = null);
		string ShowMessageBox(string title, string message, string[] buttons);
		void CopyTextToClipboard(string text);
	}
}