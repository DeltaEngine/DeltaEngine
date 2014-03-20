using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class LevelEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			new Line2D(Vector2D.Zero, Vector2D.One, Color.Red);
			viewModel = new MockLevelEditorViewModel();
		}

		private MockLevelEditorViewModel viewModel;

		private class MockLevelEditorViewModel : LevelEditorViewModel
		{
			public MockLevelEditorViewModel()
				: base(new MockService("Test", "Test")) {}

			protected override void RaisePropertyChanged(string propertyName)
			{
				LastPropertyChanged = propertyName;
			}

			public string LastPropertyChanged { get; private set; }
		}

		[Test, CloseAfterFirstFrame]
		public void CreateAndRemoveWave()
		{
			viewModel.SelectedWave = null;
			Assert.IsNull(viewModel.SelectedWave);
			AddWave();
			viewModel.xmlSaver.Level = viewModel.Level;
			viewModel.ContentName = "TestLevel";
			viewModel.xmlSaver.SaveToServer();
			Assert.AreEqual(1, viewModel.Level.Waves.Count);
			viewModel.SelectedWave = viewModel.Level.Waves[0];
			viewModel.RemoveSelectedWave();
			Assert.AreEqual(0, viewModel.Level.Waves.Count);
		}

		private void AddWave()
		{
			viewModel.MaxTime = 1.0f;
			viewModel.SpawnInterval = 1.0f;
			viewModel.SpawnTypeList = "Cloth";
			viewModel.WaveName = "CreepWave";
			viewModel.WaitTime = 1.0f;
			viewModel.AddWave();
		}

		[Test, CloseAfterFirstFrame]
		public void CannotOpenNonExistingLevelFile()
		{
			Assert.Throws(typeof(XmlSaver.CannotOpenLevelFile),
				() => viewModel.xmlSaver.OpenXmlFile());
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedImageRaisesPropertyChange()
		{
			viewModel.SelectedBackgroundImage = "MyImage";
			Assert.AreEqual("MyImage", viewModel.SelectedBackgroundImage);
			Assert.AreEqual("SelectedImage", viewModel.LastPropertyChanged);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedModelRaisesPropertyChange()
		{
			viewModel.IsFog = false;
			viewModel.SelectedBackgroundModel = "MyModel";
			Assert.AreEqual("MyModel", viewModel.SelectedBackgroundModel);
			Assert.AreEqual("SelectedModel", viewModel.LastPropertyChanged);
		}

		[Test, CloseAfterFirstFrame]
		public void SetBackgroundImageIn2DMode()
		{
			viewModel.BackgroundImages.Add("DeltaEngineLogo");
			viewModel.SelectedBackgroundImage = "DeltaEngineLogo";
			Assert.AreEqual("DeltaEngineLogo", viewModel.SelectedBackgroundImage);
		}

		[Test, CloseAfterFirstFrame]
		public void SetBackgroundImageIn3DMode()
		{
			viewModel.Is3D = true;
			viewModel.BackgroundImages.Add("DeltaEngineLogo");
			viewModel.SelectedBackgroundImage = "DeltaEngineLogo";
			Assert.AreEqual("DeltaEngineLogo", viewModel.SelectedBackgroundImage);
		}

		[Test, CloseAfterFirstFrame]
		public void GetAndSetWaveName()
		{
			viewModel.WaveName = "MyWave";
			Assert.AreEqual("MyWave", viewModel.WaveName);
		}

		[Test, CloseAfterFirstFrame]
		public void SetWavePropertiesToSelectedWave()
		{
			viewModel.WaveName = "CreepWave";
			AddWave();
			viewModel.SetWaveProperties();
			Assert.AreEqual("CreepWave", viewModel.SelectedWave.ShortName);
			Assert.AreEqual(1, viewModel.SelectedWave.SpawnInterval);
			Assert.AreEqual(1, viewModel.SelectedWave.WaitTime);
		}

		[Test, CloseAfterFirstFrame]
		public void SetCameraToNewPosition()
		{
			viewModel.CameraPosition = "5, 4, 3";
			Assert.AreEqual("CameraPosition", viewModel.LastPropertyChanged);
			Assert.AreEqual("5, 4, 3", viewModel.CameraPosition);
		}

		[Test, CloseAfterFirstFrame]
		public void RemoveAWaveFromALevel()
		{
			AddWave();
			viewModel.SelectedWave = viewModel.Level.Waves[0];
			viewModel.RemoveSelectedWave();
			Assert.AreEqual(0, viewModel.Level.Waves.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void AddAWaveWithDefaultName()
		{
			AddWave();
			Assert.AreEqual("CreepWave", viewModel.Level.Waves[0].ShortName);
		}

		[Test, CloseAfterFirstFrame]
		public void SelectedTileTypeRaisesPropertyChanged()
		{
			viewModel.SelectedTileType = LevelTileType.NoSelection;
			Assert.AreEqual("SelectedTileType", viewModel.LastPropertyChanged);
		}

		[Test, CloseAfterFirstFrame]
		public void ResetViewSetsDefaultValues()
		{
			viewModel.ResetLevelEditor();
			Assert.AreEqual("MyNewLevel", viewModel.ContentName);
			Assert.AreEqual("12, 12", viewModel.CustomSize);
			Assert.AreEqual(LevelTileType.Nothing, viewModel.SelectedTileType);
		}

		[Test, CloseAfterFirstFrame]
		public void CustomSizeChangesLevelSize()
		{
			viewModel.CustomSize = "8, 12";
			Assert.AreEqual(8.0f, viewModel.Level.Size.Width);
			Assert.AreEqual(12.0f, viewModel.Level.Size.Height);
		}

		[Test, CloseAfterFirstFrame]
		public void InvalidCustomSizeChangesLevelSizeToLastSize()
		{
			viewModel.CustomSize = "8,";
			Assert.AreEqual(12.0f, viewModel.Level.Size.Width);
			Assert.AreEqual(12.0f, viewModel.Level.Size.Height);
			viewModel.CustomSize = ", 5";
			Assert.AreEqual(12.0f, viewModel.Level.Size.Width);
			Assert.AreEqual(12.0f, viewModel.Level.Size.Height);
			viewModel.CustomSize = "3 9";
			Assert.AreEqual(12.0f, viewModel.Level.Size.Width);
			Assert.AreEqual(12.0f, viewModel.Level.Size.Height);
		}

		[Test, CloseAfterFirstFrame]
		public void ClearLevelSetsAllTilesToNothing()
		{
			viewModel.Level.SetTileWithScreenPosition(new Vector2D(0.2f, 0.3f), LevelTileType.SpawnPoint);
			viewModel.Level.SetTileWithScreenPosition(new Vector2D(0.4f, 0.6f), LevelTileType.ExitPoint);
			viewModel.ClearLevel();
			Assert.IsTrue(AreAllTilesSetToNothing());
		}

		private bool AreAllTilesSetToNothing()
		{
			return viewModel.Level.GetAllTilesOfType(LevelTileType.Nothing).Count ==
				viewModel.Level.Size.Width * viewModel.Level.Size.Height;
		}

		[Test]
		public void SetsSavedBackgroundModelFromLevel()
		{
			viewModel.ContentName = "Level1";
			viewModel.Level.ModelName = "LevelBackgroundModel";
			viewModel.SaveToServer();
			viewModel.ResetLevelEditor();
			viewModel.ContentName = "Level1";
			viewModel.Is3D = true;
			Assert.AreEqual("LevelBackgroundModel", viewModel.SelectedBackgroundModel);
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void ChangeLevelNameAndSize()
		{
			viewModel.ContentName = "Test1";
			viewModel.CustomSize = "{20}";
			Assert.AreEqual("12, 12", viewModel.CustomSize);
			viewModel.CustomSize = "{20, 20}";
			Assert.AreEqual("20, 20", viewModel.CustomSize);
		}

		[Test, Ignore]
		public void BuildXmlData()
		{
			viewModel.CustomSize = "{20, 20}";
			viewModel.xmlSaver.Level = viewModel.Level;
			viewModel.Level.SetTileWithScreenPosition(new Vector2D(0.2f, 0.3f), LevelTileType.SpawnPoint);
			viewModel.Level.SetTileWithScreenPosition(new Vector2D(0.4f, 0.6f), LevelTileType.ExitPoint);
			AddWave();
			var xml = viewModel.xmlSaver.BuildXmlData();
			Assert.AreEqual("Level", xml.Name);
			Assert.AreEqual(2, xml.Attributes.Count);
			Assert.AreEqual(4, xml.Children.Count);
			Assert.AreEqual("SpawnPoint", xml.Children[0].Name);
			Assert.AreEqual("ExitPoint", xml.Children[1].Name);
			Assert.AreEqual("Map", xml.Children[2].Name);
			Assert.AreEqual("Wave", xml.Children[3].Name);
		}

		[Test, Ignore]
		public void ChangeATileType()
		{
			viewModel.Is3D = false;
			viewModel.CustomSize = "{8, 8}";
			viewModel.SelectedTileType = LevelTileType.Blocked;
			viewModel.levelCommands.LeftMouseButton(new Vector2D(0.5f, 0.5f));
			Assert.AreEqual(1, viewModel.Level.GetAllTilesOfType(LevelTileType.Blocked).Count);
			viewModel.levelCommands.SetTileToNothingAndRemoveLevelObject(new Vector2D(0.5f, 0.5f));
			Assert.AreEqual(0, viewModel.Level.GetAllTilesOfType(LevelTileType.Blocked).Count);
		}

		[Test, Ignore]
		public void ResetOnProjectChange()
		{
			viewModel.ResetLevelEditor();
			Assert.AreEqual(2, viewModel.BackgroundImages.Count);
		}

		[Test, Ignore]
		public void ImageSizeIncreases()
		{
			viewModel.SelectedBackgroundImage = "DeltaEngineLogo";
			viewModel.IncreaseBgImageSize();
		}

		[Test, Ignore]
		public void ImageSizeDecreases()
		{
			viewModel.SelectedBackgroundImage = "DeltaEngineLogo";
			viewModel.DecreaseBgImageSize();
		}

		[Test, Ignore]
		public void ImageSizeResets()
		{
			viewModel.SelectedBackgroundImage = "DeltaEngineLogo";
			viewModel.ResetBgImageSize();
		}
	}
}