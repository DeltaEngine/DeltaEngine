using System;
using System.IO;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.ProjectCreator
{
	public static class MyDocumentsExtensions
	{
		public static string[] GetSupportedVisualStudioFolders()
		{
			string myDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			return new[]
			{
				Path.Combine(myDocumentsPath, "Visual Studio 2013"),
				Path.Combine(myDocumentsPath, "Visual Studio 2012"),
				Path.Combine(myDocumentsPath, "Visual Studio 2010"),
				Path.Combine(myDocumentsPath, "Visual Studio 2008")
			};
		}

		private static string GetVisualStudioDocumentsFolder()
		{
			if (StackTraceExtensions.IsStartedFromNCrunch())
				return GetSupportedVisualStudioFolders()[0];
			//ncrunch: no coverage start
			foreach (var myVisualStudioDocuments in GetSupportedVisualStudioFolders())
				if (Directory.Exists(myVisualStudioDocuments))
					return myVisualStudioDocuments;
			throw new VisualStudioDocumentsLocationNotFound();
			//ncrunch: no coverage end
		}

		private class VisualStudioDocumentsLocationNotFound : Exception {}

		public static string GetVisualStudioProjectsFolder()
		{
			return Path.Combine(GetVisualStudioDocumentsFolder(), "Projects");
		}

		public static string GetVisualStudioDeltaEngineTemplateZip(string templateName)
		{
			return Path.Combine(GetVisualStudioDocumentsFolder(), "Templates", "ProjectTemplates",
				"Visual C#", "Delta Engine", templateName + ".zip");
		}
	}
}