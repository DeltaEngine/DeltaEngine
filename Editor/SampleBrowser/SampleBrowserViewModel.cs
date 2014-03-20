using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Used to display Sample Games, Tutorials and Tests of engine and user code.
	/// http://deltaengine.net/games/samplebrowser
	/// </summary>
	public class SampleBrowserViewModel : ViewModelBase
	{
		public SampleBrowserViewModel(Service service)
		{
			service.DataReceived += OnDataReceived;
			this.service = service;
			frameworks = new FrameworkFinder();
			Samples = new List<Sample>();
			AddSelectionFilters();
			RegisterCommands();
		}

		private void OnDataReceived(object message)
		{
			var projectInfo = message as ProjectInfoResult;
			if (projectInfo != null)
				UpdateSampleDataAndSort(projectInfo);
		}

		private void UpdateSampleDataAndSort(ProjectInfoResult projectInfo)
		{
			foreach (Sample sample in Samples)
			{
				if (sample.Name != projectInfo.Name)
					continue;
				if (!string.IsNullOrEmpty(projectInfo.Description))
					sample.Description = projectInfo.Description;
				sample.IsFeatured = projectInfo.IsFeatured;
				Samples = Samples.OrderByDescending(s => s.IsFeatured).ToList();
				break;
			}
		}

		private readonly Service service;
		private readonly FrameworkFinder frameworks;

		public List<Sample> Samples
		{
			get { return samples; }
			set
			{
				samples = value;
				RaisePropertyChanged("Samples");
			}
		}

		private List<Sample> samples;

		private void AddSelectionFilters()
		{
			AssembliesAvailable = new List<String>
			{
				"All",
				"Sample Games",
				"Tutorials",
				//"Visual Tests"
			};
			SelectedAssembly = AssembliesAvailable[1];
			FrameworksAvailable = frameworks.All;
			SelectedFramework = frameworks.Default;
		}

		public List<string> AssembliesAvailable { get; private set; }
		public string SelectedAssembly { get; set; }
		public DeltaEngineFramework[] FrameworksAvailable { get; private set; }

		public DeltaEngineFramework SelectedFramework { get; set; }

		private void RegisterCommands()
		{
			OnAssemblySelectionChanged = new RelayCommand(UpdateItems);
			OnFrameworkSelectionChanged = new RelayCommand(UpdateItems);
			OnSearchTextChanged = new RelayCommand(UpdateItems);
			OnSearchTextRemoved = new RelayCommand(ClearSearchFilter);
			OnHelpClicked = new RelayCommand(OpenHelpWebsite);
			OnViewButtonClicked = new RelayCommand<Sample>(ViewSourceCode, CanViewSourceCode);
			OnStartButtonClicked = new RelayCommand<Sample>(StartExecutable, CanStartExecutable);
		}

		public ICommand OnAssemblySelectionChanged { get; private set; }
		public ICommand OnFrameworkSelectionChanged { get; private set; }

		public void UpdateItems()
		{
			GetSamples();
			SelectItemsByAssemblyComboBoxSelection();
			FilterItemsBySearchBox();
			Samples = itemsToDisplay.OrderBy(s => s.ProjectFilePath).ToList();
			foreach (Sample sample in Samples)
				service.Send(new ProjectInfoRequest(sample.Name));
		}

		private void SelectItemsByAssemblyComboBoxSelection()
		{
			if (SelectedAssembly == AssembliesAvailable[0])
				itemsToDisplay = everything;
			else if (SelectedAssembly == AssembliesAvailable[1])
				itemsToDisplay = allSampleGames;
			else if (SelectedAssembly == AssembliesAvailable[2])
				itemsToDisplay = allTutorials;
			else if (SelectedAssembly == AssembliesAvailable[3])
				itemsToDisplay = allVisualTests;
		}

		private List<Sample> itemsToDisplay = new List<Sample>();
		private List<Sample> everything = new List<Sample>();
		private List<Sample> allSampleGames = new List<Sample>();
		private List<Sample> allTutorials = new List<Sample>();
		private List<Sample> allVisualTests = new List<Sample>();

		private void FilterItemsBySearchBox()
		{
			if (String.IsNullOrEmpty(SearchFilter))
				return;
			string filterText = SearchFilter;
			itemsToDisplay = itemsToDisplay.Where(item => item.ContainsFilterText(filterText)).ToList();
		}

		public ICommand OnSearchTextChanged { get; private set; }
		public ICommand OnSearchTextRemoved { get; private set; }

		private void ClearSearchFilter()
		{
			SearchFilter = "";
		}

		public string SearchFilter
		{
			get { return searchFilter; }
			set
			{
				searchFilter = value;
				RaisePropertyChanged("SearchFilter");
			}
		}

		private string searchFilter;

		public ICommand OnHelpClicked { get; private set; }

		private static void OpenHelpWebsite()
		{
			Process.Start("http://deltaengine.net/games/samplebrowser");
		}

		public ICommand OnViewButtonClicked { get; private set; }

		private static void ViewSourceCode(Sample sample)
		{
			SampleLauncher.OpenSolutionForProject(sample);
		}
		
		private static bool CanViewSourceCode(Sample sample)
		{
			return SampleLauncher.DoesProjectExist(sample);
		}

		public ICommand OnStartButtonClicked { get; private set; }

		private static void StartExecutable(Sample sample)
		{
			SampleLauncher.StartExecutable(sample);
		}

		private static bool CanStartExecutable(Sample sample)
		{
			return SampleLauncher.DoesAssemblyExist(sample);
		}

		private void GetSamples()
		{
			ClearEverything();
			var sampleCreator = new SampleCreator();
			sampleCreator.CreateSamples(SelectedFramework);
			if (!sampleCreator.UsingPrecompiledSamplesFromInstaller)
				SetFrameworkToDefault();
			foreach (var sample in sampleCreator.Samples)
				if (sample.Category == SampleCategory.Game)
					allSampleGames.Add(sample);
				else if (sample.Category == SampleCategory.Tutorial)
					allTutorials.Add(sample);
				else
					allVisualTests.Add(sample);
			AddEverythingTogether();
		}

		private void ClearEverything()
		{
			itemsToDisplay.Clear();
			everything.Clear();
			allSampleGames.Clear();
			allTutorials.Clear();
			allVisualTests.Clear();
		}

		private void SetFrameworkToDefault()
		{
			FrameworksAvailable = new[] { DeltaEngineFramework.Default };
			SelectedFramework = DeltaEngineFramework.Default;
			RaisePropertyChanged("FrameworksAvailable");
			RaisePropertyChanged("SelectedFramework");
		}

		public void AddEverythingTogether()
		{
			everything.AddRange(allSampleGames);
			everything.AddRange(allTutorials);
			everything.AddRange(allVisualTests);
		}

		public void SetAllSamples(List<Sample> list)
		{
			everything = list;
		}

		public void SetSampleGames(List<Sample> list)
		{
			allSampleGames = list;
		}

		public void SetVisualTests(List<Sample> list)
		{
			allVisualTests = list;
		}

		public List<Sample> GetItemsToDisplay()
		{
			return itemsToDisplay;
		}

		public void SetSelection(int index)
		{
			SelectedAssembly = AssembliesAvailable[index];
		}

		public void SetSearchText(string text)
		{
			SearchFilter = text;
		}
	}
}