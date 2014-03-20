namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Send to the log service whenever a warning message is logged.
	/// </summary>
	public class LogWarningMessage : LogInfoMessage
	{
		protected LogWarningMessage() {}

		public LogWarningMessage(string message)
			: base(message) {}
	}
}