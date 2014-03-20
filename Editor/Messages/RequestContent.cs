namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Used by Content Plugins to request required content data of another ContentType from Client
	/// </summary>
	public class RequestContent
	{
		public RequestContent() {}

		public RequestContent(string filename)
		{
			Filename = filename;
		}

		public string Filename { get; private set; }
	}
}