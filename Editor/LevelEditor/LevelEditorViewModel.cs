using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D.Cameras;
using DeltaEngine.Rendering3D.Shapes;
using GalaSoft.MvvmLight;
using MessageBox = System.Windows.MessageBox;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.LevelEditor
{
	public class LevelEditorViewModel : ViewModelBase
	{
		public LevelEditorViewModel(Service service)
		{
			this.service = service;
			InitializeClasses();
			IsSaveEnabled = true;
			IsAddWaveEnabled = SpawnTypeList != "";
			IsRemoveWaveEnabled = false;
			InitializeLists();
			ContentName = "MyNewLevel";
			SetCamera();
			Is3D = false;
			contentListUpdater.GetBackgroundImages();
			contentListUpdater.GetLevelObjects(Is3D);
			contentListUpdater.GetLevels();
		}

		private readonly Service service;

		private void InitializeClasses()
		{
			contentListUpdater = new ContentListUpdater(this, service);
			waveHandler = new WaveHandler(this);
			propertyUpdater = new PropertyUpdater(this);
			xmlSaver = new XmlSaver(service);
			Level = new Level(new Size(12, 12));
			backgroundImageHandler = new BackgroundImageHandler(Level);
			renderer = new LevelDebugRenderer(Level);
			cameraSliders = new CameraSliders(renderer);
			levelObjectHandler = new LevelObjectHandler((int)(Level.Size.Width * Level.Size.Height));
			levelCommands = new LevelEditorCommands(this);
		}

		public ContentListUpdater contentListUpdater;
		private WaveHandler waveHandler;
		private PropertyUpdater propertyUpdater;
		private BackgroundImageHandler backgroundImageHandler;
		public XmlSaver xmlSaver;
		public Level Level { get; private set; }
		public LevelDebugRenderer renderer;
		public CameraSliders cameraSliders;
		public LevelObjectHandler levelObjectHandler;
		public LevelEditorCommands levelCommands;

		private void DestroyEntitiesAndSetCommands()
		{
			service.Viewport.DestroyRenderedEntities();
			levelCommands.SetCommands();
		}

		public bool IsSaveEnabled
		{
			get { return isSaveEnabled; }
			set
			{
				isSaveEnabled = value;
				RaisePropertyChanged("IsSaveEnabled");
			}
		}

		private bool isSaveEnabled;

		public bool IsAddWaveEnabled
		{
			get { return isAddWaveEnabled; }
			set
			{
				isAddWaveEnabled = value;
				RaisePropertyChanged("IsAddWaveEnabled");
			}
		}

		private bool isAddWaveEnabled;

		public bool IsRemoveWaveEnabled
		{
			get { return isRemoveWaveEnable; }
			set
			{
				isRemoveWaveEnable = value;
				RaisePropertyChanged("IsRemoveWaveEnabled");
			}
		}

		private bool isRemoveWaveEnable;
		
		private void InitializeLists()
		{
			LevelList = new ObservableCollection<string>();
			LevelSizeList = new ObservableCollection<Size> { new Size(8, 8), new Size(12, 12),
				new Size(16, 16), new Size(24, 24), new Size(32, 32), new Size(64, 64) };
			BackgroundImages = new ObservableCollection<string>();
			ModelList = new ObservableCollection<string>();
			WaveList = new ObservableCollection<Wave>();
			WaveNameList = new ObservableCollection<string>();
			CameraList = new ObservableCollection<string> { "3, 3, 3" };
			LevelObjects = new ObservableCollection<string>();
		}

		public ObservableCollection<string> LevelList { get; set; }
		public ObservableCollection<Size> LevelSizeList { get; set; }
		public ObservableCollection<string> BackgroundImages { get; set; }
		public ObservableCollection<string> ModelList { get; set; }
		public ObservableCollection<Wave> WaveList { get; set; }
		public ObservableCollection<string> WaveNameList { get; set; }
		public ObservableCollection<string> CameraList { get; set; }
		public ObservableCollection<string> LevelObjects { get; set; }

		public string ContentName
		{
			get { return contentName; }
			set
			{
				contentName = value;
				xmlSaver.ContentName = contentName;
				SelectedBackgroundImage = "";
				IsSaveEnabled = value != "";
				Is3D = false;
				LoadLevel();
				LoadWaves();
				ShowSliders();
				RaisePropertyChanged("ContentName");
				RaisePropertyChanged("CustomSize");
				RaisePropertyChanged("SelectedImage");
			}
		}

		private string contentName;

		private void ShowSliders()
		{
			if (Is3D)
				cameraSliders.Hide();
			else
			{
				cameraSliders.CreateSliders();
				cameraSliders.Show();
			}
		}

		private static void SetCamera()
		{
			camera3D = Camera.Use<LookAtCamera>();
			camera3D.Position = new Vector3D(-3.0f, -3.0f, 3.0f);
			camera3D.Target = Vector3D.Zero;
		}

		private static LookAtCamera camera3D;

		public string SelectedBackgroundImage
		{
			get { return selectedBackgroundImage; }
			set
			{
				selectedBackgroundImage = value;
				IsBgSizeEnabled = selectedBackgroundImage != "";
				EnableNoneButton();
				SetBackground();
				RaisePropertyChanged("SelectedImage");
			}
		}

		private string selectedBackgroundImage;

		private void LoadLevel()
		{
			if (!ContentLoader.Exists(contentName, ContentType.Level))
				return;
			DestroyEntitiesAndSetCommands();
			Level = ContentLoader.Load<Level>(contentName);
			levelCommands.Level = Level;
			renderer.RemoveCommands();
			Level.InitializeData();
			renderer = new LevelDebugRenderer(Level);
			levelCommands.Renderer = renderer;
			levelObjectHandler.LevelSize = (int)(Level.Size.Width * Level.Size.Height);
		}

		private void LoadWaves()
		{
			waveHandler.LoadWaves();
			RaisePropertyChanged("WaveNameList");
		}

		public bool IsNoneEnabled
		{
			get { return isNoneEnabled; }
			set
			{
				isNoneEnabled = value;
				RaisePropertyChanged("IsNoneEnabled");
			}
		}

		private bool isNoneEnabled;

		public bool IsBgSizeEnabled
		{
			get { return isBgSizeEnabled; }
			set
			{
				isBgSizeEnabled = value;
				RaisePropertyChanged("IsBgSizeEnabled");
			}
		}

		private bool isBgSizeEnabled;

		public bool IsBgImageEnabled
		{
			get { return isBgImageEnabled; }
			set
			{
				isBgImageEnabled = value;
				RaisePropertyChanged("IsBgImageEnabled");
			}
		}

		private bool isBgImageEnabled;

		private void SetBackground()
		{
			if (Is3D)
				levelObjectHandler.SetBackgroundModel(SelectedBackgroundModel);
			else
				backgroundImageHandler.SetBackgroundImage(SelectedBackgroundImage);
		}

		public string SelectedBackgroundModel
		{
			get { return selectedBackgroundModel; }
			set
			{
				selectedBackgroundModel = value;
				EnableNoneButton();
				RaisePropertyChanged("SelectedModel");
				levelObjectHandler.SetBackgroundModel(value);
			}
		}

		private string selectedBackgroundModel;

		public LevelTileType SelectedTileType
		{
			get { return selectedTileType; }
			set
			{
				selectedTileType = value;
				levelCommands.SelectedTileType = value;
				RaisePropertyChanged("SelectedTileType");
			}
		}

		private LevelTileType selectedTileType;

		public string CustomSize
		{
			get { return Level.Size.ToString(); }
			set
			{
				if (Level.GetAllTilesOfType(LevelTileType.Nothing).Count <
				Level.Size.Width * Level.Size.Height)
					if (!GetDialogResultToClearLevel()) //ncrunch: no coverage
						return; //ncrunch: no coverage
				if (value.SplitAndTrim(',').Length != 2)
					return;
				Level.Size = new Size(value);
				DestroyEntitiesAndSetCommands();
				SetBackground();
				renderer.RecreateTiles();
				levelObjectHandler.LevelSize = (int)(Level.Size.Width * Level.Size.Height);
			}
		}

		//ncrunch: no coverage start
		private static bool GetDialogResultToClearLevel()
		{
			var dialogResult = (DialogResult)MessageBox.Show(
					"Changing the size clears the level. Do you want to continue?", "Change the size",
					MessageBoxButton.YesNo, MessageBoxImage.Question);
			return dialogResult == DialogResult.Yes;
		} //ncrunch: no coverage end

		public void SetWaveProperties()
		{
			waveHandler.SetWaveProperties();
		}

		private string waveName;

		public Wave SelectedWave
		{
			get { return selectedWave; }
			set
			{
				selectedWave = value;
				if (selectedWave == null)
					return;
				IsRemoveWaveEnabled = true;
				WaitTime = selectedWave.WaitTime;
				SpawnInterval = selectedWave.SpawnInterval;
				MaxTime = selectedWave.MaxTime;
				SpawnTypeList = selectedWave.SpawnTypeList.ToText();
				WaveName = SelectedWave.ShortName;
				MaxSpawnItems = SelectedWave.MaxSpawnItems;
				RaisePropertyChanged("WaitTime");
				RaisePropertyChanged("SpawnInterval");
				RaisePropertyChanged("MaxTime");
				RaisePropertyChanged("SpawnTypeList");
				RaisePropertyChanged("WaveName");
				RaisePropertyChanged("MaxSpawnItems");
			}
		}

		private Wave selectedWave;

		public float WaitTime
		{
			get { return waitTime; }
			set
			{
				waitTime = value;
				RaisePropertyChanged("WaitTime");
			}
		}

		private float waitTime;

		public float SpawnInterval
		{
			get { return spawnInterval; }
			set
			{
				spawnInterval = value;
				RaisePropertyChanged("SpawnInterval");
			}
		}

		private float spawnInterval;

		public float MaxTime
		{
			get { return maxTime; }
			set
			{
				maxTime = value;
				RaisePropertyChanged("MaxTime");
			}
		}

		private float maxTime;

		public string SpawnTypeList
		{
			get { return spawnTypeList; }
			set
			{
				IsAddWaveEnabled = value != "";
				spawnTypeList = value;
				RaisePropertyChanged("SpawnTypeList");
			}
		}

		private string spawnTypeList;

		public int MaxSpawnItems
		{
			get { return maxSpawnItems; }
			set
			{
				maxSpawnItems = value;
				RaisePropertyChanged("MaxSpawnItems");
			}
		}

		private int maxSpawnItems;

		public string WaveName
		{
			get { return waveName; }
			set
			{
				waveName = value;
				RaisePropertyChanged("WaveName");
			}
		}

		public void AddWave()
		{
			waveHandler.AddWave(WaitTime, SpawnInterval, MaxTime, SpawnTypeList, WaveName, MaxSpawnItems);
			RaisePropertyChanged("WaveList");
			RaisePropertyChanged("WaveNameList");
		}

		public void RemoveSelectedWave()
		{
			waveHandler.RemoveSelectedWave();
			RaisePropertyChanged("WaveList");
			RaisePropertyChanged("WaveNameList");
		}

		public bool Is3D
		{
			get { return Level.RenderIn3D; }
			set
			{
				DisposeGrid();
				Level.RenderIn3D = value;
				DestroyEntitiesAndSetCommands();
				if (value && Camera.Current == null)
					SetCamera(); //ncrunch: no coverage
				IsBgImageEnabled = !value;
				if (selectedBackgroundImage != null && IsBgImageEnabled)
					SetBackground();
				contentListUpdater.GetLevelObjects(value);
				ShowSliders();
				levelObjectHandler.Is3D = value;
				renderer.RecreateTiles();
				LoadWaves();
				LoadLevel();
				if (value)
				{
					SelectedBackgroundImage = "";
					SelectedBackgroundModel = Level.ModelName;
				}
				else
					SelectedBackgroundModel = "";
				RaisePropertyChanged("Is3D");
				RaisePropertyChanged("CustomSize");
				RaisePropertyChanged("IsFog");
			}
		}

		private void DisposeGrid()
		{
			if (Is3D)
				renderer.Dispose3D(); //ncrunch: no coverage
			else
				renderer.Dispose2D();
		}

		private void EnableNoneButton()
		{
			IsNoneEnabled = SelectedBackgroundImage != "" || SelectedBackgroundModel != "";
		}

		public void ResetLevelEditor()
		{
			propertyUpdater.ResetLevelEditor(Is3D);
			service.Viewport.ZoomViewTo(1.0f);
			DestroyEntitiesAndSetCommands();
			renderer.RecreateTiles();
		}

		public string CameraPosition
		{
			get { return camera3D.Position.ToString(); }
			set
			{
				var position = StringToVectorConverter.Convert(value);
				if (position != null)
					camera3D.Position = (Vector3D)position;
				RaisePropertyChanged("CameraPosition");
			}
		}

		public void SaveToServer()
		{
			xmlSaver.Level = Level;
			xmlSaver.ModelName = selectedBackgroundModel;
			xmlSaver.SaveToServer();
		}

		public void ClearLevel()
		{
			DestroyEntitiesAndSetCommands();
			propertyUpdater.ClearLevel();
		}

		public void ClearWaves()
		{
			propertyUpdater.ClearWaves();
		}

		public void UpdateLists()
		{
			propertyUpdater.UpdateLists(Is3D);
		}

		public void IncreaseBgImageSize()
		{
			backgroundImageHandler.IncreaseBgImageSize();
		}

		public void DecreaseBgImageSize()
		{
			backgroundImageHandler.DecreaseBgImageSize();
		}

		public void ResetBgImageSize()
		{
			backgroundImageHandler.ResetBgImageSize();
		}

		public bool IsFog
		{
			get { return isFog; }
			set
			{
				isFog = value;
				RaisePropertyChanged("IsFog");
				levelObjectHandler.SetBackgroundModel(SelectedBackgroundModel);
				levelObjectHandler.RecreateObjects();
			}
		}

		private bool isFog;

		public string FogDensity
		{
			get { return fogDensity; }
			set
			{
				fogDensity = value;
				RaisePropertyChanged("FogDensity");
			}
		}

		private string fogDensity;

		public string SelectedLevelObject
		{
			get { return selectedLevelObject; }
			set
			{
				selectedLevelObject = value;
				levelCommands.SelectedLevelObject = value;
			}
		}

		private string selectedLevelObject;
	}
}