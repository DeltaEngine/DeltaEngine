using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuildMessagesListViewModelTests
	{
		[TestFixtureSetUp]
		public void SetLogger()
		{
			new ConsoleLogger();
		}

		[Test]
		public void AddDifferentMessages()
		{
			var messagesList = new AppBuildMessagesListViewModel();
			messagesList.AddMessage(
				AppBuilderTestExtensions.AsBuildTestWarning("A test warning for this test"));
			messagesList.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("A test error for this test"));
			messagesList.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("Just another test error for this test"));
			Assert.AreEqual(1, messagesList.Warnings.Count);
			Assert.AreEqual("1 Warning", messagesList.TextOfWarningCount);
			Assert.AreEqual(2, messagesList.Errors.Count);
			Assert.AreEqual("2 Errors", messagesList.TextOfErrorCount);
		}

		[Test]
		public void OnlyShowingErrorFilter()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = false;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		private static AppBuildMessagesListViewModel GetViewModelWithOneMessageForEachIssueType()
		{
			var messagesList = new AppBuildMessagesListViewModel();
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestWarning("Test warning"));
			messagesList.AddMessage(AppBuilderTestExtensions.AsBuildTestError("Test error"));
			return messagesList;
		}

		[Test]
		public void OnlyShowingWarningFilter()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			messagesList.IsShowingErrorsAllowed = false;
			messagesList.IsShowingWarningsAllowed = true;
			Assert.AreEqual(1, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void ShowingAllKindsOfMessages()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;
			Assert.AreEqual(2, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test]
		public void CheckMessagesMatchingCurrentFilterOrder()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			messagesList.IsShowingErrorsAllowed = true;
			messagesList.IsShowingWarningsAllowed = true;

			IList<AppBuildMessageViewModel> messages = messagesList.MessagesMatchingCurrentFilter;
			DateTime timeStampOfFirstElement = messages[0].MessageData.TimeStamp;
			for (int i = 1; i < messages.Count; i++)
				Assert.IsTrue(messages[i].MessageData.TimeStamp >= timeStampOfFirstElement);
		}

		[Test]
		public void ClearMessages()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			messagesList.ClearMessages();
			Assert.AreEqual(0, messagesList.MessagesMatchingCurrentFilter.Count);
		}

		[Test, Ignore]
		// ncrunch: no coverage start
		public void CheckCopyingSpecificMessageToSystemClipboad()
		{
			AppBuildMessagesListViewModel messagesList = GetViewModelWithOneMessageForEachIssueType();
			IList<AppBuildMessageViewModel> messages = messagesList.MessagesMatchingCurrentFilter;
			Assert.GreaterOrEqual(messages.Count, 1);
			foreach (AppBuildMessageViewModel message in messages)
			{
				messagesList.CopyMessageToClipboard(message);
				Assert.AreEqual(message.ToString(), GetCurrentTextInClipbard());
			}
		}

		private static string GetCurrentTextInClipbard()
		{
			// Clipboard access must be executed on a STA thread
			string clipboardText = "";
			var staThread = new Thread(() => clipboardText = GetClipboardText());
			staThread.SetApartmentState(ApartmentState.STA);
			staThread.Start();
			staThread.Join();
			return clipboardText;
		}

		private static string GetClipboardText()
		{
			try
			{
				return TryGetClipboardText();
			}
			catch (Exception)
			{
				Logger.Warning("Failed to access Clipboard text.");
				return "";
			}
		}

		private static string TryGetClipboardText()
		{
			return Clipboard.GetText();
		}
	}
}