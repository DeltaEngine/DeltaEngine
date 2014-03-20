using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.InputEditor
{
	/// <summary>
	/// editor for the input commands
	/// </summary>
	public partial class InputEditorView : EditorPluginView
	{
		//ncrunch: no coverage start
		public InputEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			DataContext = new InputEditorViewModel(service);
		}

		public void Activate() {}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Input Commands"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Input.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send("SaveCommands", "SaveCommands");
		}

		public void Send(IList<string> arguments) {}
	}
}