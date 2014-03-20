using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class OnlineService : Service
	{
		public OnlineService()
		{
			AvailableProjects = new string[0];
			contentProjects = Settings.Current.GetValue("ContentProjects",
				new Dictionary<string, string>());
			contentUploadQueue = new ContentUploadQueue(new FileReader(), UploadContent);
		}

		public string[] AvailableProjects { get; private set; }
		private readonly Dictionary<string, string> contentProjects;
		private readonly ContentUploadQueue contentUploadQueue;

		public void Connect(string userName, OnlineServiceConnection connection)
		{
			UserName = userName;
			onlineServiceConnection = connection;
			connection.DataReceived += OnDataReceived;
			send = connection.Send;
			var oldResolver = ContentLoader.resolver;
			ContentLoader.DisposeIfInitialized();
			ContentLoader.resolver = oldResolver;
			ContentLoader.Use<EditorContentLoader>();
			editorContent = new EditorContentLoader(onlineServiceConnection);
			editorContent.ContentUpdated += OnContentUpdated;
			editorContent.ContentDeleted += OnContentDeleted;
			editorContent.ContentReady += OnContentReady;
		}

		public string UserName { get; private set; }
		private OnlineServiceConnection onlineServiceConnection;

		private void OnDataReceived(object message)
		{
			var newProject = message as SetProject;
			var content = message as UpdateContent;
			var serverError = message as ServerError;
			if (newProject != null)
				ChangeProject(newProject);
			else if (content != null &&
				contentUploadQueue.IsLastFileUploaded(content.MetaData.LocalFilePath))
				contentUploadQueue.Next();
			else if (serverError != null && contentUploadQueue.ContainsLastFileUploaded(serverError.Error))
				contentUploadQueue.Next();
			else if (DataReceived != null)
				DataReceived(message);
		}

		private void ChangeProject(SetProject project)
		{
			IsDeveloper = project.Permissions == ProjectPermissions.Full;
			if (project.Permissions == ProjectPermissions.None)
			{
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					throw new NoAccessForProject(project.Name);
				MessageBox.Show("No access to project " + project.Name, "Fatal Error");
			}
			else
			{
				ProjectName = project.Name;
				Permissions = project.Permissions;
				editorContent.SetProject(project);
				isContentReady = false;
				if (ProjectChanged != null)
					ProjectChanged();
			}
		}

		public bool IsDeveloper { get; private set; }
		public string ProjectName { get; private set; }
		public ProjectPermissions Permissions { get; private set; }
		private EditorContentLoader editorContent;
		public event Action ProjectChanged;
		public event Action<object> DataReceived;
		private Action<object, bool> send;

		private void OnContentUpdated(ContentType type, string name)
		{
			editorContent.RefreshMetaData();
			if (isContentReady)
				Logger.Info(type + " " + name + " has been saved");
			if (ContentUpdated != null)
				ContentUpdated(type, name);
		}

		private bool isContentReady;
		public event Action<ContentType, string> ContentUpdated;

		private void OnContentDeleted(string contentName)
		{
			editorContent.RefreshMetaData();
			if (isContentReady)
				Logger.Info(contentName + " has been deleted");
			if (ContentDeleted != null)
				ContentDeleted(contentName);
		}

		public event Action<string> ContentDeleted;

		private void OnContentReady()
		{
			isContentReady = true;
			if (ContentReady != null)
				ContentReady();
		}

		public event Action ContentReady;

		public void ChangeProject(string newProjectName)
		{
			if (newProjectName.Compare(ProjectName))
				return;
			if (!AvailableProjects.Contains(newProjectName))
				throw new ProjectNotAvailable(newProjectName);
			send(new ChangeProjectRequest(newProjectName), true);
		}

		public string CurrentContentProjectSolutionFilePath
		{
			get { return GetAbsoluteSolutionFilePath(ProjectName); }
			set { SetContentProjectSolutionFilePath(ProjectName, value); }
		}

		public string GetAbsoluteSolutionFilePath(string contentProjectName)
		{
			if (String.IsNullOrWhiteSpace(contentProjectName))
				return "";
			if (contentProjects.ContainsKey(contentProjectName))
				return contentProjects[contentProjectName];
			if (IsStarterKit(contentProjectName))
				return SolutionExtensions.GetSamplesSolutionFilePath();
			if (IsTutorial(contentProjectName))
				return SolutionExtensions.GetTutorialsSolutionFilePath();
			return "";
		}

		private static bool IsStarterKit(string projectName)
		{
			var starterKits = new[]
			{
				"Asteroids", "Blocks", "Breakout", "Drench", "EmptyApp", "GameOfDeath", "GhostWars",
				"Insight", "LogoApp", "SideScroller", "Snake"
			};
			return starterKits.Contains(projectName);
		}

		private static bool IsTutorial(string projectName)
		{
			return projectName == "DeltaEngine.Tutorials";
		}

		public void SetContentProjectSolutionFilePath(string name, string slnFilePath)
		{
			if (name == null)
				return;
			if (contentProjects.ContainsKey(name))
				contentProjects[name] = slnFilePath;
			else
				contentProjects.Add(name, slnFilePath);
			Settings.Current.SetValue("ContentProjects", contentProjects);
			RaiseSolutionFilePathOfContentProjectChangedEvent(name);
		}

		private void RaiseSolutionFilePathOfContentProjectChangedEvent(string contentProjectName)
		{
			if (SolutionFilePathOfContentProjectChanged != null)
				SolutionFilePathOfContentProjectChanged(contentProjectName);
		}

		public event Action<string> SolutionFilePathOfContentProjectChanged;

		public void Send(object message, bool allowToCompressMessage = true)
		{
			send(message, allowToCompressMessage);
		}

		public IEnumerable<string> GetAllContentNames()
		{
			return editorContent.GetAllNames();
		}

		public IEnumerable<string> GetAllContentNamesByType(ContentType type)
		{
			return editorContent.GetAllNamesByType(type);
		}

		public ContentType? GetTypeOfContent(string content)
		{
			return editorContent.GetTypeOfContent(content);
		}

		public void UploadContent(ContentMetaData metaData,
			Dictionary<string, byte[]> optionalFileData = null)
		{
			editorContent.Upload(metaData, optionalFileData);
		}

		public void DeleteContent(string contentName, bool deleteSubContent = false)
		{
			editorContent.Delete(contentName, deleteSubContent);
		}

		public void StartPlugin(Type plugin)
		{
			if (StartEditorPlugin != null)
				StartEditorPlugin(plugin);
		}

		public event Action<Type> StartEditorPlugin;
		public EditorViewport Viewport { get; set; }

		void Service.ShowToolbox(bool showToolbox)
		{
			UpdateToolboxVisibility(showToolbox);
		}

		public event Action<bool> UpdateToolboxVisibility;

		public void SetAvailableProjects(string[] projectNames)
		{
			AvailableProjects = projectNames;
			if (AvailableProjectsChanged != null)
				AvailableProjectsChanged();
		}

		public event Action AvailableProjectsChanged;

		public void UploadContentFilesToService(IEnumerable<string> contentFilePaths)
		{
			contentUploadQueue.EnqueueFiles(contentFilePaths);
		}
	}
}