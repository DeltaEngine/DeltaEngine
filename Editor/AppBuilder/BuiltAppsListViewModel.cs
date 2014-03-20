using System;
using System.IO;
using DeltaEngine.Core;
using GalaSoft.MvvmLight;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// The ViewModel for BuiltAppsListView which manages all existing apps that were built via the
	/// BuildService.
	/// </summary>
	public class BuiltAppsListViewModel : ViewModelBase
	{
		public BuiltAppsListViewModel(Settings editorSettings)
		{
			appsStorage = new AppsStorage(editorSettings);
		}

		private readonly AppsStorage appsStorage;

		public string AppStorageDirectory
		{
			get { return appsStorage.StorageDirectory; }
		}

		public AppInfo[] BuiltApps
		{
			get { return appsStorage.AvailableApps; }
		}

		public int NumberOfBuiltApps
		{
			get { return appsStorage.AvailableApps.Length; }
		}

		public void AddApp(AppInfo appInfo, byte[] appData = null)
		{
			if (appInfo == null)
				throw new NoAppInfoSpecified();
			appsStorage.AddApp(appInfo);
			RaisePropertyChangedOfBuiltApps();
			if (appData != null)
				SaveBuiltApp(appInfo, appData);
		}

		public class NoAppInfoSpecified : Exception {}

		private void RaisePropertyChangedOfBuiltApps()
		{
			RaisePropertyChanged("BuiltApps");
			RaisePropertyChanged("TextOfBuiltApps");
			RaiseNumberOfBuiltAppsChangedEvent();
		}

		private void SaveBuiltApp(AppInfo appInfo, byte[] appData)
		{
			try
			{
				TrySaveBuiltApp(appInfo, appData);
			}
			// ncrunch: no coverage start
			catch (Exception ex)
			{
				throw new SavingBuiltAppFailed(ex);
			}
			// ncrunch: no coverage end
		}

		private void TrySaveBuiltApp(AppInfo appInfo, byte[] appData)
		{
			if (!Directory.Exists(AppStorageDirectory))
				Directory.CreateDirectory(AppStorageDirectory); // ncrunch: no coverage
			string appSavePath = Path.Combine(AppStorageDirectory, Path.GetFileName(appInfo.FilePath));
			File.WriteAllBytes(appSavePath, appData);
		}

		// ncrunch: no coverage start
		public class SavingBuiltAppFailed: Exception
		{
			public SavingBuiltAppFailed(Exception reason)
				: base(reason.Message, reason) { }
		}
		// ncrunch: no coverage end

		public string TextOfBuiltApps
		{
			get { return "App".GetCountAndWordInPluralIfNeeded(NumberOfBuiltApps); }
		}

		public void RequestRebuild(AppInfo app)
		{
			if (RebuildRequest != null)
				RebuildRequest(app);
		}

		public event Action<AppInfo> RebuildRequest;

		private void RaiseNumberOfBuiltAppsChangedEvent()
		{
			if (NumberOfBuiltAppsChanged != null)
				NumberOfBuiltAppsChanged();
		}

		public event Action NumberOfBuiltAppsChanged;
	}
}