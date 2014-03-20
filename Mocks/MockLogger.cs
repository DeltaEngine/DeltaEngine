using System.Collections.Generic;
using DeltaEngine.Core;

namespace DeltaEngine.Mocks
{
	/// <summary>
	/// Simply logs to a public property used in unit testing.
	/// </summary>
	public class MockLogger : Logger
	{
		public override void Write(MessageType messageType, string message)
		{
			NumberOfMessages++;
			LastMessage = message;
			Lines.Add(CreateMessageTypePrefix(messageType) + message);
		}

		public int NumberOfMessages { get; private set; }

		public static readonly List<string> Lines = new List<string>();
	}
}