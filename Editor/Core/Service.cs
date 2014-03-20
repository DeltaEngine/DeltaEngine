using System;
using System.Collections.Generic;
using DeltaEngine.Content;

namespace DeltaEngine.Editor.Core
{
	public interface Service
	{
		string UserName { get; }
		string ProjectName { get; }
		bool IsDeveloper { get; }
		string[] AvailableProjects { get; }
		event Action AvailableProjectsChanged;
		void ChangeProject(string newProjectName);
		event Action ProjectChanged;
		event Action<object> DataReceived;
		event Action<ContentType, string> ContentUpdated;
		event Action<string> ContentDeleted;
		event Action ContentReady;
		string CurrentContentProjectSolutionFilePath { get; set; }
		string GetAbsoluteSolutionFilePath(string contentProjectName);
		void SetContentProjectSolutionFilePath(string name, string slnFilePath);
		event Action<string> SolutionFilePathOfContentProjectChanged;
		void Send(object message, bool allowToCompressMessage = true);
		IEnumerable<string> GetAllContentNames();
		IEnumerable<string> GetAllContentNamesByType(ContentType type);
		ContentType? GetTypeOfContent(string content);
		void UploadContent(ContentMetaData metaData, Dictionary<string, byte[]> optionalFileData = null);
		void UploadContentFilesToService(IEnumerable<string> files);
		void DeleteContent(string selectedContent, bool deleteSubContent = false);
		void StartPlugin(Type plugin);
		EditorViewport Viewport { get; set; }
		void ShowToolbox(bool showToolbox);
	}

	//This exceptions are thrown by all implementations of the Service interface
	public class NoAccessForProject : Exception
	{
		public NoAccessForProject(string projectName)
			: base(projectName) {}
	}

	public class ProjectNotAvailable : Exception
	{
		public ProjectNotAvailable(string projectName)
			: base(projectName) {}
	}
}