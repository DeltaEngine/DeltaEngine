using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Content.Online
{
	/// <summary>
	/// Connects to the Content Service and can reload files at runtime, but only works when a
	/// developer ApiKey has been setup. Will not be used for the end user.
	/// </summary>
	public class DeveloperOnlineContentLoader : ContentLoader
	{
		//ncrunch: no coverage start
		protected internal DeveloperOnlineContentLoader(OnlineServiceConnection connection)
		{
			this.connection = connection;
			if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole &&
				File.Exists(ContentMetaDataFilePath) && IsContentMetaDataYoungerThanOneMinute())
				return;
			StartedToRequestOnlineContent = true;
			connection.loadContentMetaData += OnLoadContentMetaData;
			connection.DataReceived += OnDataReceived;
			if (!connection.IsConnected)
				connection.ConnectToService();
			if (connection.IsLoggedIn)
				SendCheckProjectContent();
		}

		protected readonly OnlineServiceConnection connection;

		private bool IsContentMetaDataYoungerThanOneMinute()
		{
			return (DateTime.Now - File.GetLastWriteTime(ContentMetaDataFilePath)).TotalMinutes < 1;
		}

		public void OnLoadContentMetaData()
		{
			RefreshMetaData();
			isContentReady = true;
		}

		public void RefreshMetaData()
		{
			lock (metaData)
				metaData.Clear();
			lock (ProjectMetaDataFile)
				ParseXmlNode(ProjectMetaDataFile.Root);
		}

		private XmlFile ProjectMetaDataFile
		{
			get
			{
				if (projectMetaDataFile == null)
					InitializeProjectMetaDataFile();
				return projectMetaDataFile;
			}
		}

		protected XmlFile projectMetaDataFile;

		private void InitializeProjectMetaDataFile()
		{
			if (File.Exists(ContentMetaDataFilePath))
				LoadOrCreateContentMetaDataFile();
			else
				CreateContentMetaDataFile();
		}

		private void LoadOrCreateContentMetaDataFile()
		{
			try
			{
				projectMetaDataFile = new XmlFile(ContentMetaDataFilePath);
			}
			catch (Exception)
			{
				CreateContentMetaDataFile();
			}
		}

		private void CreateContentMetaDataFile()
		{
			string activeProject = ProjectName ?? AssemblyExtensions.GetEntryAssemblyForProjectName();
			XmlData root = XmlMetaDataExtensions.CreateProjectMetaData(activeProject, "Windows");
			projectMetaDataFile = new XmlFile(root);
		}

		private void ParseXmlNode(XmlData currentNode)
		{
			var currentElement = ParseContentMetaData(currentNode.Attributes);
			var name = currentNode.GetAttributeValue("Name");
			if (!metaData.ContainsKey(name) && currentNode.Parent != null)
				lock (metaData)
					metaData.Add(name, currentElement);
			foreach (var node in currentNode.Children)
				ParseXmlNode(node);
		}

		private static ContentMetaData ParseContentMetaData(List<XmlAttribute> attributes)
		{
			var data = new ContentMetaData();
			foreach (var attribute in attributes)
				switch (attribute.Name)
				{
				case "Name":
					data.Name = attribute.Value;
					break;
				case "Type":
					data.Type = attribute.Value == "FontXml"
						? ContentType.Font : attribute.Value.TryParse(ContentType.Image);
					break;
				case "LastTimeUpdated":
					data.LastTimeUpdated = DateExtensions.Parse(attribute.Value);
					break;
				case "LocalFilePath":
					data.LocalFilePath = attribute.Value;
					break;
				case "PlatformFileId":
					data.PlatformFileId = attribute.Value.Convert<int>();
					break;
				case "FileSize":
					data.FileSize = attribute.Value.Convert<int>();
					break;
				default:
					data.Values.Add(attribute.Name, attribute.Value);
					break;
				}
			if (string.IsNullOrEmpty(data.Name))
				throw new InvalidContentMetaDataNameIsAlwaysNeeded(attributes.ToText());
			return data;
		}

		public class InvalidContentMetaDataNameIsAlwaysNeeded : Exception
		{
			public InvalidContentMetaDataNameIsAlwaysNeeded(string message)
				: base(message) {}
		}

		protected readonly Dictionary<string, ContentMetaData> metaData =
			new Dictionary<string, ContentMetaData>(StringComparer.OrdinalIgnoreCase);

		private bool isContentReady;

		private void OnDataReceived(object message)
		{
			var login = message as LoginSuccessful;
			var newProject = message as SetProject;
			var receivedFile = message as UpdateContent;
			var deletedFile = message as DeleteContent;
			var contentReady = message as ContentReady;
			if (login != null)
				SendCheckProjectContent();
			else if (newProject != null)
				VerifyProject(newProject);
			else if (receivedFile != null)
				UpdateLocalContent(receivedFile);
			else if (deletedFile != null)
				DeleteLocalContent(deletedFile);
			else if (contentReady != null && ContentReady != null)
				ContentReady();
		}

		protected void SendCheckProjectContent()
		{
			string contentProjectName = ProjectMetaDataFile.Root.GetAttributeValue("Name");
			if (contentProjectName == "DeltaEngine.Editor")
				return;
			connection.Send(new CheckProjectContent(ProjectMetaDataFile.Root.ToString()));
		}

		private void VerifyProject(SetProject newProject)
		{
			if (newProject.Permissions == ProjectPermissions.None)
				throw new NoPermissionToUseProject(newProject.Name);
			if (ProjectName == newProject.Name)
				return;
			ProjectName = newProject.Name;
			ProjectMetaDataFile.Root.UpdateAttribute("Name", newProject.Name);
		}

		private class NoPermissionToUseProject : Exception
		{
			public NoPermissionToUseProject(string projectName)
				: base(projectName) {}
		}

		protected string ProjectName { get; set; }

		private void UpdateLocalContent(UpdateContent content)
		{
			UpdateContentMetaDataFile(content.MetaData, ProjectMetaDataFile.Root);
			UpdateLastTimeUpdatedFile();
			SaveXmlFile();
			foreach (var contentFile in content.OptionalFiles)
				if (contentFile.name != null)
					File.WriteAllBytes(Path.Combine(ContentProjectPath, contentFile.name), contentFile.data);
			if (ContentUpdated != null)
				ContentUpdated(content.MetaData.Type, content.MetaData.Name);
		}

		private static void UpdateContentMetaDataFile(ContentMetaData entry, XmlData parent)
		{
			RemoveExistingEntry(entry, parent);
			parent.AddMetaDataEntry(entry);
		}

		private static void RemoveExistingEntry(ContentMetaData entry, XmlData parent)
		{
			foreach (var child in parent.Children)
				if (child.GetAttributeValue("Name").Compare(entry.Name))
				{
					parent.RemoveChild(child);
					break;
				}
		}

		private void UpdateLastTimeUpdatedFile()
		{
			ProjectMetaDataFile.Root.UpdateAttribute("LastTimeUpdated", DateTime.Now.GetIsoDateTime());
		}

		private void SaveXmlFile()
		{
			if (!Directory.Exists(ContentProjectPath))
				Directory.CreateDirectory(ContentProjectPath);
			lock (ProjectMetaDataFile)
				ProjectMetaDataFile.Save(ContentMetaDataFilePath);
		}

		public event Action<ContentType, string> ContentUpdated;

		private void DeleteLocalContent(DeleteContent content)
		{
			XmlData entryToDelete = null;
			foreach (var child in ProjectMetaDataFile.Root.Children)
				if (child.GetAttributeValue("Name").Compare(content.ContentName))
					entryToDelete = child;
			if (entryToDelete == null)
				return;
			DeleteFiles(entryToDelete);
			ProjectMetaDataFile.Root.RemoveChild(entryToDelete);
			UpdateLastTimeUpdatedFile();
			SaveXmlFile();
			if (ContentDeleted != null)
				ContentDeleted(content.ContentName);
		}

		private void DeleteFiles(XmlData entry)
		{
			var localFilePath = entry.GetAttributeValue("LocalFilePath");
			if (string.IsNullOrEmpty(localFilePath))
				return;
			if (NoContentEntryUsesFile(entry, localFilePath))
				File.Delete(Path.Combine(ContentProjectPath, localFilePath));
		}

		private bool NoContentEntryUsesFile(XmlData entry, string localFilePath)
		{
			foreach (var child in ProjectMetaDataFile.Root.Children)
				if (child != entry && child.GetAttributeValue("LocalFilePath").Compare(localFilePath))
					return false;
			return true;
		}

		public event Action<string> ContentDeleted;
		public event Action ContentReady;

		protected override bool HasValidContentAndMakeSureItIsLoaded()
		{
			if (isContentReady)
				return true;
			isContentReady = File.Exists(ContentMetaDataFilePath);
			if (isContentReady)
				lock (ProjectMetaDataFile)
					ParseXmlNode(ProjectMetaDataFile.Root);
			return isContentReady;
		}

		public override DateTime LastTimeUpdated
		{
			get { return ProjectMetaDataFile.Root.GetAttributeValue("LastTimeUpdated", DateTime.Now); }
		}

		public override ContentMetaData GetMetaData(string contentName, Type contentClassType = null)
		{
			if (!isContentReady)
				throw new ContentNotReady();
			return metaData.ContainsKey(contentName) ? metaData[contentName] : null;
		}

		public class ContentNotReady : Exception {}

		public override void Dispose()
		{
			// ReSharper disable DelegateSubtraction
			connection.loadContentMetaData -= OnLoadContentMetaData;
			connection.DataReceived -= OnDataReceived;
			base.Dispose();
		}
	}
}