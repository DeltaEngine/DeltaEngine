using DeltaEngine.Content;

namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// For each outdated content file after sending CheckProjectContent we get one UpdateContent
	/// message from the server containing both an updated ContentMetaData entry and also the content
	/// data itself (optional). Also used to upload new files to the content server (ContentManager)
	/// </summary>
	public class UpdateContent : ContentMessage
	{
		private UpdateContent() {}

		public UpdateContent(ContentMetaData metaData, FileNameAndBytes[] optionalFiles = null)
		{
			MetaData = metaData;
			OptionalFiles = optionalFiles;
		}

		public ContentMetaData MetaData { get; private set; }
		public FileNameAndBytes[] OptionalFiles { get; private set; }

		public struct FileNameAndBytes
		{
			public string name;
			public byte[] data;
		}
	}
}