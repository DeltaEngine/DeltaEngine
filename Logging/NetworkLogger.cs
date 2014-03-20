using DeltaEngine.Core;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;

namespace DeltaEngine.Logging
{
	/// <summary>
	/// Logs to a remote location that is listening at the given address and port. By default logs go
	/// to the DeltaEngine.net:777 LogService. This way you can manage all issues and view all errors
	/// on the DeltaEngine.net website for your project. Or use your own Log Server if you like.
	/// </summary>
	public class NetworkLogger : Logger
	{
		public NetworkLogger(OnlineServiceConnection connection)
			: base(true)
		{
			this.connection = connection;
		}

		private readonly OnlineServiceConnection connection;

		public override void Write(MessageType messageType, string message)
		{
			if(!Settings.Current.UseOnlineLogging)
				return;
			if (message.StartsWith("Server Error: ") || message.StartsWith("No content available."))
				return;
			if (!connection.IsConnected)
				connection.ConnectToService();
			if (!connection.IsLoggedIn)
				connection.LoggedIn += () => Write(messageType, message);
			else if (messageType == MessageType.Info)
				connection.Send(new LogInfoMessage(message));
			else if (messageType == MessageType.Warning)
				connection.Send(new LogWarningMessage(message));
			else
				connection.Send(new LogErrorMessage(message));
		}
	}
}