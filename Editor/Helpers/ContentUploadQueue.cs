using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Editor.ContentManager;

namespace DeltaEngine.Editor.Helpers
{
	public class ContentUploadQueue
	{
		public ContentUploadQueue(FileReader fileReader,
			Action<ContentMetaData, Dictionary<string, byte[]>> uploadContent)
		{
			this.fileReader = fileReader;
			this.uploadContent = uploadContent;
		}

		private readonly FileReader fileReader;
		private readonly Action<ContentMetaData, Dictionary<string, byte[]>> uploadContent;

		public void EnqueueFile(string absoluteFilePath)
		{
			queue.Enqueue(absoluteFilePath);
			if (isUploadingContent || queue.Count != 1)
				return;
			isUploadingContent = true;
			UploadContentFileToService();
		}

		protected readonly Queue<string> queue = new Queue<string>();
		protected bool isUploadingContent;

		private void UploadContentFileToService()
		{
			string contentFilePath = Dequeue();
			byte[] fileBytes = ReadFileBytes(contentFilePath);
			if (fileBytes.Length != 0)
			{
				ContentMetaData contentMetaData = CreateContentMetaData(contentFilePath);
				var fileNameAndBytes = CreateFileNameAndBytes(contentFilePath, fileBytes);
				uploadContent(contentMetaData, fileNameAndBytes);
				lastUploadedFilePath = contentFilePath;
			}
			else
				Next();
		}

		private string lastUploadedFilePath = "";

		private string Dequeue()
		{
			try
			{
				return TryDequeue();
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("Queue empty"))
					throw new NoMoreFilesToUpload();
				throw; //ncrunch: no coverage
			}
		}

		private string TryDequeue()
		{
			return queue.Dequeue();
		}

		public class NoMoreFilesToUpload : Exception {}

		private byte[] ReadFileBytes(string contentFilePath)
		{
			try
			{
				return TryReadFileBytes(contentFilePath);
			}
			catch (FileExceedsMaximumSize ex)
			{
				Logger.Warning(ex.FilePath + " is too large, the maximum file size is 16MB");
			}
			catch (XmlException ex)
			{
				Logger.Warning("Invalid XmlFile " + Path.GetFileName(contentFilePath) + ": " + ex.Message);
			}
			catch (Exception)
			{
				Logger.Warning("Unable to read bytes for uploading to the server: " +
					Path.GetFileName(contentFilePath));
			}
			return new byte[0];
		}

		private byte[] TryReadFileBytes(string contentFilePath)
		{
			byte[] bytes = fileReader.ReadAllBytes(contentFilePath);
			if (bytes.Length > MaximumFileSize)
				throw new FileExceedsMaximumSize(contentFilePath, MaximumFileSize); //ncrunch: no coverage
			return bytes;
		}

		private const int MaximumFileSize = 16777216;

		public class FileExceedsMaximumSize : Exception
		{
			public FileExceedsMaximumSize(string contentFilePath, int maximumFileSize)
				: base(contentFilePath + " is larger than " + maximumFileSize + "bytes")
			{
				FilePath = contentFilePath;
			}

			public string FilePath { get; private set; }
		}

		//ncrunch: no coverage start
		protected virtual ContentMetaData CreateContentMetaData(string contentFilePath)
		{
			var metaDataCreator = new ContentMetaDataCreator();
			ContentMetaData contentMetaData = metaDataCreator.CreateMetaDataFromFile(contentFilePath);
			return contentMetaData;
		} //ncrunch: no coverage end

		private static Dictionary<string, byte[]> CreateFileNameAndBytes(string contentFilePath,
			byte[] fileBytes)
		{
			var fileNameAndBytes = new Dictionary<string, byte[]>();
			fileNameAndBytes.Add(Path.GetFileName(contentFilePath), fileBytes);
			return fileNameAndBytes;
		}

		public void EnqueueFiles(IEnumerable<string> absoluteFilePaths)
		{
			foreach (var absoluteFilePath in absoluteFilePaths)
				EnqueueFile(absoluteFilePath);
		}

		public void Next()
		{
			if (isUploadingContent && queue.Count == 0)
				isUploadingContent = false;
			else
				UploadContentFileToService();
		}

		public bool IsLastFileUploaded(string filename)
		{
			return isUploadingContent && lastUploadedFilePath.Contains(filename);
		}

		public bool ContainsLastFileUploaded(string message)
		{
			string filename = Path.GetFileName(lastUploadedFilePath);
			return isUploadingContent && message.Contains(filename);
		}
	}
}