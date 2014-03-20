using System;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using Microsoft.Win32;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// The Current property will connect only once and is used by DeveloperOnlineContentLoader and
	/// NetworkLogger. The Editor will create its own connection and manages the connecting itself.
	/// </summary>
	public class OnlineServiceConnection : TcpSocket
	{
		//ncrunch: no coverage start
		internal OnlineServiceConnection(Settings settings, Action timedOut)
		{
			rememberIp = settings.OnlineServiceIp;
			rememberPort = settings.OnlineServicePort;
			projectName = AssemblyExtensions.GetEntryAssemblyForProjectName();
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				TimedOut += timedOut;
			DataReceived += OnDataReceived;
		}

		private readonly string rememberIp;
		private readonly int rememberPort;
		private readonly string projectName;

		private void OnDataReceived(object message)
		{
			var serverError = message as ServerError;
			var unknownMessage = message as UnknownMessage;
			var ready = message as ContentReady;
			var deleteContent = message as DeleteContent;
			var updateContent = message as UpdateContent;
			if (serverError != null && ServerErrorHappened != null)
				ServerErrorHappened(serverError.Error);
			else if (unknownMessage != null && ServerErrorHappened != null)
				ServerErrorHappened(unknownMessage.Text);
			else if (message is LoginSuccessful)
			{
				IsLoggedIn = true;
				if (LoggedIn != null)
					LoggedIn();
			}
			else if (ready != null)
			{
				loadContentMetaData();
				if (ContentReady != null)
					ContentReady();
			}
			else if ((deleteContent != null || updateContent != null) && ContentReceived != null)
				ContentReceived();
		}

		public Action<string> ServerErrorHappened;
		public bool IsLoggedIn { get; private set; }
		public Action LoggedIn;
		public Action loadContentMetaData;
		public Action ContentReady;
		public Action ContentReceived;

		/// <summary>
		/// Only used for the Editor, we are already connected and won't use much of this class
		/// </summary>
		internal OnlineServiceConnection()
		{
			DataReceived += OnDataReceived;
		}

		/// <summary>
		/// Allows delayed connecting to the service only when needed (logging or content request)
		/// </summary>
		public void ConnectToService()
		{
			if (connecting)
				return;
			connecting = true;
			var apiKey = GetApiKey();
			Connected += () => Send(new LoginRequest(apiKey, projectName));
			Connect(rememberIp, rememberPort);
		}

		private bool connecting;

		private static string GetApiKey()
		{
			string apiKey = "";
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					apiKey = (string)key.GetValue("ApiKey");
			return string.IsNullOrEmpty(apiKey) ? Guid.Empty.ToString() : apiKey;
		}
	}
}