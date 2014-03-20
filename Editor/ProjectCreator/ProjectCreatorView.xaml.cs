using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.ProjectCreator
{
	/// <summary>
	/// Enables users to create a Delta Engine project and select framework (Xna, OpenGL, DirectX)
	/// </summary>
	public partial class ProjectCreatorView : EditorPluginView
	{
		public ProjectCreatorView()
		{
			InitializeComponent();
		}

		public ProjectCreatorView(ProjectCreatorViewModel viewModel)
		{
			DataContext = this.viewModel = viewModel;
		}

		private ProjectCreatorViewModel viewModel;

		public void Init(Service service)
		{
			DataContext = viewModel = new ProjectCreatorViewModel(service);
		}

		public void Activate() {}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Project Creator"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/ProjectCreator.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		public void Send(IList<string> arguments) {}

		private void OnBrowseUserProjectsClicked(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			if (dialog.ShowDialog() == DialogResult.OK)
				viewModel.Location = dialog.SelectedPath;
		}
	}
}