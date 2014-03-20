using System;

namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Send to the log service whenever a info message is logged.
	/// </summary>
	public class LogInfoMessage : TextMessage
	{
		protected LogInfoMessage() {}

		public LogInfoMessage(string message)
			: base(message)
		{
			TimeStamp = DateTime.Now;
		}

		public DateTime TimeStamp { get; private set; }
	}
}