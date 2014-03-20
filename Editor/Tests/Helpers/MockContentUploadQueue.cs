using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content;
using DeltaEngine.Editor.Helpers;

namespace DeltaEngine.Editor.Tests.Helpers
{
	public class MockContentUploadQueue : ContentUploadQueue
	{
		public MockContentUploadQueue(FileReader fileReader,
			Action<ContentMetaData, Dictionary<string, byte[]>> upload)
			: base(fileReader, upload) {}

		protected override ContentMetaData CreateContentMetaData(string contentFilePath)
		{
			var contentMetaData = new ContentMetaData();
			contentMetaData.Name = Path.GetFileNameWithoutExtension(contentFilePath);
			contentMetaData.Type = ContentType.JustStore;
			contentMetaData.LastTimeUpdated = DateTime.Now;
			contentMetaData.LocalFilePath = Path.GetFileName(contentFilePath);
			contentMetaData.FileSize = 5;
			return contentMetaData;
		}

		public int QueuedFiles
		{
			get { return queue.Count; }
		}

		public bool IsReset
		{
			get { return !isUploadingContent; }
		}
	}
}