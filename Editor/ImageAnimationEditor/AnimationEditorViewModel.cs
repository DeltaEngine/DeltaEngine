using System.Collections.Generic;
using System.Collections.ObjectModel;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ImageAnimationEditor
{
	public class AnimationEditorViewModel : ViewModelBase
	{
		public AnimationEditorViewModel(Service service)
		{
			this.service = service;
			metaDataCreator = new ContentMetaDataCreator();
			LoadedImageList = new ObservableCollection<string>();
			ImageList = new ObservableCollection<string>();
			AnimationList = new ObservableCollection<string>();
			Duration = 1;
			LoadImagesFromProject();
			LoadAnimationsFromProject();
			SetMessengers();
			var material = CreateDefaultMaterial2D();
			renderExample = new Sprite(material, new Rectangle(0.25f, 0.25f, 0.5f, 0.5f));
			AnimationName = "MyAnimation";
			UpdateIfCanSave();
			CanSaveAnimation = false;
			SetButtonEnableStates();
			Messenger.Default.Register<ContentIconAndName>(this, "SelectContentInContentManager",
				SelectContentInContentManager);
		}

		private readonly Service service;
		private readonly ContentMetaDataCreator metaDataCreator;
		public ObservableCollection<string> LoadedImageList { get; set; }
		public ObservableCollection<string> ImageList { get; set; }
		public ObservableCollection<string> AnimationList { get; set; }

		private void LoadImagesFromProject()
		{
			var foundContent = service.GetAllContentNamesByType(ContentType.Image);
			foreach (string content in foundContent)
				if (!content.StartsWith("Default") && !content.EndsWith("Font"))
					LoadedImageList.Add(content);
			RaisePropertyChanged("LoadedImageList");
		}

		private void LoadAnimationsFromProject()
		{
			AnimationList.Clear();
			var foundImageAnimtaion = service.GetAllContentNamesByType(ContentType.ImageAnimation);
			var foundSpriteSheetAnimtaion =
				service.GetAllContentNamesByType(ContentType.SpriteSheetAnimation);
			foreach (var imageAnimation in foundImageAnimtaion)
				AnimationList.Add(imageAnimation);
			foreach (var imageAnimation in foundSpriteSheetAnimtaion)
				AnimationList.Add(imageAnimation);
			RaisePropertyChanged("AnimtaionList");
		}

		private void SetMessengers()
		{
			Messenger.Default.Register<int>(this, "RemoveImage", RemoveImage);
			Messenger.Default.Register<int>(this, "MoveImageUp", MoveImageUp);
			Messenger.Default.Register<int>(this, "MoveImageDown", MoveImageDown);
			Messenger.Default.Register<string>(this, "SaveAnimation", SaveAnimation);
			Messenger.Default.Register<string>(this, "AddImage", AddImage);
		}

		private Material CreateDefaultMaterial2D()
		{
			var imageData = new ImageCreationData(new Size(8.0f, 8.0f));
			imageData.DisableLinearFiltering = true;
			var image = ContentLoader.Create<Image>(imageData);
			var colors = new Color[8 * 8];
			for (int i = 0; i < 8; i++)
				for (int j = 0; j < 8; j++)
					if ((i + j) % 2 == 0)
						colors[i * 8 + j] = Color.LightGray;
					else
						colors[i * 8 + j] = Color.DarkGray;
			image.Fill(colors);
			spriteSheetAnimation =
				new SpriteSheetAnimation(new SpriteSheetAnimationCreationData(image, 1, new Size(2, 2)));
			return new Material(ShaderFlags.Position2DTextured, "")
			{
				SpriteSheet = spriteSheetAnimation
			};
		}

		public void RemoveImage(int imageIndex)
		{
			ImageList.RemoveAt(imageIndex);
			SelectedIndex = imageIndex;
			if (imageIndex == ImageList.Count)
				SelectedIndex = imageIndex - 1;
			CreateNewAnimation();
			UpdateIfCanSave();
			SetButtonEnableStates();
			RaisePropertyChanged("ImageList");
		}

		public void MoveImageUp(int imageIndex)
		{
			if (imageIndex == 0)
				return;
			var imageToMove = ImageList[imageIndex];
			ImageList.RemoveAt(imageIndex);
			ImageList.Insert(imageIndex - 1, imageToMove);
			SelectedIndex = imageIndex - 1;
			CreateNewAnimation();
			RaisePropertyChanged("ImageList");
		}

		public void SaveAnimation(string obj)
		{
			if (ImageList.Count == 0 || string.IsNullOrEmpty(AnimationName))
				return;
			ContentMetaData contentMetaData = SaveImageAnimationOrSpriteSheetAnimation();
			service.UploadContent(contentMetaData);
			LoadAnimationsFromProject();
		}

		private ContentMetaData SaveImageAnimationOrSpriteSheetAnimation()
		{
			ContentMetaData contentMetaData;
			if (ImageList.Count == 1)
				contentMetaData = metaDataCreator.CreateMetaDataFromSpriteSheetAnimation(AnimationName,
					spriteSheetAnimation);
			else
				contentMetaData = metaDataCreator.CreateMetaDataFromImageAnimation(AnimationName, animation);
			return contentMetaData;
		}

		public void AddImage(string obj)
		{
			if (selectedImage == null)
				return;
			IsDisplayingAnimation = true;
			ImageList.Add(selectedImage);
			CreateNewAnimation();
			UpdateIfCanSave();
			SetButtonEnableStates();
			RaisePropertyChanged("ImageList");
		}

		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				selectedIndex = value;
				SetButtonEnableStates();
				RaisePropertyChanged("SelectedIndex");
			}
		}

		private int selectedIndex;

		public void MoveImageDown(int imageIndex)
		{
			if (imageIndex == ImageList.Count - 1)
				return;
			var imageToMove = ImageList[imageIndex];
			ImageList.RemoveAt(imageIndex);
			ImageList.Insert(imageIndex + 1, imageToMove);
			SelectedIndex = imageIndex + 1;
			CreateNewAnimation();
			RaisePropertyChanged("ImageList");
		}

		public string SelectedImageToAdd
		{
			set
			{
				selectedImage = value;
				SetButtonEnableStates();
				if (IsDisplayingImage)
					CreateDisplayedImage();
			}
		}

		private string selectedImage;

		private void CreateNewAnimation()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			if (ImageList.Count == 1)
				ShowSpritesheetAnimation();
			else if (ImageList.Count > 1)
				ShowMultipleImageAnimation();
		}

		public void ShowSpritesheetAnimation()
		{
			var image = ContentLoader.Load<Image>(ImageList[0]);
			if (SubImageSize.Width == 0 || SubImageSize.Height == 0)
			{
				subImageSize = image.PixelSize;
				RaisePropertyChanged("SubImageSize");
			}
			spriteSheetAnimation =
				new SpriteSheetAnimation(new SpriteSheetAnimationCreationData(image, Duration, SubImageSize));
			var material = new Material(ShaderFlags.Position2DTextured, "")
			{
				SpriteSheet = spriteSheetAnimation
			};
			renderExample = new Sprite(material,
				Rectangle.FromCenter(0.5f, 0.5f, 0.5f * SubImageSize.AspectRatio, 0.5f));
		}

		private SpriteSheetAnimation spriteSheetAnimation;

		public float Duration
		{
			get { return duration; }
			set
			{
				duration = value;
				CreateNewAnimation();
				RaisePropertyChanged("Duration");
			}
		}

		private float duration;

		public Size SubImageSize
		{
			get { return subImageSize; }
			set
			{
				if (value.Width < 0 || value.Height < 0)
					return;
				subImageSize = value;
				if (value.Width > spriteSheetAnimation.Image.PixelSize.Width)
					subImageSize.Width = spriteSheetAnimation.Image.PixelSize.Width;
				if (value.Height > spriteSheetAnimation.Image.PixelSize.Height)
					subImageSize.Height = spriteSheetAnimation.Image.PixelSize.Height;
				CreateNewAnimation();
				RaisePropertyChanged("SubImageSize");
			}
		}

		private Size subImageSize;

		private void ShowMultipleImageAnimation()
		{
			var imagelist = new List<Image>();
			foreach (var image in ImageList)
				imagelist.Add(ContentLoader.Load<Image>(image));
			animation = new ImageAnimation(imagelist.ToArray(), Duration);
			renderExample =
				new Sprite(new Material(ShaderFlags.Position2DTextured, "") { Animation = animation },
					Rectangle.FromCenter(0.5f, 0.5f, 0.5f * imagelist[0].PixelSize.AspectRatio, 0.5f));
		}

		private ImageAnimation animation;
		public Entity2D renderExample;

		public string AnimationName
		{
			get { return animationName; }
			set
			{
				animationName = value;
				UpdateIfCanSave();
				if (ContentLoader.Exists(animationName, ContentType.ImageAnimation) ||
					ContentLoader.Exists(animationName, ContentType.SpriteSheetAnimation))
					CreateAnimationFromFile();
				RaisePropertyChanged("AnimationName");
			}
		}

		private string animationName;

		public void CreateAnimationFromFile()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			ImageList.Clear();
			Material material;
			try
			{
				material = new Material(ShaderFlags.Position2DTextured, animationName);
				CreateNewRendereExample(material);
				if (material.Animation != null)
				{
					foreach (var image in material.Animation.Frames)
						ImageList.Add(image.Name);
					Duration = material.Animation.DefaultDuration;
				}
				else
				{
					ImageList.Add(material.SpriteSheet.Image.Name);
					Duration = material.SpriteSheet.DefaultDuration;
					SubImageSize = material.SpriteSheet.SubImageSize;
				}
				SetButtonEnableStates();
			}
				//ncrunch: no coverage start
			catch
			{
				Logger.Warning(
					"Some of this animation's images were missing. Make sure all the content of your project is present.");
				material = CreateDefaultMaterial2D();
				CreateNewRendereExample(material);
			} //ncrunch: no coverage end
		}

		private void CreateNewRendereExample(Material material)
		{
			renderExample = new Sprite(material,
				Rectangle.FromCenter(0.5f, 0.5f, 0.5f * material.MaterialRenderSize.AspectRatio, 0.5f));
		}

		public bool IsDisplayingImage
		{
			get { return isDisplayingImage; }
			set
			{
				isDisplayingImage = value;
				ChangeDisplay();
				RaisePropertyChanged("IsDisplayingImage");
			}
		}

		private bool isDisplayingImage;

		private void ChangeDisplay()
		{
			if (isDisplayingImage)
				CreateDisplayedImage();
			else
				CreateNewAnimation();
		}

		private void CreateDisplayedImage()
		{
			if (renderExample != null)
				renderExample.IsActive = false;
			renderExample =
				new Sprite(new Material(ShaderFlags.Position2DColoredTextured, selectedImage),
					Rectangle.HalfCentered);
		}

		public bool IsDisplayingAnimation
		{
			get { return !isDisplayingImage; }
			set
			{
				isDisplayingImage = !value;
				ChangeDisplay();
				RaisePropertyChanged("IsDisplayingAnimation");
			}
		}

		public void ResetOnProjectChange()
		{
			RefreshOnContentChange();
		}

		public void RefreshOnContentChange()
		{
			LoadedImageList.Clear();
			ImageList.Clear();
			LoadImagesFromProject();
			LoadAnimationsFromProject();
			CreateDefaultMaterial2D();
			RaisePropertyChanged("ImageList");
		}

		public void ActivateAnimation()
		{
			renderExample.IsActive = true;
			if (service.Viewport == null)
				return;
			//ncrunch: no coverage start 
			service.Viewport.CenterViewOn(renderExample.Center);
			service.Viewport.ZoomViewTo(1.0f);
			Messenger.Default.Send("ImageAnimation", "OpenEditorPlugin");
			//ncrunch: no coverage end
		}

		public bool CanSaveAnimation
		{
			get { return canSaveAnimation; }
			set
			{
				canSaveAnimation = value;
				RaisePropertyChanged("CanSaveAnimation");
			}
		}

		private bool canSaveAnimation;

		private void UpdateIfCanSave()
		{
			CanSaveAnimation = !string.IsNullOrEmpty(AnimationName) && ImageList.Count != 0;
		}

		private void SetButtonEnableStates()
		{
			IsFrameSizeEnabled = ImageList.Count <= 1;
			IsRemoveEnabled = SelectedIndex > -1 && ImageList.Count > 0;
			IsMovingEnabled = SelectedIndex > -1 && ImageList.Count > 1;
			IsAddEnabled = selectedImage != null;
		}

		private void SelectContentInContentManager(ContentIconAndName content)
		{
			var type = service.GetTypeOfContent(content.Name);
			if (type == ContentType.ImageAnimation)
			{
				var newAnimation = ContentLoader.Load<ImageAnimation>(content.Name);
				AnimationName = content.Name;
				var imageNames = newAnimation.MetaData.Get("ImageNames", "").SplitAndTrim(',');
				ImageList.Clear();
				foreach (var imageName in imageNames)
					ImageList.Add(imageName);
				Duration = float.Parse(newAnimation.MetaData.Get("DefaultDuration", ""));
			}
			if (type == ContentType.SpriteSheetAnimation)
			{
				var spritesheet = ContentLoader.Load<SpriteSheetAnimation>(content.Name);
				AnimationName = content.Name;
				var imageNames = spritesheet.MetaData.Get("ImageName", "").SplitAndTrim(',');
				ImageList.Clear();
				foreach (var imageName in imageNames)
					ImageList.Add(imageName);
				Duration = float.Parse(animation.MetaData.Get("DefaultDuration", ""));
				SubImageSize = spritesheet.MetaData.Get("SubImageSize", Size.Zero);
			}
		}

		public bool IsFrameSizeEnabled
		{
			get { return isFrameSizeEnabled; }
			set
			{
				isFrameSizeEnabled = value;
				RaisePropertyChanged("IsFrameSizeEnabled");
			}
		}

		private bool isFrameSizeEnabled;

		public bool IsRemoveEnabled
		{
			get { return isRemoveEnabled; }
			set
			{
				isRemoveEnabled = value;
				RaisePropertyChanged("IsRemoveEnabled");
			}
		}

		private bool isRemoveEnabled;

		public bool IsMovingEnabled
		{
			get { return isMovingEnabled; }
			set
			{
				isMovingEnabled = value;
				RaisePropertyChanged("IsMovingEnabled");
			}
		}

		private bool isMovingEnabled;

		public bool IsAddEnabled
		{
			get { return isAddEnabled; }
			set
			{
				isAddEnabled = value;
				RaisePropertyChanged("IsAddEnabled");
			}
		}

		private bool isAddEnabled;
	}
}