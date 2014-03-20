using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.ContentManager
{
	/// <summary>
	/// Interaction logic for ContentIconAndNameView.xaml
	/// </summary>
	public partial class ContentIconAndNameView
	{
		//ncrunch: no coverage start
		public ContentIconAndNameView()
		{
			InitializeComponent();
		}

		private void ClickOnElement(object sender, MouseButtonEventArgs e)
		{
			if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.RightButton != MouseButtonState.Pressed)
				Messenger.Default.Send(ContentName.Text, "AddToSelection");
			else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
				Messenger.Default.Send(ContentName.Text, "AddMultipleContentToSelection");
			else if (e.RightButton == MouseButtonState.Pressed)
			{
				Messenger.Default.Send(ContentName.Text, "SelectToDelete");
			}
			else if (e.LeftButton == MouseButtonState.Pressed)
			{
				Messenger.Default.Send("ClearList", "ClearList");
				Messenger.Default.Send(ContentName.Text, "AddToSelection");
			}
		}
	}
}