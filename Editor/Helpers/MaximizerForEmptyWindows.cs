using System.Windows;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.Helpers
{
	internal class MaximizerForEmptyWindows
	{
		public MaximizerForEmptyWindows(WpfWindow window)
		{
			this.window = window;
		}

		private readonly WpfWindow window;

		public void ToggleMaximize(bool moveWindowToTop, EditorViewModel viewModel)
		{
			if (isMaximized)
				RestoreWindowLocation(moveWindowToTop);
			else
				MaximizeWindow();
			viewModel.UpdateBorderThicknessOfChromeStyle();
		}

		public bool isMaximized;

		private void RestoreWindowLocation(bool moveWindowToTop)
		{
			isMaximized = false;
			window.ResizeMode = ResizeMode.CanResize;
			window.Top = currentWindowBounds.Top;
			if (moveWindowToTop)
				window.Top = 0;
			window.Left = currentWindowBounds.Left;
			window.Width = currentWindowBounds.Width;
			window.Height = currentWindowBounds.Height;
		}

		private Rect currentWindowBounds;

		public void MaximizeWindow()
		{
			window.WindowState = WindowState.Normal;
			isMaximized = true;
			window.ResizeMode = ResizeMode.NoResize;
			SaveWindowLocation();
			SetWindowMaximized();
		}

		private void SaveWindowLocation()
		{
			currentWindowBounds = new Rect(window.Left, window.Top, window.Width, window.Height);
			var screenWorkAreas = screens.GetDisplayWorkAreas();
			foreach (var screen in screenWorkAreas)
				if (window.Left >= screen.left && window.Left < screen.right && window.Top >= screen.top &&
					window.Top < screen.bottom && currentWindowBounds.Width == screen.right - screen.left)
				{
					currentWindowBounds.Width = (screen.right - screen.left) * 0.75f;
					currentWindowBounds.Height = (screen.bottom - screen.top) * 0.75f;
					break;
				}
		}

		private void SetWindowMaximized()
		{
			var screenWorkAreas = screens.GetDisplayWorkAreas();
			foreach (var screen in screenWorkAreas)
				if (window.Left >= screen.left && window.Left < screen.right && window.Top >= screen.top &&
					window.Top < screen.bottom)
				{
					window.Top = screen.top;
					window.Left = screen.left;
					window.Width = screen.right - screen.left;
					window.Height = screen.bottom - screen.top;
				}
		}

		private readonly NativeScreens screens = new NativeScreens();

		public void BringWindowToForeground()
		{
			window.Activate();
			if (window.WindowState == WindowState.Minimized)
				window.WindowState = WindowState.Normal;
		}
	}
}