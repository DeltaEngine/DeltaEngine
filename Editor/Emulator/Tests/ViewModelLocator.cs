using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace DeltaEngine.Editor.Emulator.Tests
{
	/// <summary>
	/// References to an empty view model and provides an entry point for the bindings for testing.
	/// </summary>
	public class ViewModelLocator
	{
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
			SimpleIoc.Default.Register<EmptyMainViewModel>();
		}

		public class EmptyMainViewModel : ViewModelBase {}

		public EmptyMainViewModel Main
		{
			get { return ServiceLocator.Current.GetInstance<EmptyMainViewModel>(); }
		}
	}
}