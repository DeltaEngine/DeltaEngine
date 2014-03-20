using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.MaterialEditor.Tests
{
	public class MaterialEditorViewModelTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			materialEditor = new MockMaterialEditorViewModel();
			mockService = new MockService("Test", "Test");
			viewModel = new MaterialEditorViewModel(mockService);
		}

		private MockMaterialEditorViewModel materialEditor;

		private class MockMaterialEditorViewModel : MaterialEditorViewModel
		{
			public MockMaterialEditorViewModel()
				: base(new MockService("Test", "Test")) {}

			protected override void RaisePropertyChanged(string propertyName)
			{
				LastPropertyChanged = propertyName;
			}

			public string LastPropertyChanged { get; private set; }
		}

		[Test]
		public void GetDefaultVariables()
		{
			Assert.AreEqual("PixelBased", materialEditor.SelectedRenderSize);
			Assert.AreEqual("Normal", materialEditor.SelectedBlendMode);
		}

		[Test]
		public void ChangeBlendModeAndRenderSize()
		{
			materialEditor.SelectedRenderSize = RenderSizeMode.SizeFor1024X768.ToString();
			materialEditor.SelectedBlendMode = BlendMode.Opaque.ToString();
			Assert.AreEqual("SizeFor1024X768", materialEditor.SelectedRenderSize);
			Assert.AreEqual("Opaque", materialEditor.SelectedBlendMode);
		}

		[Test, Ignore]
		public void SaveMaterialFromImage()
		{
			viewModel.SelectedImage = "DeltaEngineLogo";
			viewModel.MaterialName = "NewMaterial";
			viewModel.Save();
			Assert.AreEqual(1, mockService.NumberOfMessagesSent);
		}

		private MockService mockService;
		private MaterialEditorViewModel viewModel;

		[Test]
		public void AddMaterialToMaterialList()
		{
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.Material, "NewMaterial");
			Assert.IsFalse(materialEditor.CanSaveMaterial);
			Assert.AreEqual(3, materialEditor.MaterialList.Count);
		}

		[Test]
		public void AddSpriteSheetToSpriteSheetList()
		{
			Assert.AreEqual(6, materialEditor.ImageList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.SpriteSheetAnimation, "NewSpriteSheet");
			Assert.AreEqual(7, materialEditor.ImageList.Count);
		}

		[Test]
		public void AddShaderToShaderList()
		{
			Assert.AreEqual(9, materialEditor.ShaderList.Count);
		}

		[Test]
		public void LoadInMaterialWithSpriteSheetAnimation()
		{
			materialEditor.MaterialName = "NewMaterialSpriteSheetAnimation";
			materialEditor.LoadMaterial();
			Assert.IsNotNull(materialEditor.NewMaterial);
			Assert.AreEqual(new Size(107, 80), materialEditor.NewMaterial.SpriteSheet.SubImageSize);
		}

		[Test]
		public void ResetListWhenChangingProject()
		{
			materialEditor.Activate();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.RefreshOnAddedContent(ContentType.Material, "NewMaterial");
			Assert.AreEqual(3, materialEditor.MaterialList.Count);
			materialEditor.ResetOnProjectChange();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
			materialEditor.renderExample = null;
			materialEditor.Activate();
			Assert.AreEqual(2, materialEditor.MaterialList.Count);
		}

		[Test]
		public void SelectedImageShouldRaisePropertyChanged()
		{
			materialEditor.SelectedImage = "MyImage";
			Assert.AreEqual("SelectedImage", materialEditor.LastPropertyChanged);
			Assert.AreEqual("MyImage", materialEditor.SelectedImage);
		}

		[Test]
		public void SelectedShaderShouldRaisePropertyChange()
		{
			materialEditor.SelectedShader = ShaderFlags.Position2DTextured;
			Assert.AreEqual("SelectedShader", materialEditor.LastPropertyChanged);
			Assert.AreEqual(ShaderFlags.Position2DTextured, materialEditor.SelectedShader);
		}

		[Test]
		public void SelectedColorShouldRaisePropertyChanged()
		{
			materialEditor.SelectedColor = "Black";
			Assert.AreEqual("SelectedColor", materialEditor.LastPropertyChanged);
			Assert.AreEqual("Black", materialEditor.SelectedColor);
		}

		[Test]
		public void EmptyMaterialShouldReturnWhenSaving()
		{
			viewModel.MaterialName = "";
			viewModel.Save();
			Assert.AreEqual(0, mockService.NumberOfMessagesSent);
		}

		[Test]
		public void New3D()
		{
			materialEditor.SelectedShader = ShaderFlags.Textured;
			materialEditor.MaterialName = "MyMaterial";
			materialEditor.SelectedImage = "DeltaEngineLogo";
		}
	}
}