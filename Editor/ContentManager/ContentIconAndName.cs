using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.ContentManager
{
	public class ContentIconAndName : ViewModelBase
	{
		public ContentIconAndName(string icon, string name,
			ObservableCollection<ContentIconAndName> subContent = null,
			ObservableCollection<ContentIconAndName> additionalContent = null)
		{
			Icon = icon;
			Name = name;
			Brush = new SolidColorBrush();
			SubContent = subContent ?? new ObservableCollection<ContentIconAndName>();
			AdditionalContent = additionalContent ?? new ObservableCollection<ContentIconAndName>();
		}

		public string Icon { get; private set; }
		public string Name { get; private set; }

		public Brush Brush
		{
			get { return brush; }
			set
			{
				brush = value;
				RaisePropertyChanged("Brush");
			}
		}

		private Brush brush;
		public ObservableCollection<ContentIconAndName> SubContent { get; private set; }
		public ObservableCollection<ContentIconAndName> AdditionalContent { get; private set; }

		public static string GetContentTypeIcon(ContentType? type)
		{
			string iconFileName = type + ".png";
			return Path.Combine(ContentTypeFolder, iconFileName);
		}

		private const string ContentTypeFolder = "../Images/ContentTypes/";

		public ContentType GetContentType()
		{
			return GetContentTypeFromIcon(Icon);
		}

		private static ContentType GetContentTypeFromIcon(string icon)
		{
			var fileName = Path.GetFileNameWithoutExtension(icon);
			return ConvertStringToContentType(fileName);
		}

		private static ContentType ConvertStringToContentType(string fileName)
		{
			ContentType type;
			Enum.TryParse(fileName, false, out type);
			return type;
		}

		public ObservableCollection<ContentIconAndName> DeleteSubContent(Service service,
			ObservableCollection<ContentIconAndName> contentList)
		{
			foreach (var contentIconAndName in AdditionalContent)
				contentList = contentIconAndName.DeleteSubContent(service, contentList);
			foreach (var contentIconAndName in SubContent)
				contentList = contentIconAndName.DeleteSubContent(service, contentList);
			service.DeleteContent(Name);
			for (int i = 0; i < contentList.Count; i++)
			{
				if (contentList[i].Name != Name)
					continue;
				contentList.Remove(contentList[i]);
				i--;
			}
			return contentList;
		}
	}
}