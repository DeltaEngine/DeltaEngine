using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Editor.Core;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Shapes;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// passes the data used in the ContentManagerView
	/// </summary>
	public sealed class ContentManagerViewModel : ViewModelBase
	{
		public ContentManagerViewModel(Service service)
		{
			this.service = service;
			SetMessenger();
			DisplayContentList = new ObservableCollection<ContentIconAndName>();
			ContentList = new ObservableCollection<ContentIconAndName>();
			filterList = new List<ContentType>();
			StartPreviewList = new List<Entity2D>();
			SelectedContentList = new List<string>();
			new Command(CheckMousePosition).Add(new MouseButtonTrigger());
			ShowingContentManager = true;
		}

		public void CheckMousePosition(Vector2D position)
		{
			if (!isShowingStartContent || !ShowingContentManager)
				return;
			foreach (var entity2D in StartPreviewList)
				if (entity2D.DrawArea.Contains(position))
					foreach (var content in DisplayContentList)
						if (entity2D.Get<Material>().Animation != null &&
							content.Name == entity2D.Get<Material>().Animation.Name)
							SelectedContent = content;
						else if (content.Name == entity2D.Get<Material>().DiffuseMap.Name)
							SelectedContent = content;
			if (selectedContent != null)
				isShowingStartContent = false;
		}

		private readonly Service service;
		public readonly List<Entity2D> StartPreviewList;
		public readonly List<string> SelectedContentList;
		private bool isShowingStartContent;

		private void SetMessenger()
		{
			Messenger.Default.Register<string>(this, "DeleteContent", DeleteContentFromList);
			Messenger.Default.Register<string>(this, "AddToSelection", AddContentToSelection);
			Messenger.Default.Register<string>(this, "SelectToDelete", SelectToDelete);
			Messenger.Default.Register<string>(this, "AddMultipleContentToSelection",
				AddMultipleContentToSelection);
			Messenger.Default.Register<string>(this, "ClearList", ClearSelectionList);
			Messenger.Default.Register<string>(this, "OpenFileExplorerToAddNewContent",
				OpenFileExplorerToAddNewContent);
		}

		public void DeleteContentFromList(string contentList)
		{
			foreach (var contentName in SelectedContentList)
			{
				service.DeleteContent(contentName);
				RemoveContentFromContentLists(contentName);
			}
			FilterContentList();
			ClearEntities();
		}

		private void RemoveContentFromContentLists(string contentName)
		{
			for (int index = 0; index < DisplayContentList.Count; index++)
			{
				var content = DisplayContentList[index];
				if (content.Name == contentName)
				{
					DisplayContentList.Remove(content);
					ContentList.Remove(content);
				}
			}
		}

		public void DeleteContentWithSubContent()
		{
			for (int index = 0; index < SelectedContentList.Count; index++)
			{
				var contentName = SelectedContentList[index];
				var content = DisplayContentList.ToList().Where(c => c.Name == contentName);
				var contentIconAndNames = content as IList<ContentIconAndName> ?? content.ToList();
				if (contentIconAndNames.Any())
					ContentList = contentIconAndNames.ElementAt(0).DeleteSubContent(service, ContentList);
				RemoveContentFromContentLists(contentName);
			}
			ClearEntities();
			FilterContentList();
		}

		public void AddContentToSelection(string contentName)
		{
			if (SelectedContentList.Contains(contentName))
				foreach (var contentIconAndName in DisplayContentList)
					if (contentIconAndName.Name == contentName)
					{
						contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
						SelectedContentList.Remove(contentName);
						return;
					}
			foreach (var contentIconAndName in DisplayContentList)
				if (contentIconAndName.Name == contentName)
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			SelectedContentList.Add(contentName);
			lastSelectedContent = contentName;
			RaisePropertyChanged("ContentList");
		}

		public void SelectToDelete(string contentName)
		{
			if (SelectedContentList.Contains(contentName))
				return;
			SelectedContentList.Clear();
			foreach (var contentIconAndName in DisplayContentList)
				if (contentIconAndName.Name == contentName)
				{
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
					SelectedContentList.Add(contentName);
					lastSelectedContent = contentName;
				}
				else
				{
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
					SelectedContentList.Remove(contentName);
				}
			RaisePropertyChanged("ContentList");
		}

		public void AddMultipleContentToSelection(string contentName)
		{
			foreach (var contentIconAndName in DisplayContentList)
				if (contentIconAndName.Name == contentName)
				{
					if (SelectedContentList.Count != 0)
						GetContentBetweenLastAndNewContent(contentName);
					contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
					SelectedContentList.Add(contentName);
					lastSelectedContent = contentName;
				}
		}

		private void GetContentBetweenLastAndNewContent(string contentName)
		{
			int indexOfLastContent =
				DisplayContentList.IndexOf(
					DisplayContentList.FirstOrDefault(content => content.Name == lastSelectedContent));
			var indexOfNewContent =
				DisplayContentList.IndexOf(
					DisplayContentList.FirstOrDefault(content => content.Name == contentName));
			if (indexOfLastContent < indexOfNewContent)
				for (int i = 0; indexOfLastContent + i < indexOfNewContent; i++)
					SelectNewContent(indexOfLastContent, i);
			else if (indexOfNewContent < indexOfLastContent)
				for (int i = 0; indexOfNewContent + i < indexOfLastContent; i++)
					SelectNewContent(indexOfNewContent, i);
		}

		private void SelectNewContent(int indexOfLastContent, int i)
		{
			DisplayContentList[indexOfLastContent + i].Brush =
				new SolidColorBrush(Color.FromArgb(255, 195, 195, 195));
			SelectedContentList.Add(DisplayContentList[indexOfLastContent + i].Name);
		}

		private string lastSelectedContent;

		public void ClearSelectionList(string obj)
		{
			SelectedContentList.Clear();
			foreach (var contentIconAndName in DisplayContentList)
				contentIconAndName.Brush = new SolidColorBrush(Color.FromArgb(0, 175, 175, 175));
			RaisePropertyChanged("ContentList");
		}

		//ncrunch: no coverage start
		public void OpenFileExplorerToAddNewContent(string obj)
		{
			var dialog = new OpenFileDialog { Multiselect = true };
			if ((bool)dialog.ShowDialog(Application.Current.MainWindow))
				service.UploadContentFilesToService(dialog.FileNames);
		} //ncrunch: no coverage end

		public bool IsAnimation
		{
			get
			{
				if (selectedContent == null)
					return false;
				var contentType = service.GetTypeOfContent(selectedContent.Name);
				return contentType == ContentType.ImageAnimation ||
					contentType == ContentType.SpriteSheetAnimation || contentType == ContentType.Material;
			}
		}

		public void RefreshContentList()
		{
			if (!IsLoggedInAlready())
				return;
			DisplayContentList.Clear();
			ContentList.Clear();
			var foundContent = service.GetAllContentNames();
			foreach (var contentName in foundContent)
				AddNewContent(contentName);
			FilterContentList();
			RaisePropertyChanged("DisplayContentList");
			isShowingStartContent = true;
		}

		private bool IsLoggedInAlready()
		{
			return !string.IsNullOrEmpty(service.UserName);
		}

		public void AddNewContent(string contentName)
		{
			var contentType = service.GetTypeOfContent(contentName);
			var newContent = new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(contentType),
				contentName);
			ContentList.Add(newContent);
		}

		public ObservableCollection<ContentIconAndName> DisplayContentList { get; set; }
		public ObservableCollection<ContentIconAndName> ContentList { get; set; }

		public void ShowStartContent()
		{
			if (!ShowingContentManager)
				return;
			ClearEntities();
			int row = 0;
			int column = 0;
			if (service.Viewport != null)
				service.Viewport.CenterViewOn(Vector2D.Half); //ncrunch: no coverage
			foreach (var content in DisplayContentList)
				if (service.GetTypeOfContent(content.Name) == ContentType.Image ||
					service.GetTypeOfContent(content.Name) == ContentType.SpriteSheetAnimation ||
					service.GetTypeOfContent(content.Name) == ContentType.ImageAnimation)
				{
					if (content.Name.Contains("Font"))
						continue;
					Entity2D entity = null;
					try
					{
						entity = TryCreateEntityAndSetDrawAreaPosition(content, row, column);
						if (UpdateRowAndColumn(ref column, ref row))
							return;
					} //ncrunch: no coverage start
					catch (Exception)
					{
						if (entity != null)
							entity.IsActive = false;
					} //ncrunch: no coverage end
				}
				else if (service.GetTypeOfContent(content.Name) == ContentType.Material)
					try
					{
						var canUpdate = TryLoadMaterialCreateSpriteAndSetDrawAreaPosition(content, row, column);
						if (canUpdate && UpdateRowAndColumn(ref column, ref row))
							return;
					}
					catch (Exception) {} //ncrunch: no coverage
		}

		private Entity2D TryCreateEntityAndSetDrawAreaPosition(ContentIconAndName content, int row,
			int column)
		{
			var entity = new Sprite(content.Name, new Rectangle(0, 0, 1, 1));
			SetDrawAreaPosition(entity, row, column);
			return entity;
		}

		private bool TryLoadMaterialCreateSpriteAndSetDrawAreaPosition(ContentIconAndName content,
			int row, int column)
		{
			var material = ContentLoader.Load<Material>(content.Name);
			if ((material.Shader as ShaderWithFormat).Format.Is3D)
				return false;
			var sprite = new Sprite(material, new Rectangle(0, 0, 1, 1));
			SetDrawAreaPosition(sprite, row, column);
			return true;
		}

		public bool ShowingContentManager;

		private static bool UpdateRowAndColumn(ref int column, ref int row)
		{
			column++;
			if (column > MaxRows)
			{
				column = 0;
				row++;
			}
			return row == MaxColumns;
		}

		private const int MaxRows = 3;
		private const int MaxColumns = 3;

		private void SetDrawAreaPosition(Entity2D entity, int row, int column)
		{
			Size size = entity.Get<Material>().DiffuseMap != null
				? entity.Get<Material>().DiffuseMap.PixelSize
				: entity.Get<Material>().Animation.Frames[0].PixelSize;
			float widthScale = size.Width > size.Height ? 1.0f : size.Width / size.Height;
			float heightScale = size.Width > size.Height ? size.Height / size.Width : 1.0f;
			entity.DrawArea =
				Rectangle.FromCenter(new Vector2D(0.175f + 0.2f * column, 0.3f + 0.2f * row),
					new Size(0.15f * widthScale, 0.15f * heightScale));
			CreateOutline(row, column);
			StartPreviewList.Add(entity);
		}

		private static void CreateOutline(int row, int column)
		{
			new Line2D(new Vector2D(0.09f + 0.2f * column, 0.215f + 0.2f * row),
				new Vector2D(0.15f + 0.11f + 0.2f * column, 0.215f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.09f + 0.2f * column, 0.215f + 0.2f * row),
				new Vector2D(0.09f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.15f + 0.11f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Vector2D(0.15f + 0.11f + 0.2f * column, 0.215f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
			new Line2D(new Vector2D(0.15f + 0.11f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Vector2D(0.09f + 0.2f * column, 0.125f + 0.26f + 0.2f * row),
				new Datatypes.Color(67, 78, 200));
		}

		public Object SelectedContent
		{
			get { return selectedContent; }
			set
			{
				isShowingStartContent = false;
				selectedContent = (ContentIconAndName)value;
				ClearEntities();
				CreatePreviewerForSelectedContent();
				DrawBackground();
				RaisePropertyChanged("IsAnimation");
			}
		}

		private ContentIconAndName selectedContent;

		private void CreatePreviewerForSelectedContent()
		{
			if (selectedContent == null)
				return;
			var type = service.GetTypeOfContent(selectedContent.Name);
			if (type == null)
				return;
			try
			{
				TryCreatePreviewerForSelectedContent(type);
				Messenger.Default.Send(selectedContent, "SelectContentInContentManager");
			} //ncrunch: no coverage start
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			//ncrunch: no coverage end
			if (service.Viewport == null)
				return;
			//ncrunch: no coverage start
			service.Viewport.CenterViewOn(Vector2D.Half);
			service.Viewport.ZoomViewTo(1.0f);
			//ncrunch: no coverage end
		}

		private void TryCreatePreviewerForSelectedContent(ContentType? type)
		{
			contentViewer.View(selectedContent.Name, (ContentType)type);
			CanDeleteSubContent = (ContentType)type == ContentType.ImageAnimation ||
				(ContentType)type == ContentType.SpriteSheetAnimation;
		}

		private readonly ContentViewer contentViewer = new ContentViewer();

		public string SelectedBackgroundImage
		{
			get { return selectedBackgroundImage; }
			set
			{
				selectedBackgroundImage = value;
				ClearEntities();
				CreatePreviewerForSelectedContent();
				DrawBackground();
			}
		}

		private string selectedBackgroundImage;

		private void ClearEntities()
		{
			if (service.Viewport != null)
				service.Viewport.DestroyRenderedEntities(); //ncrunch: no coverage
		}

		private void DrawBackground()
		{
			if (selectedBackgroundImage == null || selectedBackgroundImage == "None")
				return;
			var background =
				new Sprite(new Material(ShaderFlags.Position2DTextured, selectedBackgroundImage),
					new Rectangle(Vector2D.Zero, Size.One));
			background.RenderLayer = -100;
		}

		public string SearchText
		{
			get { return searchText; }
			set
			{
				searchText = value;
				RefreshContentList();
			}
		}

		private string searchText;
		public bool CanDeleteSubContent { get; set; }

		public void Activate()
		{
			ShowingContentManager = true;
			isShowingStartContent = true;
			ShowStartContent();
		}

		public void Deactivate()
		{
			ShowingContentManager = false;
		}

		public void AddContentToContentList(ContentType type, string name)
		{
			RemoveContentFromContentLists(name);
			var newContent = new ContentIconAndName(ContentIconAndName.GetContentTypeIcon(type), name);
			DisplayContentList.Add(newContent);
			ContentList.Add(newContent);
			SubContentManager.AddSubContentAndRemoveDuplicateContent(DisplayContentList, service,
				newContent);
			RaisePropertyChanged("ContentList");
		}

		public void DeleteContentFromContentList(string name)
		{
			RemoveContentFromContentLists(name);
			FilterContentList();
			RaisePropertyChanged("ContentList");
		}

		public void RemoveTypeFromFilter(string typeAsString)
		{
			DisplayContentList.Clear();
			ContentType type;
			Enum.TryParse(typeAsString, out type);
			filterList.Remove(type);
			FilterContentList();
		}

		public void FilterContentList()
		{
			DisplayContentList.Clear();
			for (int index = 0; index < ContentList.Count; index++)
			{
				var content = ContentList[index];
				if (filterList.Count == 0 || filterList.Contains(content.GetContentType()))
					if (string.IsNullOrEmpty(SearchText) ||
						content.Name.ToLower().Contains(SearchText.ToLower()))
						DisplayContentList.Add(content);
			}
			var list = CopyList(DisplayContentList);
			for (int index = 0; index < list.Count; index++)
			{
				var content = list[index];
				SubContentManager.AddSubContentAndRemoveDuplicateContent(DisplayContentList, service,
					content);
			}
		}

		private readonly List<ContentType> filterList;

		public void AddTypeToFilter(string typeAsString)
		{
			DisplayContentList.Clear();
			ContentType type;
			Enum.TryParse(typeAsString, out type);
			if (!filterList.Contains(type))
				filterList.Add(type);
			FilterContentList();
		}

		private static List<ContentIconAndName> CopyList(
			ObservableCollection<ContentIconAndName> toCopyList)
		{
			var list = new List<ContentIconAndName>();
			foreach (var contentIconAndName in toCopyList)
				list.Add(contentIconAndName);
			return list;
		}
	}
}