using System;
using System.Timers;
using System.Windows;
using DeltaEngine.Content;
using DeltaEngine.Editor.Core;

namespace DeltaEngine.Editor
{
	public class PopupMessageViewModel
	{
		public PopupMessageViewModel(Service service, int messageDisplayTime = 3000)
		{
			Text = "";
			Visiblity = Visibility.Hidden;
			service.ContentUpdated += ShowUpdateText;
			messageTimer = new Timer(messageDisplayTime);
			messageTimer.Elapsed += HideUpdateText;
		}

		public string Text { get; private set; }
		public Visibility Visiblity { get; private set; }

		private void ShowUpdateText(ContentType type, string name)
		{
			Visiblity = Visibility.Visible;
			Text = name + " " + type + " Updated!";
			FireVisibilityUpdatedAction();
			if (messageTimer.Enabled)
				ResetTimer();
			else
				messageTimer.Start();
		}

		private readonly Timer messageTimer;

		private void FireVisibilityUpdatedAction()
		{
			if (MessageUpdated != null)
				MessageUpdated();
		}

		public event Action MessageUpdated;

		private void ResetTimer()
		{
			messageTimer.Stop();
			messageTimer.Start();
		}

		private void HideUpdateText(object sender, ElapsedEventArgs e)
		{
			Visiblity = Visibility.Hidden;
			FireVisibilityUpdatedAction();
		}
	}
}