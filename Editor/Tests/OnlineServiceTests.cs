using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using DeltaEngine.Platforms;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	[Ignore]
	public class OnlineServiceTests
	{
		[Test]
		public void CheckOnlineService()
		{
			var settings = new MockSettings();
			var service = new OnlineService();
			object result = null;
			var connection = new OnlineServiceConnection(settings,
				() => { throw new ConnectionTimedOut(); });
			connection.Connected +=
				() => connection.Send(new LoginRequest(LoadApiKeyFromRegistry(), "LogoApp"));
			connection.DataReceived += message => result = message;
			service.Connect("CurrentUser", connection);
			Thread.Sleep(500);
			Console.WriteLine("User Name: " + service.UserName);
			CheckService(service, "LogoApp", result);
			Assert.IsFalse(service.IsDeveloper);
			bool hasProjectChanged = false;
			service.ProjectChanged += () => hasProjectChanged = true;
			service.ChangeProject("Asteroids");
			Thread.Sleep(500);
			Assert.IsTrue(hasProjectChanged);
			CheckService(service, "Asteroids", result);
			Assert.IsFalse(service.IsDeveloper);
		}

		private static string LoadApiKeyFromRegistry()
		{
			using (var key = Registry.CurrentUser.OpenSubKey(@"Software\DeltaEngine\Editor", false))
				if (key != null)
					return (string)key.GetValue("ApiKey");
			return null;
		}

		private class ConnectionTimedOut : Exception {}

		private static void CheckService(OnlineService service, string projectName, object message)
		{
			Assert.AreEqual(projectName, service.ProjectName);
			Assert.AreNotEqual(ProjectPermissions.None, service.Permissions);
			Assert.IsInstanceOf<SetProject>(message);
		}

		[Test]
		public void GetAvailableProjectNames()
		{
			var service = new MockService("John Doe", "LogoApp");
			bool hasAvailableProjectsChanged = false;
			service.AvailableProjectsChanged += () => hasAvailableProjectsChanged = true;
			Assert.IsFalse(hasAvailableProjectsChanged);
			service.SetAvailableProjects("LogoApp", "GhostWars");
			Assert.IsTrue(hasAvailableProjectsChanged);
			Assert.AreEqual(2, service.AvailableProjects.Length);
			Assert.AreEqual("LogoApp", service.AvailableProjects[0]);
			Assert.AreEqual("GhostWars", service.AvailableProjects[1]);
		}

		[Test]
		public void AbsoluteSolutionFilePathDependsOnTheSelectedContentProject()
		{
			var settings = new MockSettings();
			var service = new OnlineService();
			var connection = new OnlineServiceConnection(settings,
				() => { throw new ConnectionTimedOut(); });
			service.Connect("CurrentUser", connection);
			Assert.AreEqual("", service.CurrentContentProjectSolutionFilePath);
			service.ChangeProject("LogoApp");
			Thread.Sleep(1000);
			AssertSolutionFilePath(GetSamplesSlnPath(), service);
			service.ChangeProject("DeltaEngine.Tutorials");
			Thread.Sleep(1000);
			AssertSolutionFilePath(GetTutorialsSlnPath(), service);
		}

		private static string GetSamplesSlnPath()
		{
			return Path.Combine(PathExtensions.GetFallbackEngineSourceCodeDirectory(),
				"DeltaEngine.Samples.sln");
		}

		private static void AssertSolutionFilePath(string expectedFilePath, Service service)
		{
			Assert.IsTrue(expectedFilePath.Compare(service.CurrentContentProjectSolutionFilePath));
			Assert.IsTrue(File.Exists(service.CurrentContentProjectSolutionFilePath));
		}

		private static string GetTutorialsSlnPath()
		{
			return Path.Combine(PathExtensions.GetFallbackEngineSourceCodeDirectory(), "Tutorials",
				"DeltaEngine.Tutorials.Basics.sln");
		}

		[Test]
		public void SolutionFilePathIsStoredInSettingsFileWithProjectName()
		{
			var settings = new FileSettings();
			var service = new OnlineService();
			var connection = new OnlineServiceConnection(settings,
				() => { throw new ConnectionTimedOut(); });
			service.Connect("CurrentUser", connection);
			service.ChangeProject("DeltaEngine.Tutorials");
			Thread.Sleep(1000);
			service.CurrentContentProjectSolutionFilePath = TutorialsSolutionFilePath;
			var projects = Settings.Current.GetValue("ContentProjects", new Dictionary<string, string>());
			Assert.GreaterOrEqual(projects.Count, 1);
			settings.Save();
		}

		private const string TutorialsSolutionFilePath =
			@"C:\code\DeltaEngine\Tutorials\DeltaEngine.Tutorials.Entities.sln";

		[Test]
		public void SolutionFilePathCanBeLoadedFromSettings()
		{
			var settings = new FileSettings();
			var service = new OnlineService();
			var connection = new OnlineServiceConnection(settings,
				() => { throw new ConnectionTimedOut(); });
			service.Connect("CurrentUser", connection);
			service.ChangeProject("DeltaEngine.Tutorials");
			Thread.Sleep(1000);
			Assert.AreEqual(TutorialsSolutionFilePath, service.CurrentContentProjectSolutionFilePath);
		}
	}
}