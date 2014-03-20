using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorViewModel : ViewModelBase
	{
		public UIEditorViewModel(Service service)
		{
			this.service = service;
			Messenger.Default.Send("ClearScene", "ClearScene");
			UiEditorScene = new UIEditorScene(service);
			SetMouseCommands("");
			CreateCenteredControl("Button");
			UIName = "MyScene";
			UpdateIfCanSaveScene();
			SetMessengers();
		}

		public readonly Service service;
		public UIEditorScene UiEditorScene;

		private void SetMouseCommands(string obj)
		{
			var leftClickTriggerReleasing = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			leftClickTriggerReleasing.AddTag("temporary");
			var findEntityCommand = new Command(FindEntity2DOnPosition).Add(leftClickTriggerReleasing);
			findEntityCommand.AddTag("temporary");
			var releaseMiddleMouse = new MouseButtonTrigger(MouseButton.Left, State.Releasing);
			releaseMiddleMouse.AddTag("temporary");
			var setReleasingCommand =
				new Command(position => AddControlToScene(position)).Add(releaseMiddleMouse);
			setReleasingCommand.AddTag("temporary");
			UiEditorScene.SetMousePosition();
		}

		public int NewGridWidth
		{
			set { UiEditorScene.NewGridWidth = value; }
		}

		public int NewGridHeight
		{
			set { UiEditorScene.NewGridHeight = value; }
		}

		private void FindEntity2DOnPosition(Vector2D position)
		{
			SelectedEntity2DList = UiEditorScene.FindEntity2DOnPosition(position);
			RaiseAllProperties();
		}

		public List<Entity2D> SelectedEntity2DList
		{
			get { return UiEditorScene.SelectedEntity2DList; }
			set
			{
				UiEditorScene.SelectedEntity2DList = value;
				EnableButtons = UiEditorScene.SelectedEntity2DList.Count != 0;
			}
		}

		public bool EnableButtons
		{
			get { return UiEditorScene.EnableButtons; }
			set
			{
				UiEditorScene.EnableButtons = value;
				RaisePropertyChanged("EnableButtons");
			}
		}

		private void AddControlToScene(Vector2D position)
		{
			if (UiEditorScene.AddControl(position))
				return; //ncrunch: no coverage
			RaiseAllProperties();
		}

		private void RaiseAllProperties()
		{
			RaisePropertyChanged("SelectedSpriteNameInList");
			RaisePropertyChanged("SpriteListIndex");
			RaisePropertyChanged("Entity2DWidth");
			RaisePropertyChanged("Entity2DHeight");
			RaisePropertyChanged("ControlName");
			RaisePropertyChanged("ControlLayer");
			RaisePropertyChanged("ContentText");
			RaisePropertyChanged("UIImagesInList");
			RaisePropertyChanged("ImageInGridList");
			RaisePropertyChanged("ResolutionList");
			RaisePropertyChanged("RightMargin");
			RaisePropertyChanged("LeftMargin");
			RaisePropertyChanged("BottomMargin");
			RaisePropertyChanged("TopMargin");
			RaisePropertyChanged("SelectedName");
			RaisePropertyChanged("HorizontalAllignment");
		}

		private void CreateCenteredControl(string control)
		{
			UiEditorScene.CreateNewCenteredControl(control);
			RaiseAllProperties();
		}

		public string UIName
		{
			get { return UiEditorScene.UIName; }
			set
			{
				UiEditorScene.UIName = value;
				UpdateIfCanSaveScene();
				RaisePropertyChanged("UIName");
			}
		}

		private void UpdateIfCanSaveScene()
		{
			CanSaveScene = UIWidth != 0 && UIHeight != 0 && !string.IsNullOrEmpty(UIName);
		}

		public bool CanSaveScene
		{
			get { return UiEditorScene.CanSaveScene; }
			set
			{
				UiEditorScene.CanSaveScene = value;
				RaisePropertyChanged("CanSaveScene");
			}
		}

		public int UIWidth
		{
			get { return (int)UiEditorScene.SceneResolution.Width; }
			set
			{
				UiEditorScene.ChangeUIWidth(value);
				UpdateIfCanSaveScene();
			}
		}

		public int UIHeight
		{
			get { return (int)UiEditorScene.SceneResolution.Height; }
			set
			{
				UiEditorScene.ChangeUIHeight(value);
				UpdateIfCanSaveScene();
			}
		}

		private void SetMessengers()
		{
			Messenger.Default.Register<int>(this, "ChangeRenderLayer", ChangeRenderLayer);
			Messenger.Default.Register<string>(this, "AddNewResolution", AddNewResolution);
			Messenger.Default.Register<List<string>>(this, "SetSelectedNameFromHierachy",
				SetSelectedNamesFromHierachy);
			Messenger.Default.Register<int>(this, "SetSelectedIndexFromHierachy",
				SetSelectedIndexFromHierachy);
			Messenger.Default.Register<string>(this, "SetCenteredControl", CreateCenteredControl);
			Messenger.Default.Register<string>(this, "SetMouseCommands", SetMouseCommands);
			Messenger.Default.Register<string>(this, "DeleteSelectedContent", DeleteSelectedContent);
			Messenger.Default.Register<string>(this, "ClearScene", ClearScene);
			Messenger.Default.Register<ContentIconAndName>(this, "SelectContentInContentManager",
				SelectContentInContentManager);
		}

		public Scene Scene
		{
			get { return UiEditorScene.Scene; }
		}

		public bool CanGenerateSourceCode
		{
			get { return UiEditorScene.CanGenerateSourceCode; }
			set
			{
				UiEditorScene.CanGenerateSourceCode = value;
				RaisePropertyChanged("CanGenerateSourceCode");
			}
		}

		public void ChangeRenderLayer(int changeValue)
		{
			ControlLayer += changeValue;
			RaisePropertyChanged("ControlLayer");
		}

		public int ControlLayer
		{
			get { return UiEditorScene.uiControl.controlLayer; }
			set { UiEditorScene.ChangeControlLayer(value); }
		}

		public void AddNewResolution(string obj)
		{
			if (UiEditorScene.AddAndSelectResolution())
				return;
			RaiseAllProperties();
			SelectedResolution = UiEditorScene.SelectedResolution;
			RaisePropertyChanged("SelectedResolution");
		}

		public ObservableCollection<string> ResolutionList
		{
			get { return UiEditorScene.ResolutionList; }
		}

		public string SelectedResolution
		{
			get { return UiEditorScene.SelectedResolution; }
			set
			{
				UiEditorScene.ChangeGrid(value);
				RaisePropertyChanged("SelectedResolution");
			}
		}

		public void SetSelectedNamesFromHierachy(List<string> newSelectedName)
		{
			SelectedControlNamesInList = newSelectedName;
		}

		public List<string> SelectedControlNamesInList
		{
			get { return UiEditorScene.SelectedControlNamesInList; }
			set
			{
				UiEditorScene.SetSelectedControlNamesInList(value);
				RaiseAllProperties();
			}
		}

		public void SetSelectedIndexFromHierachy(int newSelectedIndex)
		{
			ControlListIndex = newSelectedIndex;
		}

		public int ControlListIndex
		{
			get { return UIControl.Index; }
			set { UIControl.Index = value; }
		}
		private UIControl UIControl
		{
			get { return UiEditorScene.uiControl; }
		}

		public void DeleteSelectedContent(string arg)
		{
			UiEditorScene.DeleteSelectedContent();
			SelectedControlNamesInList.Clear();
		}

		public void ClearScene(string obj)
		{
			UiEditorScene.ClearScene();
			SetMouseCommands("");
		}

		//ncrunch: no coverage start
		private void SelectContentInContentManager(ContentIconAndName content)
		{
			var type = service.GetTypeOfContent(content.Name);
			if (type != ContentType.Scene)
				return;
			UIName = content.Name;
			LoadScene();
		}//ncrunch: no coverage end

		public void LoadScene()
		{
			if (UiEditorScene.LoadScene())
				return; //ncrunch: no coverage
			UpdateIfCanSaveScene();
			SetMouseCommands("");
		}

		public ObservableCollection<string> ContentImageListList
		{
			get { return UiEditorScene.ContentImageListList; }
		}

		public ObservableCollection<string> UIImagesInList
		{
			get { return UiEditorScene.UIImagesInList; }
		}

		public ObservableCollection<string> MaterialList
		{
			get { return UiEditorScene.MaterialList; }
		}

		public ObservableCollection<string> SceneNames
		{
			get { return UiEditorScene.SceneNames; }
		}

		public string SelectedName
		{
			get { return UiEditorScene.SelectedName; }
			set { UiEditorScene.SetSelectedName(value); }
		}

		public int SelectedIndex
		{
			get { return UiEditorScene.SelectedIndex; }
			set { UiEditorScene.SetSelectedIndex(value); }
		}

		public string ControlName
		{
			get { return UiEditorScene.uiControl.ControlName; }
			set { UiEditorScene.controlChanger.ChangeControlName(value, UiEditorScene); }
		}

		public int GridWidth
		{
			get { return UiEditorScene.UISceneGrid.GridWidth; }
			set
			{
				UiEditorScene.UISceneGrid.GridWidth = value;
				UiEditorScene.UISceneGrid.DrawGrid();
			}
		}

		public int GridHeight
		{
			get { return UiEditorScene.UISceneGrid.GridHeight; }
			set
			{
				UiEditorScene.UISceneGrid.GridHeight = value;
				UiEditorScene.UISceneGrid.DrawGrid();
			}
		}

		public bool IsShowingGrid
		{
			get { return UiEditorScene.IsDrawingGrid; }
			set
			{
				UiEditorScene.IsDrawingGrid = value;
				UiEditorScene.UISceneGrid.DrawGrid();
			}
		}

		public float Entity2DWidth
		{
			get { return UIControl.EntityWidth; }
			set { ControlChanger.SetWidth(value, UiEditorScene); }
		}

		private ControlChanger ControlChanger
		{
			get { return UiEditorScene.controlChanger; }
		}

		public float Entity2DHeight
		{
			get { return UIControl.EntityHeight; }
			set { ControlChanger.SetHeight(value, UiEditorScene); }
		}

		public string ContentText
		{
			get { return UIControl.contentText; }
			set { ControlChanger.SetContentText(value, UiEditorScene); }
		}

		public bool IsSnappingToGrid
		{
			get { return UiEditorScene.IsSnappingToGrid; }
			set { UiEditorScene.IsSnappingToGrid = value; }
		}

		public string SelectedMaterial
		{
			get { return UIControl.SelectedMaterial; }
			set { UIControl.SelectedMaterial = value; }
		}

		public string SelectedHoveredMaterial
		{
			get { return UIControl.SelectedHoveredMaterial; }
			set { UIControl.SelectedHoveredMaterial = value; }
		}

		public string SelectedPressedMaterial
		{
			get { return UIControl.SelectedPressedMaterial; }
			set { UIControl.SelectedPressedMaterial = value; }
		}

		public string SelectedDisabledMaterial
		{
			get { return UIControl.SelectedDisabledMaterial; }
			set { UIControl.SelectedDisabledMaterial = value; }
		}

		public float BottomMargin
		{
			get { return UIControl.BottomMargin; }
			set
			{
				UiEditorScene.ControlAllignmentAndMargins.ChangeBottomMargin(value);
				RaiseAllProperties();
			}
		}

		public float TopMargin
		{
			get { return UIControl.TopMargin; }
			set
			{
				UiEditorScene.ControlAllignmentAndMargins.ChangeTopMargin(value);
				RaiseAllProperties();
			}
		}

		public float LeftMargin
		{
			get { return UIControl.LeftMargin; }
			set
			{
				UiEditorScene.ControlAllignmentAndMargins.ChangeLeftMargin(value);
				RaiseAllProperties();
			}
		}

		public float RightMargin
		{
			get { return UIControl.RightMargin; }
			set
			{
				UiEditorScene.ControlAllignmentAndMargins.ChangeRightMargin(value);
				RaiseAllProperties();
			}
		}

		public string HorizontalAllignment
		{
			get { return UIControl.HorizontalAllignment; }
			set
			{
				if (UiEditorScene.ControlAllignmentAndMargins.AllignControlsHorizontal(value))
					return;
				RaiseAllProperties();
			}
		}

		public string VerticalAllignment
		{
			get { return UIControl.VerticalAllignment; }
			set
			{
				if (UiEditorScene.ControlAllignmentAndMargins.AllignControlVertical(value))
					return; //ncrunch: no coverage
				RaisePropertyChanged("VerticalAllignment");
			}
		}

		public void Reset()
		{
			Messenger.Default.Send("ClearScene", "ClearScene");
			UiEditorScene.CreateNewGrid();
			UiEditorScene.RefreshOnContentChange();
			SetMouseCommands("");
		}

		public void ActivateHiddenScene()
		{
			UiEditorScene.ActivateHiddenScene();
			UiEditorScene.controlTransformer.uiMouseCursor.IsActive = true;
			SetMouseCommands("");
			Messenger.Default.Send("Scene", "OpenEditorPlugin");
		}

		public bool IsInteractiveButton
		{
			get { return UIControl.IsInteractiveButton; }
			set
			{
				if (UIControl.ChangeToInteractiveButton(value, UiEditorScene))
					return; //ncrunch: no coverage
				RaisePropertyChanged("IsInteractiveButton");
			}
		}
	}
}