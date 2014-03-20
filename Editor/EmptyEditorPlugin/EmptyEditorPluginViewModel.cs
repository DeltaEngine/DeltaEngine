using System.Windows.Input;
using DeltaEngine.Editor.Core.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.EmptyEditorPlugin
{
	public class EmptyEditorPluginViewModel : ViewModelBase
	{
		public EmptyEditorPluginViewModel()
		{
			ClickCommand = new RelayCommand(Click);
		}

		public ICommand ClickCommand { get; private set; }

		private void Click()
		{
			button.Text = Resources.Clicked;
			RaisePropertyChanged("Button");
		}

		private readonly MyButton button = new MyButton();

		public MyButton Button
		{
			get { return button; }
		}
	}
}