namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Check client content by sending the ContentMetaData.xml to the server. It is used for grabbing
	/// and updating local content in the app and also in the ContentManager tool to get new content.
	/// </summary>
	public class CheckProjectContent : ContentMessage
	{
		private CheckProjectContent() {}

		public CheckProjectContent(string contentMetaDataFile)
		{
			ContentMetaDataFile = contentMetaDataFile;
		}

		public string ContentMetaDataFile { get; private set; }
	}
}