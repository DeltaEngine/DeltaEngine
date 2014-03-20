using System;
using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.MaterialEditor
{
	/// <summary>
	/// Interaction logic for MaterialEditorView.xaml
	/// </summary>
	public partial class MaterialEditorView : EditorPluginView
	{
		//ncrunch: no coverage start
		public MaterialEditorView()
		{
			InitializeComponent();
		}

		public void Init(Service setService)
		{
			service = setService;
			current = new MaterialEditorViewModel(service);
			service.ProjectChanged += ChangeProject;
			service.ContentUpdated += (type, name) =>
			{
				Action updateAction = () => { current.RefreshOnAddedContent(type, name); };
				Dispatcher.Invoke(updateAction);
			};
			service.ContentDeleted += s => Dispatcher.Invoke(new Action(current.RefreshOnContentChange));
			DataContext = current;
		}

		private Service service;
		private MaterialEditorViewModel current;

		private void ChangeProject()
		{
			Dispatcher.Invoke(new Action(current.ResetOnProjectChange));
		}

		public void Activate()
		{
			current.Activate();
		}

		public void Deactivate() {}

		public string ShortName
		{
			get { return "Material Editor"; }
		}

		public string Icon
		{
			get { return "Images/Plugins/Material.png"; }
		}

		public bool RequiresLargePane
		{
			get { return false; }
		}

		public void Send(IList<string> arguments) {}

		private void SaveMaterial(object sender, RoutedEventArgs e)
		{
			current.Save();
		}

		private void LoadMaterial(object sender, RoutedEventArgs e)
		{
			current.LoadMaterial();
		}

		private void OnDropDownClosed(object sender, EventArgs e)
		{
			current.UpdateIfCanSave();
		}
	}
}