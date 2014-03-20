using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using Color = DeltaEngine.Datatypes.Color;
using Orientation = DeltaEngine.Core.Orientation;
using Size = DeltaEngine.Datatypes.Size;
using SystemSize = System.Drawing.Size;
using SystemPoint = System.Drawing.Point;

namespace DeltaEngine.Platforms.Windows
{
	/// <summary>
	/// Windows Forms window implementation for the Delta Engine to run applications in it.
	/// </summary>
	public class FormsWindow : Window
	{
		public FormsWindow(Settings settings)
			: this(new NativeMessageForm())
		{
			var form = panel as Form;
			if (settings.StartInFullscreen)
			{
				IsFullscreen = settings.StartInFullscreen;
				form.FormBorderStyle = FormBorderStyle.None;
				form.TopMost = true;
				form.StartPosition = FormStartPosition.Manual;
				form.DesktopLocation = new SystemPoint(0, 0);
			}
			else
			{
				form.FormBorderStyle = FormBorderStyle.Sizable;
				form.StartPosition = FormStartPosition.CenterScreen;
			}
			form.ClientSize = new SystemSize((int)settings.Resolution.Width,
				(int)settings.Resolution.Height);
			form.MinimumSize = new SystemSize(1, 1);
			form.Text = StackTraceExtensions.GetEntryName();
			BackgroundColor = Color.Black;
			Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
			if (appIcon != null)
				form.Icon = appIcon;
			form.SizeChanged += OnSizeChanged;
			form.Show();
		}

		protected FormsWindow(Control panel)
		{
			this.panel = panel;
			BackgroundColor = Color.Black;
		}

		protected readonly Control panel;

		private sealed class NativeMessageForm : Form
		{
			protected override void WndProc(ref Message m)
			{
				if (NativeEvent != null)
					NativeEvent(ref m);
				base.WndProc(ref m);
			}

			public event NativeMessageDelegate NativeEvent;
		}

		public event NativeMessageDelegate NativeEvent
		{
			add
			{
				var nativeMessageForm = panel as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent += value;
			}
			remove
			{
				var nativeMessageForm = panel as NativeMessageForm;
				if (nativeMessageForm != null)
					nativeMessageForm.NativeEvent -= value;
			}
		}

		public delegate void NativeMessageDelegate(ref Message m);

		protected void OnSizeChanged(object sender, EventArgs e)
		{
			if (ViewportSizeChanged != null)
				ViewportSizeChanged(ViewportPixelSize);
			Orientation = ViewportPixelSize.Width > ViewportPixelSize.Height
				? Orientation.Landscape : Orientation.Portrait;
			if (OrientationChanged != null)
				OrientationChanged(Orientation);
		}

		public Orientation Orientation { get; private set; }
		public event Action<Size> ViewportSizeChanged;
		public event Action<Orientation> OrientationChanged;

		public string Title
		{
			get { return panel.Text; }
			set { panel.Text = value; }
		}

		public bool IsVisible
		{
			get { return panel.Visible; }
		}

		public IntPtr Handle
		{
			get { return panel.IsDisposed ? IntPtr.Zero : panel.Handle; }
		}

		public Size ViewportPixelSize
		{
			get { return new Size(panel.ClientSize.Width, panel.ClientSize.Height); }
			set
			{
				if (ViewportPixelSize == value)
					return;
				SetResolution(value + (TotalPixelSize - ViewportPixelSize));
			}
		}

		public Vector2D ViewportPixelPosition
		{
			get
			{
				var screenPoint = panel.PointToScreen(new Point(0, 0));
				return new Vector2D(screenPoint.X, screenPoint.Y);
			}
		}

		public Size TotalPixelSize
		{
			get { return new Size(panel.Width, panel.Height); }
		}

		public Vector2D PixelPosition
		{
			get { return new Vector2D(panel.Location.X, panel.Location.Y); }
			set { panel.Location = new SystemPoint((int)value.X, (int)value.Y); }
		}

		public Color BackgroundColor
		{
			get { return color; }
			set
			{
				color = value;
				if (color.A > 0)
					panel.BackColor = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
			}
		}
		private Color color;

		public virtual void SetFullscreen(Size setFullscreenViewportSize)
		{
			IsFullscreen = true;
			rememberedWindowedSize = new Size(panel.Size.Width, panel.Size.Height);
			var form = panel as Form;
			if (form != null)
			{
				form.TopMost = true;
				form.StartPosition = FormStartPosition.Manual;
				form.DesktopLocation = new Point(0, 0);
				form.WindowState = FormWindowState.Maximized;
				form.FormBorderStyle = FormBorderStyle.None;
				Cursor.Hide();
				SetFullscreenNative(form.Handle, setFullscreenViewportSize);
			}
			SetResolution(setFullscreenViewportSize);
		}

		private Size rememberedWindowedSize;

		private static void SetFullscreenNative(IntPtr hwnd, Size size)
		{
			SetWindowPos(hwnd, IntPtr.Zero, 0, 0, (int)size.Width, (int)size.Height, 64);
		}

		[DllImport("user32.dll")]
		private static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int left,
			int top, int width, int height, uint flags);

		protected virtual void SetResolution(Size displaySize)
		{
			ResizeCentered(displaySize);
			if (FullscreenChanged != null)
				FullscreenChanged(displaySize, IsFullscreen);
		}

		protected virtual void ResizeCentered(Size newSizeInPixels)
		{
			var xOffset = (int)((panel.Width - newSizeInPixels.Width) / 2.0f);
			var yOffset = (int)((panel.Height - newSizeInPixels.Height) / 2.0f);
			panel.Location = new SystemPoint(panel.Location.X + xOffset, panel.Location.Y + yOffset);
			panel.Size = new SystemSize((int)newSizeInPixels.Width, (int)newSizeInPixels.Height);
		}

		public void SetWindowed()
		{
			IsFullscreen = false;
			var form = panel as Form;
			if (form != null)
			{
				form.WindowState = FormWindowState.Normal;
				form.FormBorderStyle = FormBorderStyle.Sizable;
				Cursor.Show();
			}
			SetResolution(rememberedWindowedSize);
		}

		public bool IsFullscreen { get; private set; }
		public event Action<Size, bool> FullscreenChanged;

		public virtual bool IsClosing
		{
			get { return panel.Disposing || panel.IsDisposed || rememberToClose; }
		}
		private bool rememberToClose;

		public bool ShowCursor
		{
			get { return !remDisabledShowCursor; }
			set
			{
				if (remDisabledShowCursor != value)
					return;
				remDisabledShowCursor = !value;
				if (remDisabledShowCursor)
					Cursor.Hide();
				else
					Cursor.Show();
				currentCursor = null;
			}
		}

		private bool remDisabledShowCursor;
		private Cursor currentCursor;

		public bool IsWindowsFormAndNotJustAPanel { get { return panel is Form; } }

		public void SetCursorIcon(string iconFilePath)
		{
			ShowCursor = true;
			currentCursor = string.IsNullOrEmpty(iconFilePath)
				? Cursors.Default : new Cursor(iconFilePath);
			Cursor.Current = currentCursor;
		}

		public virtual string ShowMessageBox(string caption, string message, string[] buttons)
		{
			if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				throw new Exception(caption + " " + message);
			var buttonCombination = MessageBoxButtons.OK;
			if (buttons.Contains("Cancel"))
				buttonCombination = MessageBoxButtons.OKCancel;
			if (buttons.Contains("Ignore") || buttons.Contains("Abort") || buttons.Contains("Retry"))
				buttonCombination = MessageBoxButtons.AbortRetryIgnore;
			if (buttons.Contains("Yes") || buttons.Contains("No"))
				buttonCombination = MessageBoxButtons.YesNo;
			return MessageBox.Show(message, Title + " " + caption, buttonCombination).ToString();
		}

		/// <summary>
		/// Clipboard.SetText must be executed on a STA thread, which we are not, create extra thread!
		/// </summary>
		public void CopyTextToClipboard(string text)
		{
			var staThread = new Thread(() => SetClipboardText(text));
			staThread.SetApartmentState(ApartmentState.STA);
			staThread.Start();
		}

		private static void SetClipboardText(string text)
		{
			try
			{
				TrySetClipboardText(text);
			}
			catch (Exception)
			{
				Logger.Warning("Failed to set clipboard text: " + text);
			}
		}

		private static void TrySetClipboardText(string text)
		{
			Clipboard.SetText(text, TextDataFormat.Text);
		}

		public virtual void Present()
		{
			Application.DoEvents();
			if (currentCursor != null)
				Cursor.Current = currentCursor;
		}

		public void CloseAfterFrame()
		{
			rememberToClose = true;
		}

		public virtual void Dispose()
		{
			var form = panel as Form;
			if (form != null)
				form.Close();
			panel.Dispose();
		}
	}
}