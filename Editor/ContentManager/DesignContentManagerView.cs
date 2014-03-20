using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DeltaEngine.Editor.ContentManager
{
	public class DesignContentManagerView
	{
		//ncrunch: no coverage start 
		public DesignContentManagerView()
		{
			var children = new ObservableCollection<ContentIconAndName>();
			children.Add(new ContentIconAndName("../Images/ContentTypes/Xml.png", "Xml File"));
			DisplayContentList = new List<ContentIconAndName>
			{
				new ContentIconAndName("../Images/ContentTypes/Image.png", "Test Image", children),
				new ContentIconAndName("../Images/ContentTypes/Json.png", "Json File")
			};
		}

		public List<ContentIconAndName> DisplayContentList { get; set; }
	}
}