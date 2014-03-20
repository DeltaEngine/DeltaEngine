using System.Collections.Generic;
using System.Windows.Controls;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator
{
	/// <summary>
	/// Change emulation of devices for the Engine Viewport
	/// </summary>
	public partial class EmulatorControl : EditorPluginView
	{
		public EmulatorControl()
		{
			EmulatorViewModel = new EmulatorViewModel();
			InitializeComponent();
		}

		public EmulatorViewModel EmulatorViewModel { get; private set; }

		public void Init(Service service)
		{
			DataContext = EmulatorViewModel;
		}

		public void Activate() {}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Emulator"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Emulator.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		private void OnDeviceSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EmulatorViewModel.ChangeEmulator();
		}

		private void OnOrientationSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EmulatorViewModel.ChangeEmulator();
		}

		private void OnScalingSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EmulatorViewModel.ChangeEmulator();
		}
	}
}