namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Deletes a content file on the server or client, whoever has obsolete files.
	/// </summary>
	public class DeleteContent : ContentMessage
	{
		private DeleteContent() {}

		public DeleteContent(string contentName, bool deleteSubImages = false)
		{
			ContentName = contentName;
			DeleteSubImages = deleteSubImages;
		}

		public string ContentName { get; private set; }
		public bool DeleteSubImages { get; private set; }
	}
}