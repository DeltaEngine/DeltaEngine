using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using DeltaEngine.Commands;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using GalaSoft.MvvmLight.Messaging;
using Color = System.Drawing.Color;
using Cursors = System.Windows.Input.Cursors;
using Mouse = System.Windows.Input.Mouse;
using MouseButton = DeltaEngine.Input.MouseButton;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Engine Viewport for all of the Editor plugins
	/// </summary>
	public partial class ViewportControl : EditorPluginView
	{
		//ncrunch: no coverage start
		public ViewportControl()
		{
			InitializeComponent();
		}

		public void Init(Service setService)
		{
			service = setService;
			DataContext = new ViewportControlViewModel();
			CreateViewportCommands(service);
		}

		private Service service;

		private static void CreateViewportCommands(Service service)
		{
			var dragTrigger = new MouseDragTrigger(MouseButton.Middle);
			var zoomTrigger = new MouseZoomTrigger();
			new Command(service.Viewport.OnViewportPanning).Add(dragTrigger);
			new Command(service.Viewport.OnViewPortZooming).Add(zoomTrigger);
		}

		public void Activate() {}

		public void Deactivate() {}

		public void ShowToolboxPane(bool isVisible)
		{
			ToolPane.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
		}

		public void ApplyEmulator()
		{
			SetupScreen();
			SetupDeviceFrame();
			SetupEmulator();
			SetupHost();
		}

		private void SetupScreen()
		{
			Screen = new Panel();
			Screen.MouseLeave += ScreenOnMouseLeave;
			Screen.MouseEnter += ScreenOnMouseEnter;
			Screen.Dock = DockStyle.Fill;
		}

		public Panel Screen { get; private set; }

		private void ScreenOnMouseLeave(object sender, EventArgs eventArgs)
		{
			service.Viewport.Window.SetCursorIcon();
			Messenger.Default.Send("SetDefaultCursor", "SetDefaultCursor");
		}

		private static void ScreenOnMouseEnter(object sender, EventArgs e)
		{
			Messenger.Default.Send("AllowChangeOfCursor", "AllowChangeOfCursor");
		}

		private void SetupDeviceFrame()
		{
			deviceFrame = new PictureBox();
			deviceFrame.BackColor = Color.Transparent;
			deviceFrame.BackgroundImageLayout = ImageLayout.Center;
			deviceFrame.Location = new Point(0, 0);
		}

		private PictureBox deviceFrame;

		private void SetupEmulator()
		{
			emulatorPanel = new Panel();
			emulatorPanel.Controls.Add(Screen);
			emulatorPanel.Controls.Add(deviceFrame);
			emulatorPanel.Dock = DockStyle.Fill;
		}

		private Panel emulatorPanel;
		
		private void SetupHost()
		{
			ViewportHost.Child = emulatorPanel;
			((ISupportInitialize)(deviceFrame)).EndInit();
			emulatorPanel.ResumeLayout(false);
		}

		private void CreateAndDragNewSceneControl(object sender, MouseEventArgs e)
		{
			if (UIToolbox.SelectedItem == null)
				return;
			var item = UIToolbox.SelectedItem as ToolboxEntry;
			if (item.ShortName == "Image")
				DragImage(e);
			if (item.ShortName == "Button")
				DragButton(e);
			if (item.ShortName == "Label")
				DragLabel(e);
			if (item.ShortName == "Slider")
				DragSlider(e);
			isClicking = false;
		}

		private void DragImage(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingImage");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingImage");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private bool isDragging;

		private void DragButton(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingButton");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingButton");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void DragLabel(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingLabel");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingLabel");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void DragSlider(MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Messenger.Default.Send(true, "SetDraggingSlider");
				isDragging = true;
				Mouse.OverrideCursor = Cursors.Hand;
			}
			if (e.LeftButton != MouseButtonState.Pressed)
			{
				Messenger.Default.Send(false, "SetDraggingSlider");
				isDragging = false;
				Mouse.OverrideCursor = Cursors.Arrow;
			}
		}

		private void ClickingOnButton(object sender, MouseButtonEventArgs e)
		{
			isClicking = true;
		}

		private bool isClicking;

		private void CreateNewSceneControl(object sender, MouseButtonEventArgs mouseButtonEventArgs)
		{
			if (UIToolbox.SelectedItem == null || isClicking == false)
				return;
			var item = UIToolbox.SelectedItem as ToolboxEntry;
			PlaceCenteredControl(item.ShortName);
			UIToolbox.SelectedItem = null;
			isClicking = false;
		}

		private static void PlaceCenteredControl(string newControl)
		{
			Messenger.Default.Send(newControl, "SetCenteredControl");
			Mouse.OverrideCursor = Cursors.Hand;
		}

		public string ShortName
		{
			get { return "Viewport"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Viewport.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		public void Send(IList<string> arguments) {}
		
		public void UpdateEmulator(Emulator emulator)
		{
			Screen.Dock = emulator.IsDefault ? DockStyle.Fill : DockStyle.None;
			if (emulator.IsDefault)
			{
				deviceFrame.SizeMode = PictureBoxSizeMode.Normal;
				deviceFrame.Size = new Size();
				deviceFrame.Location = new Point(0, 0);
				if (DeviceChanged != null)
					DeviceChanged();
				return;
			}
			string scalePercentage = emulator.Scale.Substring(0, emulator.Scale.Length - 1);
			scale = Int32.Parse(scalePercentage) * 0.01f;
			SetEmulatorImageAndSize(emulator);
			SetScreenLocationAndSize(emulator);
			if (DeviceChanged != null)
				DeviceChanged();
		}

		private float scale = 1.0f;
		public event Action DeviceChanged;

		private void SetEmulatorImageAndSize(Emulator emulator)
		{
			deviceFrame.SizeMode = PictureBoxSizeMode.StretchImage;
			deviceFrame.Image = Image.FromStream(GetImageFilestream(emulator.ImageResourceName));
			if (emulator.Orientation.IsLandscape())
				deviceFrame.Size = new Size((int)(deviceFrame.Image.Size.Width * scale),
					(int)(deviceFrame.Image.Size.Height * scale));
			else
				deviceFrame.Size = new Size((int)(deviceFrame.Image.Size.Height * scale),
					(int)(deviceFrame.Image.Size.Width * scale));
		}

		private static Stream GetImageFilestream(string imageName)
		{
			var imageFilename = "Images.Emulators." + imageName + ".png";
			return EmbeddedResourcesLoader.GetEmbeddedResourceStream(imageFilename);
		}

		private void SetScreenLocationAndSize(Emulator emulator)
		{
			if (emulator.Orientation.IsLandscape())
				SetLandscapeScreenLocationAndSize(emulator);
			else
				SetPortraitScreenLocationAndSize(emulator);
		}

		private void SetLandscapeScreenLocationAndSize(Emulator emulator)
		{
			Screen.Location = new Point((int)(emulator.ScreenPoint.X * scale),
				(int)(emulator.ScreenPoint.Y * scale));
			Screen.Size = new Size((int)(emulator.ScreenSize.Width * scale),
				(int)(emulator.ScreenSize.Height * scale));
		}

		private void SetPortraitScreenLocationAndSize(Emulator emulator)
		{
			deviceFrame.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
			Screen.Location = new Point((int)(emulator.ScreenPoint.Y * scale),
				(int)(emulator.ScreenPoint.X * scale));
			Screen.Size = new Size((int)(emulator.ScreenSize.Height * scale),
				(int)(emulator.ScreenSize.Width * scale));
		}
	}
}