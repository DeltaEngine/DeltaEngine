using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using DeltaEngine.Core;
using DeltaEngine.Editor.Messages;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Manages all build warnings and build errors when building an app via the BuildService.
	/// </summary>
	public class AppBuildMessagesListViewModel : ViewModelBase
	{
		public AppBuildMessagesListViewModel()
		{
			Warnings = new List<AppBuildMessage>();
			Errors = new List<AppBuildMessage>();
			IsShowingErrorsAllowed = true;
			IsShowingWarningsAllowed = false;
		}

		public List<AppBuildMessage> Warnings { get; set; }
		public List<AppBuildMessage> Errors { get; set; }

		public bool IsShowingErrorsAllowed
		{
			get { return isShowingErrorsAllowed; }
			set
			{
				isShowingErrorsAllowed = value;
				RaisePropertyChanged("IsShowingErrorsAllowed");
				TriggerMatchingCurrentFilterChanged();
			}
		}

		private void TriggerMatchingCurrentFilterChanged()
		{
			RaisePropertyChanged("MessagesMatchingCurrentFilter");
		}

		private bool isShowingErrorsAllowed;

		public bool IsShowingWarningsAllowed
		{
			get { return isShowingWarningsAllowed; }
			set
			{
				isShowingWarningsAllowed = value;
				RaisePropertyChanged("IsShowingWarningsAllowed");
				TriggerMatchingCurrentFilterChanged();
			}
		}

		private bool isShowingWarningsAllowed;

		public string TextOfErrorCount
		{
			get { return "Error".GetCountAndWordInPluralIfNeeded(Errors.Count); }
		}

		public string TextOfWarningCount
		{
			get
			{
				return "Warning".GetCountAndWordInPluralIfNeeded(Warnings.Count);
			}
		}

		public void AddMessage(AppBuildMessage message)
		{
			if (message.Type == AppBuildMessageType.BuildError)
				AddMessageToErrors(message);
			else
				AddMessageToWarnings(message);
			TriggerMatchingCurrentFilterChanged();
		}

		private void AddMessageToErrors(AppBuildMessage message)
		{
			Errors.Add(message);
			RaisePropertyChanged("TextOfErrorCount");
		}

		private void AddMessageToWarnings(AppBuildMessage message)
		{
			Warnings.Add(message);
			RaisePropertyChanged("TextOfWarningCount");
		}

		public List<AppBuildMessageViewModel> MessagesMatchingCurrentFilter
		{
			get
			{
				var messages = new List<AppBuildMessageViewModel>();
				if (IsShowingWarningsAllowed)
					AddMessageToViewModelList(Warnings, messages);
				if (IsShowingErrorsAllowed)
					AddMessageToViewModelList(Errors, messages);
				messages.Sort(SortByTimeStamp);
				return messages;
			}
		}

		private static void AddMessageToViewModelList(List<AppBuildMessage> messageList,
			List<AppBuildMessageViewModel> messageViewModelList)
		{
			foreach (AppBuildMessage message in messageList)
				messageViewModelList.Add(new AppBuildMessageViewModel(message));
		}

		private static int SortByTimeStamp(AppBuildMessageViewModel message,
			AppBuildMessageViewModel other)
		{
			return message.MessageData.TimeStamp.CompareTo(other.MessageData.TimeStamp);
		}

		public void ClearMessages()
		{
			Warnings.Clear();
			Errors.Clear();
			TriggerMatchingCurrentFilterChanged();
			RaisePropertyChanged("TextOfErrorCount");
			RaisePropertyChanged("TextOfWarningCount");
		}

		// ncrunch: no coverage start
		public void CopyMessageToClipboard(AppBuildMessageViewModel message)
		{
			SetCurrentTextInClipbard(message.ToString());
		}

		private static void SetCurrentTextInClipbard(string text)
		{
			// Clipboard access must be executed on a STA thread
			var staThread = new Thread(() => SetClipboardText(text));
			staThread.SetApartmentState(ApartmentState.STA);
			staThread.Start();
			staThread.Join();
		}

		private static void SetClipboardText(string text)
		{
			try
			{
				TrySetClipboardText(text);
			}
			catch (Exception)
			{
				Logger.Warning("Failed to access Clipboard text.");
			}
		}

		private static void TrySetClipboardText(string text)
		{
			Clipboard.SetText(text);
		}
	}
}