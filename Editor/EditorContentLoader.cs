using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Content.Online;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Editor
{
	public class EditorContentLoader : DeveloperOnlineContentLoader
	{
		public EditorContentLoader(OnlineServiceConnection connection)
			: base(connection) {}

		public void SetProject(SetProject newProject)
		{
			ContentProjectPath = Path.Combine("Content", newProject.Name);
			projectMetaDataFile = null;
			ProjectName = newProject.Name;
			ClearBufferedResources();
			RefreshMetaData();
			SendCheckProjectContent();
		}

		public List<string> GetAllNames()
		{
			return new List<string>(metaData.Keys);
		}

		public List<string> GetAllNamesByType(ContentType type)
		{
			var names = new List<string>();
			lock (metaData)
			{
				foreach (var contentMetaData in metaData)
					if (contentMetaData.Value.Type == type)
						names.Add(contentMetaData.Key);
			}
			return names;
		}

		public void Upload(ContentMetaData contentMetaData,
			Dictionary<string, byte[]> optionalFileData)
		{
			var fileList = new List<UpdateContent.FileNameAndBytes>();
			if (optionalFileData != null)
				foreach (var data in optionalFileData)
					fileList.Add(new UpdateContent.FileNameAndBytes { name = data.Key, data = data.Value });
			connection.Send(new UpdateContent(contentMetaData, fileList.ToArray()));
		}

		public void Delete(string contentName, bool deleteSubContent)
		{
			connection.Send(new DeleteContent(contentName, deleteSubContent));
		}

		public ContentType? GetTypeOfContent(string content)
		{
			lock (metaData)
			{
				foreach (var contentMetaData in metaData)
					if (contentMetaData.Value.Name == content)
						return contentMetaData.Value.Type;
			}
			return null;
		}
	}
}