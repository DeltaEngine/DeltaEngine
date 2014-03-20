using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering3D;
using DeltaEngine.Rendering3D.Shapes;
using DeltaEngine.ScreenSpaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using MessageBox = System.Windows.MessageBox;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.MaterialEditor
{
	public class MaterialEditorViewModel : ViewModelBase
	{
		public MaterialEditorViewModel(Service service)
		{
			this.service = service;
			colorList = new Dictionary<string, Color>();
			ColorStringList = new ObservableCollection<string>();
			MaterialList = new ObservableCollection<string>();
			BlendModeList = new ObservableCollection<string>();
			RenderStyleList = new ObservableCollection<string>();
			MaterialName = "MyMaterial";
			SelectedShader = ShaderFlags.Position2DColoredTextured;
			CanSaveMaterial = false;
			if (NewMaterial == null)
				CreateDefaultMaterial();
			FillLists();
			Messenger.Default.Register<ContentIconAndName>(this, "SelectContentInContentManager",
				SelectContentInContentManager);
		}

		private void CreateDefaultMaterial()
		{
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities(); //ncrunch: no coverage
			Material material = ContentExtensions.CreateDefaultMaterial2D();
			renderExample = new Sprite(material, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			renderExample.IsActive = true;
		}

		public Material NewMaterial { get; set; }
		private readonly Service service;
		public ObservableCollection<ShaderFlags> ShaderList { get; set; }
		public ObservableCollection<string> MaterialList { get; set; }
		public ObservableCollection<string> BlendModeList { get; set; }
		public ObservableCollection<string> RenderStyleList { get; set; }
		private readonly Dictionary<string, Color> colorList;
		public ObservableCollection<string> ColorStringList { get; set; }

		private void FillLists()
		{
			LoadImageList();
			LoadShaders();
			LoadColors();
			LoadMaterials();
			FillListWithBlendModes();
			FillListWithRenderSizeModes();
		}

		private void LoadShaders()
		{
			ShaderList = new ObservableCollection<ShaderFlags>(ShaderFlagExtension.GetSupportedShaders());
		}

		private void LoadImageList()
		{
			ImageList = new ObservableCollection<string>();
			AddContentTypeToContentList(ImageList, ContentType.Image);
			AddContentTypeToContentList(ImageList, ContentType.ImageAnimation);
			AddContentTypeToContentList(ImageList, ContentType.SpriteSheetAnimation);
		}

		public ObservableCollection<string> ImageList { get; set; }

		private void AddContentTypeToContentList(ObservableCollection<string> contentList,
			ContentType type)
		{
			var contentTypeList = service.GetAllContentNamesByType(type);
			foreach (var content in contentTypeList)
				contentList.Add(content);
		}

		private void LoadColors()
		{
			colorList.Add("Black", Color.Black);
			colorList.Add("White", Color.White);
			colorList.Add("Blue", Color.Blue);
			colorList.Add("Cyan", Color.Cyan);
			colorList.Add("Green", Color.Green);
			colorList.Add("Orange", Color.Orange);
			colorList.Add("Pink", Color.Pink);
			colorList.Add("Purple", Color.Purple);
			colorList.Add("Red", Color.Red);
			colorList.Add("Teal", Color.Teal);
			colorList.Add("Yellow", Color.Yellow);
			colorList.Add("CornflowerBlue", Color.CornflowerBlue);
			colorList.Add("LightBlue", Color.LightBlue);
			colorList.Add("LightGray", Color.LightGray);
			colorList.Add("DarkGray", Color.DarkGray);
			colorList.Add("DarkGreen", Color.DarkGreen);
			colorList.Add("Gold", Color.Gold);
			colorList.Add("PaleGreen", Color.PaleGreen);
			FillColorStringList();
		}

		private void FillColorStringList()
		{
			foreach (var color in colorList)
				ColorStringList.Add(color.Key);
			selectedColor = "White";
			CreateNewMaterial();
			RaisePropertyChanged("SelectedColor");
		}

		private void LoadMaterials()
		{
			MaterialList.Clear();
			var foundmaterials = service.GetAllContentNamesByType(ContentType.Material);
			foreach (var material in foundmaterials)
				MaterialList.Add(material);
			RaisePropertyChanged("MaterialList");
		}

		private void FillListWithBlendModes()
		{
			Array enumValues = Enum.GetValues(typeof(BlendMode));
			foreach (var value in enumValues)
				BlendModeList.Add(Enum.GetName(typeof(BlendMode), value));
			selectedBlendMode = BlendMode.Normal.ToString();
			RaisePropertyChanged("SelectedBlendMode");
		}

		private void FillListWithRenderSizeModes()
		{
			Array enumValues = Enum.GetValues(typeof(RenderSizeMode));
			foreach (var value in enumValues)
				RenderStyleList.Add(Enum.GetName(typeof(RenderSizeMode), value));
			selectedRenderSize = RenderSizeMode.PixelBased.ToString();
			RaisePropertyChanged("SelectedRenderSize");
		}

		private void SelectContentInContentManager(ContentIconAndName content)
		{
			if (content.GetContentType() != ContentType.Material)
				return;
			var material = ContentLoader.Load<Material>(content.Name);
			MaterialName = content.Name;
			SelectedImage = material.DiffuseMap.Name;
			SelectedBlendMode =
				material.DiffuseMap.MetaData.Get("BlendMode", BlendMode.Normal).ToString();
			SelectedRenderSize = material.RenderSizeMode.ToString();
			foreach (var color in colorList.Where(color => color.Value == material.DefaultColor))
				SelectedColor = color.Key;
			SelectedShader = material.MetaData.Get("ShaderFlags", ShaderFlags.ColoredTextured);
		}

		public string SelectedRenderSize
		{
			get { return selectedRenderSize; }
			set
			{
				selectedRenderSize = value;
				CreateNewMaterial();
				RaisePropertyChanged("SelectedRenderSize");
			}
		}

		private string selectedRenderSize;

		public string SelectedBlendMode
		{
			get { return selectedBlendMode; }
			set
			{
				selectedBlendMode = value;
				CreateNewMaterial();
				RaisePropertyChanged("SelectedBlendMode");
			}
		}

		private string selectedBlendMode;

		private void CreateNewMaterial()
		{
			if (errorMessage != null)
				errorMessage.IsActive = false; //ncrunch: no coverage
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities(); //ncrunch: no coverage
			try
			{
				TryCreateNewMaterial();
			}
			catch //ncrunch: no coverage start
			{
				CreateDefaultMaterial();
				Logger.Warning("Could not display material due to corrupted content");
				var verdana = ContentLoader.Load<Font>("Verdana12");
				errorMessage = new FontText(verdana,
					"Could not load " + SelectedImage + ". Check if all content is available", Rectangle.One);
			} //ncrunch: no coverage end
		}

		private void TryCreateNewMaterial()
		{
			if (selectedShader == ShaderFlags.None || string.IsNullOrEmpty(selectedColor) ||
				(string.IsNullOrEmpty(selectedImage)))
				return;
			NewMaterial = new Material(selectedShader, selectedImage);
			NewMaterial.DefaultColor = colorList[selectedColor];
			NewMaterial.RenderSizeMode =
				(RenderSizeMode)Enum.Parse(typeof(RenderSizeMode), selectedRenderSize, true);
			NewMaterial.DiffuseMap.BlendMode =
				(BlendMode)Enum.Parse(typeof(BlendMode), selectedBlendMode);
			DrawExample();
		}

		public Entity renderExample;
		private FontText errorMessage;

		public string SelectedImage
		{
			get { return selectedImage; }
			set
			{
				if (renderExample != null)
					renderExample.IsActive = false;
				selectedImage = value;
				MaterialName = selectedImage + "Material";
				CreateNewMaterial();
				UpdateIfCanSave();
				RaisePropertyChanged("SelectedImage");
			}
		}

		private string selectedImage;

		public ShaderFlags SelectedShader
		{
			get { return selectedShader; }
			set
			{
				selectedShader = value;
				CreateNewMaterial();
				RaisePropertyChanged("SelectedShader");
			}
		}

		private ShaderFlags selectedShader;

		public string MaterialName
		{
			get { return materialName; }
			set
			{
				materialName = value;
				UpdateIfCanSave();
				RaisePropertyChanged("MaterialName");
			}
		}

		private string materialName;

		public void Save()
		{
			if (NewMaterial == null || materialName == "")
				return;
			if (materialName == selectedImage) //ncrunch: no coverage start
				if (GetDialogResultToRenameMaterial())
					MaterialName += "Material";
				else
					return;
			if(MaterialList.Contains(MaterialName))
				if (!GetDialogResultToSaveMaterial())
					return;
			var metaDataCreator = new ContentMetaDataCreator();
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromMaterial(materialName,
				NewMaterial);
			service.UploadContent(contentMetaData);
		}

		private bool GetDialogResultToRenameMaterial()
		{
			var dialogResult = (DialogResult)MessageBox.Show(
				"A material must not have the same name as the image attached to it. Would you like to rename the material to: " + materialName + "Material and save it?", "Rename and Save material", MessageBoxButton.YesNo, MessageBoxImage.Question);
			return dialogResult == DialogResult.Yes;
		}

		private static bool GetDialogResultToSaveMaterial()
		{
			var dialogResult = (DialogResult)MessageBox.Show(
				"A material with the same name already exists. The image information will be lost by overwriting the existing material.\r\nDo you want to continue?", "Save material", MessageBoxButton.YesNo, MessageBoxImage.Question);
			return dialogResult == DialogResult.Yes;
		} //ncrunch: no coverage end

		public string SelectedColor
		{
			get { return selectedColor; }
			set
			{
				selectedColor = value;
				CreateNewMaterial();
				RaisePropertyChanged("SelectedColor");
			}
		}

		private string selectedColor;

		public void RefreshOnContentChange()
		{
			ImageList.Clear();
			MaterialList.Clear();
			LoadImageList();
			LoadMaterials();
			RaisePropertyChanged("ImageList");
			RaisePropertyChanged("MaterialList");
		}

		public void RefreshOnAddedContent(ContentType type, string name)
		{
			if (type == ContentType.Material && !MaterialList.Contains(name))
				MaterialList.Add(name);
			if ((type == ContentType.Image || type == ContentType.ImageAnimation ||
				type == ContentType.SpriteSheetAnimation) && !ImageList.Contains(name))
				ImageList.Add(name);
		}

		public void ResetOnProjectChange()
		{
			RefreshOnContentChange();
			materialName = "";
			selectedImage = "";
			selectedShader = ShaderFlags.None;
			RaisePropertyChanged("MaterialName");
			RaisePropertyChanged("SelectedImage");
			RaisePropertyChanged("SelectedShader");
		}

		public void Activate()
		{
			if (renderExample == null)
				return;
			renderExample.IsActive = true;
			if (service.Viewport == null)
				return; //ncrunch: no coverage
			service.Viewport.ZoomViewTo(1.0f); //ncrunch: no coverage
			Messenger.Default.Send("Material", "OpenEditorPlugin");
		}

		public bool CanSaveMaterial
		{
			get { return canSaveMaterial; }
			set
			{
				canSaveMaterial = value;
				RaisePropertyChanged("CanSaveMaterial");
			}
		}

		private bool canSaveMaterial;

		public void UpdateIfCanSave()
		{
			if (NewMaterial == null)
				CanSaveMaterial = false;
			else if (string.IsNullOrEmpty(MaterialName) || string.IsNullOrEmpty(selectedImage))
			{ //ncrunch: no coverage start
				if (NewMaterial.DiffuseMap == null && NewMaterial.Animation == null &&
					NewMaterial.SpriteSheet == null)
					CanSaveMaterial = false;
			} //ncrunch: no coverage end
			else
				CanSaveMaterial = true;
		}

		public void LoadMaterial()
		{
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities(); //ncrunch: no coverage
			if (renderExample != null)
				renderExample.IsActive = false;
			if (ContentLoader.Exists(materialName, ContentType.Material))
				try
				{
					SetComboboxes();
					CreateNewMaterial();
				}
				catch //ncrunch: no coverage start
				{
					CreateDefaultMaterial();
				} //ncrunch: no coverage end
			UpdateIfCanSave();
		}

		private void SetComboboxes()
		{
			var newMaterial = ContentLoader.Load<Material>(materialName);
			selectedBlendMode = newMaterial.DiffuseMap.BlendMode.ToString();
			selectedRenderSize = newMaterial.RenderSizeMode.ToString();
			if (newMaterial.Animation != null)
				selectedImage = newMaterial.Animation.Name; //ncrunch: no coverage
			else if (newMaterial.SpriteSheet != null)
				selectedImage = newMaterial.SpriteSheet.Name;
			else
				selectedImage = newMaterial.DiffuseMap.Name; //ncrunch: no coverage 
			foreach (var colorWithString in colorList)
				if (newMaterial.DefaultColor == colorWithString.Value)
					selectedColor = colorWithString.Key;
			selectedShader = newMaterial.Shader.Flags;
			RaiseAllPropertyChanged();
		}

		private void DrawExample()
		{
			var shaderWithFormat = NewMaterial.Shader as ShaderWithFormat;
			if (shaderWithFormat.Format.Is3D)
				Draw3DExample(shaderWithFormat);
			else
				Draw2DExample(shaderWithFormat);
		}

		private void Draw3DExample(ShaderWithFormat shader)
		{
			new Grid3D(new Size(10));
			if (shader.Format.HasUV)
				renderExample = new Billboard(Vector3D.Zero, Size.One, NewMaterial);
		}

		private void Draw2DExample(ShaderWithFormat shader)
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			if (shader.Format.HasUV)
				renderExample = new Sprite(NewMaterial,
					Rectangle.FromCenter(Vector2D.Half,
						ScreenSpace.Current.FromPixelSpace(NewMaterial.DiffuseMap.PixelSize)));
		}

		private void RaiseAllPropertyChanged()
		{
			RaisePropertyChanged("SelectedImage");
			RaisePropertyChanged("SelectedShader");
			RaisePropertyChanged("SelectedBlendMode");
			RaisePropertyChanged("SelectedRenderSize");
			RaisePropertyChanged("SelectedColor");
		}
	}
}