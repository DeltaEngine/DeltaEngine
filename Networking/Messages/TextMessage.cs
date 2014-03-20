namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Simple text message, mostly used as a base class for log messages, but also for chat services.
	/// </summary>
	public class TextMessage
	{
		protected TextMessage() {}

		public TextMessage(string text)
		{
			Text = text;
		}

		public string Text { get; private set; }
	}
}