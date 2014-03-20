using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using Microsoft.Win32;

namespace DeltaEngine.Editor.ContentManager.Previewers
{
	public class XmlPreviewer : ContentPreview
	{
		//ncrunch: no coverage start
		protected override void Init() {}

		protected override void Preview(string contentName)
		{
			var projectName = GetProjectName();
			if (string.IsNullOrEmpty(projectName))
			{
				Logger.Warning("No project selected, can't access selected file.");
				return;
			}
			Process.Start(Path.Combine(Directory.GetCurrentDirectory(), "Content", projectName,
				contentName + ".xml"));
		}

		private static string GetProjectName()
		{
			string projectName = "";
			var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor");
			if (key != null)
				projectName = (string)key.GetValue("SelectedProject");
			return projectName;
		}
	}
}