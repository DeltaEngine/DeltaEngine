using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class UIEditorScene
	{
		public UIEditorScene(Service service)
		{
			editorService = service;
			InitializeDefaults();
			InitializeLists();
			FillResolutionListWithDefaultResolutions();
			CreateNewGrid();
			FillContentImageList();
			FillSceneNames();
			FillListOfAvailableFonts();
			FillMaterialList();
			SetMousePosition();
			SetMessengers();
		}

		private void InitializeDefaults()
		{
			InitializeClasses();
			SceneResolution = DefaultResolution;
			uiSceneGrid.GridRenderLayer = 10;
			NewGridWidth = 30;
			NewGridHeight = 30;
		}

		public int NewGridWidth { get; set; }
		public int NewGridHeight { get; set; }

		private void InitializeClasses()
		{
			ScreenSpace.Scene = new SceneScreenSpace(editorService.Viewport.Window, SceneResolution);
			ControlProcessor = new ControlProcessor();
			controlAdder = new ControlAdder();
			controlChanger = new ControlChanger();
			uiControl = new UIControl();
			controlTransformer = new ControlTransformer(editorService);
			controlAllignmentAndMargins = new ControlAllignmentAndMargins(this);
			controlMaterialChanger = new ControlMaterialChanger(this);
			uiSceneGrid = new UISceneGrid(this);
		}

		public ControlProcessor ControlProcessor { get; set; }
		public ControlAdder controlAdder;
		public ControlChanger controlChanger;
		public UIControl uiControl;
		public ControlTransformer controlTransformer;
		private ControlAllignmentAndMargins controlAllignmentAndMargins;
		public ControlMaterialChanger controlMaterialChanger;

		private void SetMessengers()
		{
			Messenger.Default.Register<string>(this, "ChangeGrid", ChangeGrid);
			Messenger.Default.Register<string>(this, "ChangeMaterial", ChangeMaterialIfSelectingControl);
			Messenger.Default.Register<string>(this, "ChangeHoveredMaterial",
				controlMaterialChanger.ChangeHoveredMaterial);
			Messenger.Default.Register<string>(this, "ChangePressedMaterial",
				controlMaterialChanger.ChangePressedMaterial);
			Messenger.Default.Register<string>(this, "ChangeDisabledMaterial",
				controlMaterialChanger.ChangeDisabledMaterial);
			Messenger.Default.Register<string>(this, "SaveUI", SaveUI);
		}

		private readonly Service editorService;
		public Size SceneResolution { get; set; }
		private static readonly Size DefaultResolution = new Size(1280, 720);

		private void InitializeLists()
		{
			Scene = new Scene();
			AvailableFontsInProject = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			UIImagesInList = new ObservableCollection<string>();
			ContentImageListList = new ObservableCollection<string>();
			SceneNames = new ObservableCollection<string>();
			ResolutionList = new ObservableCollection<string>();
			SelectedEntity2DList = new List<Entity2D>();
			SelectedControlNamesInList = new List<string>();
		}

		public Scene Scene { get; set; }
		public ObservableCollection<string> AvailableFontsInProject { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> UIImagesInList { get; set; }
		public ObservableCollection<string> ContentImageListList { get; set; }
		public ObservableCollection<string> SceneNames { get; set; }
		public ObservableCollection<string> ResolutionList { get; set; }
		public List<Entity2D> SelectedEntity2DList { get; set; }
		public List<string> SelectedControlNamesInList { get; set; }
		public ControlTransformer UIControlTransformer { get; set; }

		public void FillResolutionListWithDefaultResolutions()
		{
			ResolutionList.Add("10 x 10");
			ResolutionList.Add("16 x 16");
			ResolutionList.Add("20 x 20");
			ResolutionList.Add("24 x 24");
			ResolutionList.Add("50 x 50");
			IsDrawingGrid = true;
			ChangeGrid("20 x 20");
		}

		public string SelectedResolution { get; set; }
		public bool IsDrawingGrid { get; set; }

		public void CreateNewGrid()
		{
			uiSceneGrid.CreateGritdOutline();
			uiSceneGrid.UpdateGridOutline(SceneResolution);
			UISceneGrid.DrawGrid();
			foreach (var line in UISceneGrid.LinesInGridList)
				line.RenderLayer = 1;
		}

		public void FillContentImageList()
		{
			ContentImageListList.Clear();
			var imageList = editorService.GetAllContentNamesByType(ContentType.Image);
			foreach (var image in imageList)
				ContentImageListList.Add(image);
		}

		public void FillSceneNames()
		{
			SceneNames.Clear();
			foreach (var newScene in editorService.GetAllContentNamesByType(ContentType.Scene))
				SceneNames.Add(newScene);
		}

		private void FillListOfAvailableFonts()
		{
			AvailableFontsInProject.Clear();
			var fontsInProject = editorService.GetAllContentNamesByType(ContentType.Font);
			foreach (var fontName in fontsInProject)
				AvailableFontsInProject.Add(fontName);
			if (AvailableFontsInProject.Count > 0)
				SelectedFontName = AvailableFontsInProject[0];
		}

		public string SelectedFontName { get; set; }

		public void FillMaterialList()
		{
			MaterialList.Clear();
			var materialList = editorService.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in
				materialList.Where(ControlMaterialChanger.TryAddMaterial))
				MaterialList.Add(material);
		}

		public void SetMousePosition()
		{
			var middleClick = new MouseButtonTrigger();
			middleClick.AddTag("temporary");
			var setLastPositionCommand = new Command(position => SetPosition(position)).Add(middleClick);
			setLastPositionCommand.AddTag("temporary");
			var moveMouse = new MousePositionTrigger(MouseButton.Left, State.Released);
			moveMouse.AddTag("temporary");
			var checkTransformationCommand =
				new Command(position => CheckIfCanTransformControl(position)).Add(moveMouse);
			checkTransformationCommand.AddTag("temporary");
			var pressAndMoveMouse = new MousePositionTrigger(MouseButton.Left, State.Pressed);
			pressAndMoveMouse.AddTag("temporary");
			var pressOnControlCommand =
				new Command(position => TransformSelectedControl(position)).Add(pressAndMoveMouse);
			pressOnControlCommand.AddTag("temporary");
			var pressControl = new KeyTrigger(Key.LeftControl);
			pressControl.AddTag("temporary");
			var multiSelectControl = new Command(() => SetMultiSelection(true)).Add(pressControl);
			multiSelectControl.AddTag("temporary");
			var releaseControl = new KeyTrigger(Key.LeftControl, State.Releasing);
			releaseControl.AddTag("temporary");
			var disableMultiSelectControl =
				new Command(() => SetMultiSelection(false)).Add(releaseControl);
			disableMultiSelectControl.AddTag("temporary");
		}

		private bool SetMultiSelection(bool canMultiSelect)
		{
			return IsMultiSelecting = canMultiSelect;
		}

		//ncrunch: no coverage start
		private Vector2D SetPosition(Vector2D position)
		{
			return ControlProcessor.lastMousePosition = position;
		} //ncrunch: no coverage end

		public void UpdateOutLine(List<Entity2D> selectedEntity2D)
		{
			ControlProcessor.UpdateOutlines(selectedEntity2D);
		}

		public bool LoadScene()
		{
			if (!ContentLoader.Exists(UIName, ContentType.Scene))
				return true; //ncrunch: no coverage
			editorService.Viewport.DestroyRenderedEntities();
			Messenger.Default.Send("ClearScene", "ClearScene");
			var scene = new Scene();
			Scene = new Scene();
			try
			{
				scene = ContentLoader.Load<Scene>(UIName);
				foreach (var control in scene.Controls)
					ActivateControl((Control)control);
			}
			catch
			{
				foreach (var control in EntitiesRunner.Current.GetEntitiesOfType<Control>())
					ActivateControl((Control)control);
			}
			UISceneGrid.DrawGrid();
			uiSceneGrid.UpdateGridOutline(SceneResolution);
			controlTransformer = new ControlTransformer(editorService);
			return false;
		}

		private void ActivateControl(Control control)
		{
			control.IsActive = true;
			if (control.Contains<Material>())
				if (control.Get<Material>().Shader == null)
					control.Set(new Theme().Button); //ncrunch: no coverage
			UIImagesInList.Add(control.Name);
			Messenger.Default.Send(control.Name, "AddToHierachyList");
			if (uiSceneGrid.GridRenderLayer <= control.RenderLayer)
				uiSceneGrid.GridRenderLayer = control.RenderLayer + 1; //ncrunch: no coverage
			controlAdder.AddControlToScene(control, Scene);
			control.IsActive = false;
		}

		public string UIName { get; set; }

		public void ChangeGrid(string grid)
		{
			SelectedResolution = grid;
			var newGridWidthAndHeight = grid.Trim(new[] { ' ' });
			var gridwidthAndHeight = newGridWidthAndHeight.Split((new[] { '{', ',', '}', 'x' }));
			int width;
			int.TryParse(gridwidthAndHeight[0], out width);
			int height;
			int.TryParse(gridwidthAndHeight[1], out height);
			if (width <= 0 || height <= 0)
				return;
			SetGridWidthAndHeight(width, height);
			UISceneGrid.DrawGrid();
		}

		private void SetGridWidthAndHeight(int width, int height)
		{
			UISceneGrid.GridWidth = width;
			UISceneGrid.GridHeight = height;
		}

		public void ActivateHiddenScene()
		{
			foreach (var control in Scene.Controls)
				control.IsActive = true;
			foreach (Line2D line in ControlProcessor.Outlines.SelectMany(lines => lines))
				line.IsActive = true;
			foreach (var line in UISceneGrid.LinesInGridList)
				line.IsActive = true;
			foreach (var line in uiSceneGrid.GridOutline)
				line.IsActive = true;
			if (editorService.Viewport != null)
				ResetViewport(); //ncrunch: no coverage
		}

		//ncrunch: no coverage start
		private void ResetViewport()
		{
			editorService.Viewport.CenterViewOn(Vector2D.Half);
			editorService.Viewport.ZoomViewTo(1.0f);
		} //ncrunch: no coverage end

		public void DeleteSelectedContent()
		{
			for (int i = 0; i < SelectedControlNamesInList.Count; i++)
			{
				var control = SelectedControlNamesInList[i];
				var index = UIImagesInList.IndexOf(control);
				if (index < 0)
					return;
				RemoveSelectedControl(index);
			}
		}

		private void RemoveSelectedControl(int index)
		{
			Entity2D entity2D = Scene.Controls[index];
			UIImagesInList.RemoveAt(index);
			Scene.Remove(entity2D);
			SetNewControlAfterDelete(index);
			UpdateControlListAfterDelete();
		}

		private void SetNewControlAfterDelete(int index)
		{
			SelectedEntity2DList.Clear();
			if (Scene.Controls.Count == 0)
				return;
			SelectedEntity2DList.Add(Scene.Controls.Count == index
				? Scene.Controls[index - 1] : Scene.Controls[index]);
		}

		private void UpdateControlListAfterDelete()
		{
			uiControl.Index = -1;
			ControlProcessor.UpdateOutlines(SelectedEntity2DList);
			Messenger.Default.Send("", "DeleteSelectedContent");
			if (SelectedEntity2DList.Count == 0)
				return;
			uiControl.Index =
				Scene.Controls.IndexOf(SelectedEntity2DList[SelectedEntity2DList.Count - 1]);
			SelectedControlNamesInList.Clear();
			SelectedControlNamesInList.Add((Scene.Controls[uiControl.Index] as Control).Name);
			Messenger.Default.Send(SelectedControlNamesInList, "SetSelectedName");
			Messenger.Default.Send(uiControl.Index, "SetSelectedIndex");
		}

		public void DeleteSelectedControl(string obj)
		{
			for (int i = 0; i < SelectedEntity2DList.Count; i++)
			{
				Entity2D entity2D = SelectedEntity2DList[i];
				if (entity2D == null || !CanDeleteSelectedControl)
					return; //ncrunch: no coverage
				uiControl.Index = Scene.Controls.IndexOf(entity2D);
				if (uiControl.Index < 0)
					return; //ncrunch: no coverage
				var index = Scene.Controls.IndexOf(entity2D);
				RemoveSelectedControl(index);
			}
		}

		public bool CanDeleteSelectedControl { get; set; }

		public void ClearScene()
		{
			foreach (var control in Scene.Controls)
				control.IsActive = false;
			Scene.Controls.Clear();
			UIImagesInList.Clear();
			SelectedEntity2DList.Clear();
			EnableButtons = SelectedEntity2DList.Count != 0;
			foreach (var entity in EntitiesRunner.Current.GetEntitiesOfType<DrawableEntity>())
				entity.IsActive = false;
		}

		public bool EnableButtons { get; set; }

		public void SetEntity2D(Control control)
		{
			if (control == null)
				return;
			if (!IsMultiSelecting)
			{
				SelectedEntity2DList.Clear();
				SelectedControlNamesInList.Clear();
			}
			controlChanger.ChangeUIControlWidthAndHeight(control, uiControl);
			uiControl.Index = Scene.Controls.IndexOf(control);
			if (uiControl.Index < 0)
				return;
			UpdateUIControlAndLists(control);
			SelectedName = control.Name;
		}

		private void UpdateUIControlAndLists(Control control)
		{
			SelectedControlNamesInList.Add(control.Name);
			if (!SelectedEntity2DList.Contains(control))
				SelectedEntity2DList.Add(control);
			IsSelectingControl = true;
			uiControl.SetMaterials(SelectedEntity2DList);
			Messenger.Default.Send(SelectedControlNamesInList, "SetSelectedName");
			Messenger.Default.Send(uiControl.Index, "SetSelectedIndex");
			ControlProcessor.UpdateOutlines(SelectedEntity2DList);
			controlChanger.SetControlLayer(control.RenderLayer, this);
		}

		public bool IsMultiSelecting { get; set; }

		public List<Entity2D> FindEntity2DOnPosition(Vector2D mousePosition)
		{
			ClearSceneWhenNotMulitSelecting(mousePosition);
			var control = FindControlOnPositionWithHighestRenderlayer(mousePosition);
			if (control == null)
				return new List<Entity2D>();
			if (IsAnchoringControls)
			{
				IsAnchoringControls = false;
				ControlAnchorer.AnchorSelectedControls(control, SelectedEntity2DList);
			}
			UpdateOutLine(SelectedEntity2DList);
			uiControl.UpdateMaterialsInViewPort(LastEntityInList);
			return SelectedEntity2DList;
		}

		private void ClearSceneWhenNotMulitSelecting(Vector2D mousePosition)
		{
			CanDeleteSelectedControl = true;
			if (SelectedEntity2DList.Count != 0 && SelectedEntity2DList.Count < 2)
				if (SelectedEntity2DList[0].GetType() == typeof(Button) &&
					SelectedEntity2DList[0].DrawArea.Contains(mousePosition))
					uiControl.isClicking = true;
			ControlProcessor.lastMousePosition = mousePosition;
			if (IsMultiSelecting || IsAnchoringControls)
				return;
			SelectedEntity2DList.Clear();
			SelectedControlNamesInList.Clear();
		}

		private Control FindControlOnPositionWithHighestRenderlayer(Vector2D mousePosition)
		{
			Control foundControl = null;
			for (int index = 0; index < Scene.Controls.Count; index++)
			{
				var control = (Control)Scene.Controls[index];
				if (!control.RotatedDrawAreaContains(mousePosition))
					continue;
				if (SelectedEntity2DList.Count != 0 && control.RenderLayer < LastEntityInList.RenderLayer)
					continue;
				foundControl = control;
				if (!IsAnchoringControls)
					SetEntity2D(control);
			}
			if (SelectedEntity2DList.Count == 0)
				controlAdder.IsDragging = true;
			return foundControl;
		}

		public bool IsAnchoringControls;

		private Entity2D LastEntityInList
		{
			get
			{
				return SelectedEntity2DList.Count == 0
					? null : SelectedEntity2DList[SelectedEntity2DList.Count - 1];
			}
		}

		public void SetSingleSelectedControl(Control control)
		{
			SelectedEntity2DList.Clear();
			SelectedControlNamesInList.Clear();
			SelectedEntity2DList.Add(control);
			SelectedControlNamesInList.Add(control.Name);
		}

		public bool IsSnappingToGrid { get; set; }
		public bool CanSaveScene { get; set; }
		public bool IsSelectingControl { get; set; }
		public bool CanGenerateSourceCode { get; set; }
		public string SelectedName;
		public int SelectedIndex;
		private UISceneGrid uiSceneGrid;

		public void TransformSelectedControl(Vector2D position)
		{
			if (controlTransformer.uiMouseCursor.CurrentCursor == ControlTransformer.MoveCursor)
			{
				ControlProcessor.MoveImage(position, SelectedEntity2DList, controlAdder.IsDragging, this);
				ControlTransformer.isTransforming = true;
			}
			if (controlTransformer.uiMouseCursor.CurrentCursor == ControlTransformer.ScaleCursor)
				controlTransformer.ScaleControl(position, SelectedEntity2DList);
			if (controlTransformer.uiMouseCursor.CurrentCursor == ControlTransformer.RotateCursor)
				controlTransformer.RotateControl(position, SelectedEntity2DList);
		}

		public void CreateNewCenteredControl(string control)
		{
			controlAdder.CreateCenteredControl(control, this);
			SetEntity2D((Control)ShownEntity2D);
		}

		private Entity2D ShownEntity2D
		{
			get
			{
				return SelectedEntity2DList.Count == 0
					? null : SelectedEntity2DList[SelectedEntity2DList.Count - 1];
			}
		}

		public ControlAllignmentAndMargins ControlAllignmentAndMargins
		{
			get { return controlAllignmentAndMargins; }
		}

		public UISceneGrid UISceneGrid
		{
			get { return uiSceneGrid; }
		}

		public void SetSelectedControlNamesInList(List<string> value)
		{
			SelectedControlNamesInList = value;
			controlChanger.SetSelectedControlNameInList(value, this);
		}

		public void ChangeControlLayer(int value)
		{
			if (uiSceneGrid.GridRenderLayer <= value)
			{
				uiSceneGrid.GridRenderLayer = value + 1;
				uiSceneGrid.UpdateRenderlayerGrid();
			}
			controlChanger.SetControlLayer(value, this);
		}

		public void RefreshOnContentChange()
		{
			FillContentImageList();
			FillMaterialList();
			FillSceneNames();
			controlTransformer = new ControlTransformer(editorService);
		}

		public void CheckIfCanTransformControl(Vector2D mousePos)
		{
			controlTransformer.ChangeCursorIfCanTransform(SelectedEntity2DList, mousePos,
				ControlProcessor);
		}

		public void SetSelectedName(string value)
		{
			if (value == null)
				return;
			SelectedName = value;
			SelectedControlNamesInList.Add(SelectedName);
		}

		public void SetSelectedIndex(int value)
		{
			if (value < 0)
				return;
			SelectedIndex = value;
			var control = Scene.Controls[value];
			SetEntity2D((Control)control);
		}

		public void ChangeMaterialIfSelectingControl(string newMaterial)
		{
			if (IsSelectingControl)
			{
				IsSelectingControl = false;
				return;
			}
			controlMaterialChanger.ChangeMaterial(newMaterial);
			UpdateOutLine(SelectedEntity2DList);
		}

		public bool AddControl(Vector2D position)
		{
			ControlTransformer.isTransforming = false;
			controlAdder.AddControl(position, this);
			uiControl.isClicking = false;
			if (SelectedEntity2DList == null)
				return true; //ncrunch: no coverage
			ControlProcessor.UpdateOutlines(SelectedEntity2DList);
			SetEntity2D((Control)ShownEntity2D);
			return false;
		}

		public void ChangeUIWidth(int value)
		{
			SceneResolution = new Size(value, SceneResolution.Height);
			(ScreenSpace.Scene as SceneScreenSpace).ForcedUpdate(SceneResolution);
			foreach (var control in Scene.Controls)
				uiControl.SetControlSize(control, control.Get<Material>(), this);
			uiSceneGrid.UpdateGridOutline(SceneResolution);
			UISceneGrid.DrawGrid();
		}

		public void ChangeUIHeight(int value)
		{
			SceneResolution = new Size(SceneResolution.Width, value);
			(ScreenSpace.Scene as SceneScreenSpace).ForcedUpdate(SceneResolution);
			foreach (var control in Scene.Controls)
				uiControl.SetControlSize(control, control.Get<Material>(), this);
			uiSceneGrid.UpdateGridOutline(SceneResolution);
			UISceneGrid.DrawGrid();
		}

		public bool AddAndSelectResolution()
		{
			if (NewGridWidth <= 0 || NewGridHeight <= 0)
				return true;
			SelectedResolution = NewGridWidth + " x " + NewGridHeight;
			if (ResolutionList.Contains(NewGridWidth + " x " + NewGridHeight))
				return false;
			RemoveOldestResolutionIfMoreThenTen();
			ResolutionList.Add(NewGridWidth + " x " + NewGridHeight);
			return false;
		}

		public void RemoveOldestResolutionIfMoreThenTen()
		{
			if (ResolutionList.Count > 9)
				for (int i = 0; i < 10; i++)
					if (i == 9)
						ResolutionList.RemoveAt(i);
					else
						ResolutionList[i] = ResolutionList[i + 1];
		}

		public void SaveUI(string obj)
		{
			if (Scene.Controls.Count == 0 || string.IsNullOrEmpty(UIName))
				return; //ncrunch: no coverage
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			var bytes = BinaryDataExtensions.ToByteArrayWithTypeInformation(Scene);
			fileNameAndBytes.Add(UIName + ".deltascene", bytes);
			var metaDataCreator = new ContentMetaDataCreator();
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromUI(UIName, bytes);
			editorService.UploadContent(contentMetaData, fileNameAndBytes);
			if (CanGenerateSourceCode)
				new SceneCodeGenerator(editorService, Scene, UIName).GenerateSourceCodeClass();
		}
	}
}