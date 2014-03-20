using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DeltaEngine.Content;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Mocks;
using DeltaEngine.Networking;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	[Ignore]
	public class EditorContentLoaderTests
	{
		[Test]
		public void GetContentFromOnlineService()
		{
			DeleteContentDirectoriesIfAvailable();
			var messagesReceived = new List<object>();
			var connection = new OnlineServiceConnection(new MockSettings(),
				() => { throw new ConnectionTimedOut(); });
			var contentLoader = new EditorContentLoader(connection);
			connection.Connected += () => GetProjectsAndLogin(connection);
			connection.DataReceived += message => { messagesReceived.Add(message); };
			Thread.Sleep(3000);
			CheckConnection(connection);
			CheckContentLoader(contentLoader);
			CheckAndWriteMessages(messagesReceived);
		}

		private static void DeleteContentDirectoriesIfAvailable()
		{
			const string ContentDirectory = "Content";
			var projectDirectory = Path.Combine(ContentDirectory, ProjectName);
			if (Directory.Exists(projectDirectory))
				Directory.Delete(projectDirectory, true);
			if (Directory.Exists(ContentDirectory))
				Directory.Delete(ContentDirectory, true);
		}

		private const string ProjectName = "LogoApp";

		private static void GetProjectsAndLogin(Client connection)
		{
			connection.Send(new ProjectNamesRequest());
			connection.Send(new LoginRequest(LoadApiKeyFromRegistry(), ProjectName));
		}

		private static string LoadApiKeyFromRegistry()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					return (string)key.GetValue("ApiKey");
			return null;
		}

		private class ConnectionTimedOut : Exception {}

		private static void CheckConnection(OnlineServiceConnection connection)
		{
			Assert.AreEqual(true, connection.IsConnected);
			Assert.AreEqual(true, connection.IsLoggedIn);
		}

		private static void CheckContentLoader(EditorContentLoader contentLoader)
		{
			Assert.NotNull(contentLoader);
			Assert.AreEqual(DefaultContentFiles + 2, contentLoader.GetAllNames().Count);
			Assert.AreEqual(DefaultImages + 1, contentLoader.GetAllNamesByType(ContentType.Image).Count);
		}

		private const int DefaultContentFiles = 11;
		private const int DefaultImages = 1;

		private static void CheckAndWriteMessages(List<object> messages)
		{
			foreach (var message in messages)
				Console.WriteLine(message);
			Assert.AreEqual(UpdateContentMessages + AdditionalMessages, messages.Count);
		}

		private const int UpdateContentMessages = 14;
		private const int AdditionalMessages = 4;
	}
}