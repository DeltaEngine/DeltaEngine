namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Send to the log service whenever a fatal error happens at runtime and is logged.
	/// </summary>
	public class LogErrorMessage : LogWarningMessage
	{
		protected LogErrorMessage() {}

		public LogErrorMessage(string message)
			: base(message) {}
	}
}