using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using DeltaEngine.Editor.Core.Properties;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Mocks;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Editor.Tests
{
	[Ignore]
	public class EditorViewModelTests
	{
		//ncrunch: no coverage start
		[Test]
		public void LoginWithInvalidApiKeyShouldFail()
		{
			var model = CreateModel();
			model.ApiKey = "invalid";
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			Assert.IsTrue(model.Error.Contains("Server Error: Unable to login, invalid ApiKey!"));
			Assert.IsFalse(model.IsLoggedIn);
		}

		private static EditorViewModel CreateModel()
		{
			return new EditorViewModel(new EditorPluginLoader(Path.Combine("..", "..", "..")),
				new MockSettings());
		}

		[Test]
		public void LoginWithValidApiKeyShouldPass()
		{
			var model = CreateModel();
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			Assert.AreEqual(Resources.GetApiKeyHere, model.Error);
			Assert.IsTrue(model.IsLoggedIn);
		}

		[Test]
		public void LogoutSetsApiKeyEmpty()
		{
			var model = CreateModel();
			model.OnLogoutButtonClicked.Execute(null);
			Assert.AreEqual("", model.ApiKey);
		}

		[Test]
		public void LoginPanelShouldBeVisibleWhenNotLoggedIn()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			if (model.Error == "")
			{
				Assert.AreEqual(Visibility.Hidden, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Hidden, model.ErrorVisibility);
				Assert.AreEqual(Visibility.Visible, model.EditorPanelVisibility);
			}
			else
			{
				Assert.AreEqual(Visibility.Visible, model.LoginPanelVisibility);
				Assert.AreEqual(Visibility.Visible, model.ErrorVisibility);
				Assert.AreEqual(Visibility.Hidden, model.EditorPanelVisibility);
			}
		}

		private static EditorViewModel CreateModelAndTryToLoginWithEmptyApiKey()
		{
			var model = CreateModel();
			model.ApiKey = "";
			model.OnLoginButtonClicked.Execute(null);
			Thread.Sleep(200);
			return model;
		}

		[Test]
		public void CheckErrorForegroundAndBackgroundColor()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			if (model.Error == "")
			{
				Assert.AreEqual(Brushes.Black, model.ErrorForegroundColor);
				Assert.AreEqual(Brushes.Transparent, model.ErrorBackgroundColor);
			}
			else
			{
				Assert.AreEqual(Brushes.Blue, model.ErrorForegroundColor);
				Assert.AreEqual(Brushes.Transparent, model.ErrorBackgroundColor);
			}
		}

		[Test]
		public void LoadApiKeyFromRegistry()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			var rememberKey = model.ApiKey;
			Console.WriteLine("Current API Key from Registry: " + rememberKey);
			model.ApiKey = "123";
			model.SaveApiKey();
			Assert.AreEqual("123", model.ApiKey);
			model.ApiKey = rememberKey;
			model.SaveApiKey();
		}

		[Test]
		public void GetAndSetProjectName()
		{
			var model = CreateModelAndTryToLoginWithEmptyApiKey();
			var rememberProject = model.SelectedProject;
			model.OnLogoutButtonClicked.Execute(null);
			Assert.AreEqual(rememberProject, model.SelectedProject);
			Assert.GreaterOrEqual(model.AvailableProjects.Count, 1);
			Console.WriteLine("Current Project from Registry: " + rememberProject);
			foreach (var project in model.AvailableProjects)
				Console.Write(project + " ");
		}

		[Test]
		public void CreateEditorPluginEntryFromLoadedPlugins()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			model.AddAllPlugins();
			Assert.AreEqual(1, model.EditorPlugins.Count);
			Assert.AreEqual("Mock Plugin", model.EditorPlugins[0].ShortName);
			Assert.AreEqual("Mock.png", model.EditorPlugins[0].Icon);
			Assert.AreEqual(typeof(MockEditorPluginView), model.EditorPlugins[0].GetType());
		}

		private static EditorPluginLoader GetEditorPluginLoaderMock()
		{
			var mockPlugins = new EditorPluginLoader(Path.Combine("..", "..", ".."));
			mockPlugins.UserControlsType.Clear();
			mockPlugins.UserControlsType.Add(typeof(MockEditorPluginView));
			return mockPlugins;
		}

		[Test]
		public void StartEditorFullscreenWhenNoRegistryKeyIsSetAndSaveStateInRegistry()
		{
			MakeSureToDeleteStartMaximizedRegistryEntry();
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			Assert.IsTrue(model.StartEditorMaximized);
		}

		private static void MakeSureToDeleteStartMaximizedRegistryEntry()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryPathForEditorValues, true))
				if (key != null)
					key.DeleteValue("StartMaximized", false);
		}

		private const string RegistryPathForEditorValues = @"Software\DeltaEngine\Editor";

		[Test]
		public void SaveAndLoadStartEditorFullscreenRegistryState()
		{
			MakeSureToDeleteStartMaximizedRegistryEntry();
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			model.StartEditorMaximized = false;
			Assert.IsFalse(model.StartEditorMaximized);
			model.StartEditorMaximized = true;
			Assert.IsTrue(model.StartEditorMaximized);
		}

		[Test]
		public void DefaultProjectToLoginWithIsSelectedIfUserHasNotLoggedInYetOrLoggedOut()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			model.SelectedProject = "";
			Assert.AreEqual("GhostWars", model.SelectedProject);
		}

		[Test]
		public void WhenContentIsNotReadyTheProjectSelectionComboBoxIsDisabled()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			Assert.AreEqual(false, model.IsContentReady);
		}

		[Test]
		public void PopUpNotificationForUpdatedContentDoesNotShowUpInitially()
		{
			var mockPlugins = GetEditorPluginLoaderMock();
			var model = new EditorViewModel(mockPlugins, new MockSettings());
			Assert.IsTrue(string.IsNullOrEmpty(model.PopupText));
			Assert.AreEqual(Visibility.Hidden, model.PopupVisibility);
		}
	}
}