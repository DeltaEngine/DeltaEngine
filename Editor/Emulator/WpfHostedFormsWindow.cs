using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.ScreenSpaces;
using Application = System.Windows.Application;
using DeltaSize = DeltaEngine.Datatypes.Size;
using MessageBox = System.Windows.MessageBox;
using MessageBoxButton = System.Windows.MessageBoxButton;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Used for rendering within the Editor
	/// </summary>
	public class WpfHostedFormsWindow : FormsWindow
	{
		//ncrunch: no coverage start
		public WpfHostedFormsWindow(ViewportControl viewportControl, Window window)
			: base(GetViewportControlPanel(viewportControl))
		{
			dispatcher = viewportControl.ViewportHost.Dispatcher;
			this.viewportControl = viewportControl;
			this.window = window;
			viewportControl.ViewportHost.SizeChanged += OnHostControlSizeChanged;
			viewportControl.Background = new SolidColorBrush(Colors.Black);
			viewportControl.DeviceChanged += ChangeDevice;
		}

		private static Panel GetViewportControlPanel(ViewportControl viewportControl)
		{
			viewportControl.ApplyEmulator();
			return viewportControl.Screen;
		}

		private readonly Dispatcher dispatcher;
		private readonly ViewportControl viewportControl;
		private readonly Window window;

		private void OnHostControlSizeChanged(object s, SizeChangedEventArgs e)
		{
			OnSizeChanged(null, EventArgs.Empty);
		}

		private void ChangeDevice()
		{
			float width = ViewportPixelSize.Width;
			float height = ViewportPixelSize.Height;
			ViewportPixelSize = new DeltaSize(MathExtensions.Max(width, height),
				MathExtensions.Max(width, height));
			ScreenSpace.Current = new Camera2DScreenSpace(this);
			ViewportPixelSize = new DeltaSize(width, height);
		}

		public override void Dispose()
		{
			base.Dispose();
			dispatcher.InvokeShutdown();
			if (Application.Current != null)
				Application.Current.Shutdown();
		}

		public override void Present()
		{
			base.Present();
			ForceRescaleOnFirstFrameToFixBlackBlocksNotRefreshingWpfWindow();
		}

		public override void SetFullscreen(DeltaSize setFullscreenViewportSize) {}

		private void ForceRescaleOnFirstFrameToFixBlackBlocksNotRefreshingWpfWindow()
		{
			if (forceRescaleOnce)
				return;
			forceRescaleOnce = true;
			window.Height = window.Height + 1;
			viewportControl.ViewportHost.Width = double.NaN;
			viewportControl.ViewportHost.Height = double.NaN;
		}

		private bool forceRescaleOnce;

		public override string ShowMessageBox(string caption, string message, string[] buttons)
		{
			if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				throw new Exception(caption + " " + message);
			var buttonCombination = MessageBoxButton.OK;
			if (buttons.Contains("Cancel"))
				buttonCombination = MessageBoxButton.OKCancel;
			if (buttons.Contains("Ignore") || buttons.Contains("Abort") || buttons.Contains("Retry"))
				buttonCombination = MessageBoxButton.YesNoCancel;
			if (buttons.Contains("Yes") || buttons.Contains("No"))
				buttonCombination = MessageBoxButton.YesNo;
			var title = Title + " " + caption;
			return (string)window.Dispatcher.Invoke(
				new Func<string>(
					() => MessageBox.Show(window, message, title, buttonCombination).ToString()));
		}
	}
}