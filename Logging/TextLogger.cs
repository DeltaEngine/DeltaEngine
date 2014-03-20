using System;
using DeltaEngine.Core;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Simply appends log messages to a public property.
	/// </summary>
	public class TextLogger : Logger
	{
		public TextLogger()
			: base(true) {}

		public override void Write(MessageType messageType, string message)
		{
			message = CreateMessageTypePrefix(messageType) + message;
			Log = string.IsNullOrEmpty(Log) ? message : Log + Environment.NewLine + message;
			if (NewLogMessage != null)
				NewLogMessage();
		}

		public string Log { get; set; }
		public event Action NewLogMessage;
	}
}