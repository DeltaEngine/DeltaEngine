using System.Windows.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.InputEditor
{
	/// <summary>
	/// Interaction logic for TriggerLayoutView.xaml
	/// </summary>
	public partial class TriggerLayoutView
	{
		//ncrunch: no coverage start
		public TriggerLayoutView()
		{
			InitializeComponent();
		}

		private void ChangeKey(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0)
				return;

			var inputLayoutData = new TriggerLayoutData
			{
				AddingItem = e.AddedItems[0].ToString(),
				RemovingItem = e.RemovedItems[0].ToString(),
				NumberOfAddingItems = e.AddedItems.Count,
				NumberOfRemovingItems = e.RemovedItems.Count
			};
			Messenger.Default.Send(inputLayoutData, "KeyChanged");
		}

		private void ChangeState(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0)
				return;

			var inputLayoutData = new TriggerLayoutData
			{
				AddingItem = e.AddedItems[0].ToString(),
				RemovingItem = e.RemovedItems[0].ToString(),
				NumberOfAddingItems = e.AddedItems.Count,
				NumberOfRemovingItems = e.RemovedItems.Count,
				KeyBox = TriggerKey,
				TypeBox = TriggerType
			};
			Messenger.Default.Send(inputLayoutData, "StateChanged");
		}

		private void ChangeType(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0)
				return;

			var inputLayoutData = new TriggerLayoutData
			{
				AddingItem = e.AddedItems[0].ToString(),
				RemovingItem = e.RemovedItems[0].ToString(),
				NumberOfAddingItems = e.AddedItems.Count,
				NumberOfRemovingItems = e.RemovedItems.Count,
				KeyBox = TriggerKey
			};
			Messenger.Default.Send(inputLayoutData, "TypeChanged");
		}
	}
}