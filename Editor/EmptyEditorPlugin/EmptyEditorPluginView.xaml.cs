using System.Collections.Generic;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.EmptyEditorPlugin
{
	/// <summary>
	/// Code-behind of EmptyEditorPluginView.xaml
	/// </summary>
	public partial class EmptyEditorPluginView : EditorPluginView
	{
		public EmptyEditorPluginView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			if (!(DataContext is EmptyEditorPluginViewModel))
				DataContext = new EmptyEditorPluginViewModel();
		}

		public string ShortName
		{
			get { return "EmptyEditorPlugin"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/New.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Activate() {}

		public void Deactivate() {}

		public void Send(IList<string> arguments) {}
	}
}