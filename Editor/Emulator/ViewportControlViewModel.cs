using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.Emulator
{
	public class ViewportControlViewModel : ViewModelBase
	{
		public ViewportControlViewModel()
		{
			Tools = new List<ToolboxEntry>();
			selectedNameList = new List<string>();
			UIImagesInList = new ObservableCollection<string>();
			Messenger.Default.Register<string>(this, "AddToHierachyList", AddToHierachyList);
			Messenger.Default.Register<List<string>>(this, "SetSelectedName", SetSelectedName);
			Messenger.Default.Register<int>(this, "SetSelectedIndex", SetSelectedIndex);
			Messenger.Default.Register<string>(this, "ChangeSelectedControlName",
				ChangeSelectedControlName);
			Messenger.Default.Register<string>(this, "DeleteSelectedContent", DeleteSelectedContent);
			Messenger.Default.Register<string>(this, "ClearScene", ClearScene);
			SetupToolbox();
		}

		private void AddToHierachyList(string newControl)
		{
			UIImagesInList.Add(newControl);
		}

		private void SetSelectedName(List<string> selectedName)
		{
			selectedNameList = selectedName;
		}

		private void SetSelectedIndex(int selectedIndex)
		{
			IndexOfSelectedNameInList = selectedIndex;
		}

		private void ChangeSelectedControlName(string newName)
		{
			UIImagesInList[IndexOfSelectedNameInList] = newName;
			SelectedName = newName;
		}

		private void DeleteSelectedContent(string obj)
		{
			var selectedControlNameList = new List<string>();
			foreach (var name in selectedNameList)
			{
				selectedControlNameList.Add(name);
				UIImagesInList.Remove(name);
			}
			Messenger.Default.Send(selectedControlNameList, "DeleteSelectedContentListFromWpf");
			selectedNameList.Clear();
			RaisePropertyChanged("SelectedNameInList");
		}

		private void ClearScene(string obj)
		{
			UIImagesInList.Clear();
			RaisePropertyChanged("SelectedNameInList");
		}

		public string SelectedName
		{
			get { return selectedName; }
			set
			{
				if (value == null)
					return;
				if (!Keyboard.IsKeyDown(Key.LeftCtrl))
					selectedNameList.Clear();
				selectedName = value;
				selectedNameList.Add(selectedName);
				IndexOfSelectedNameInList = UIImagesInList.IndexOf(selectedName);
				Messenger.Default.Send(selectedName, "SetSelectedNameFromHierachy");
				RaisePropertyChanged("SelectedNameInList");
			}
		}

		private List<string> selectedNameList;

		private string selectedName;

		public int IndexOfSelectedNameInList
		{
			get { return indexOfSelectedNameInList; }
			set
			{
				indexOfSelectedNameInList = value;
				Messenger.Default.Send(indexOfSelectedNameInList, "SetSelectedIndexFromHierachy");
				RaisePropertyChanged("IndexOfSelectedNameInList");
			}
		}

		private int indexOfSelectedNameInList;

		public List<ToolboxEntry> Tools { get; private set; }

		public ObservableCollection<string> UIImagesInList
		{
			get { return uiImagesInList; }
			set
			{
				uiImagesInList = value;
				RaisePropertyChanged("UIImagesInList");
			}
		}

		private ObservableCollection<string> uiImagesInList { get; set; }

		private void SetupToolbox()
		{
			var namesAndPaths = new UIToolNamesAndPaths();
			int i = 0;
			foreach (string tool in namesAndPaths.GetNames())
				if (controls.Contains(tool))
					Tools.Add(new ToolboxEntry(tool, namesAndPaths.GetImagePath(tool), toolTips[i++]));
		}

		private readonly string[] controls = { "Button", "Image", "Label", "Slider" };
		private readonly string[] toolTips = { "Create Button", "Create Image", "Create Label",
			"Create Slider" };
	}
}