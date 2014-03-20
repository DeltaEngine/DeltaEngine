using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor.Emulator
{
	public class UIToolNamesAndPaths : ToolExtensions
	{
		public List<string> GetNames()
		{
			return (from UITool toolName in Enum.GetValues(typeof(UITool)) 
				select toolName.ToString()).ToList();
		}

		public string GetImagePath(string toolName)
		{
			return BaseImagePath + toolName + ImageExtension;
		}

		private const string BaseImagePath = @"..\Images\UIEditor\Create";
		private const string ImageExtension = ".png";
	}
}