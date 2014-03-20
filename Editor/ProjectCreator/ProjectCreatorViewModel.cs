using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DeltaEngine.Editor.ProjectCreator
{
	public class ProjectCreatorViewModel : ViewModelBase
	{
		public ProjectCreatorViewModel(Service service)
		{
			this.service = service;
			this.service.DataReceived += OnDataReceived;
			project = new CsProject(service.UserName);
			AvailableStarterKits = VsTemplate.GetAllTemplateNames(project.Framework);
			AvailableFrameworks = StackTraceExtensions.IsStartedFromNCrunch()
				? new FrameworkFinderSpy().All : new FrameworkFinder().All;
			RegisterCommands();
		}

		private readonly Service service;

		private void OnDataReceived(object message)
		{
			var createProject = message as CreateProject;
			var alreadyExists = message as ProjectAlreadyExists;
			if (createProject != null ||
				alreadyExists != null && alreadyExists.ProjectName == project.Name)
				OpenProject(createProject, alreadyExists); //ncrunch: no coverage
		}

		//ncrunch: no coverage start
		private void OpenProject(CreateProject createProject, ProjectAlreadyExists alreadyExists)
		{
			if (createProject != null)
				Logger.Info("Content Project " + createProject.ProjectName + " has been created");
			else
				MessageBox.Show(
					"Content Project " + alreadyExists.ProjectName +
						" already exists, will use existing project", "Server reported", MessageBoxButton.OK);
			var projectName = createProject != null
				? createProject.ProjectName : alreadyExists.ProjectName;
			Process.Start(service.GetAbsoluteSolutionFilePath(projectName));
		} //ncrunch: no coverage end

		private readonly CsProject project;
		public string[] AvailableStarterKits { get; private set; }
		public DeltaEngineFramework[] AvailableFrameworks { get; private set; }

		private void RegisterCommands()
		{
			OnNameChanged = new RelayCommand<string>(ChangeName);
			OnFrameworkSelectionChanged = new RelayCommand<int>(ChangeSelection);
			OnLocationChanged = new RelayCommand<string>(ChangeLocation);
			OnCreateClicked = new RelayCommand(CreateProject, CanCreateProject);
		}

		public ICommand OnNameChanged { get; private set; }

		private void ChangeName(string projectName)
		{
			Name = projectName;
		}

		public string Name
		{
			get { return project.Name; }
			set
			{
				project.Name = value;
				RaisePropertyChanged("Name");
			}
		}

		public ICommand OnFrameworkSelectionChanged { get; private set; }

		private void ChangeSelection(int index)
		{
			SelectedFramework = (DeltaEngineFramework)index;
		}

		public DeltaEngineFramework SelectedFramework
		{
			get { return project.Framework; }
			set
			{
				project.Framework = value;
				RaisePropertyChanged("SelectedFramework");
			}
		}

		public string SelectedStarterKit
		{
			get { return project.StarterKit; }
			set
			{
				project.StarterKit = value;
				RaisePropertyChanged("SelectedStarterKit");
				RaisePropertyChanged("Name");
			}
		}

		public ICommand OnLocationChanged { get; private set; }

		private void ChangeLocation(string projectPath)
		{
			Location = projectPath;
		}

		public string Location
		{
			get { return project.BaseDirectory; }
			set
			{
				project.BaseDirectory = value;
				RaisePropertyChanged("Location");
			}
		}

		public ICommand OnCreateClicked { get; private set; }

		private void CreateProject()
		{
			try
			{
				TryCreateProjectAndSendToService();
			}
			catch (Exception ex) //ncrunch: no coverage start
			{
				MessageBox.Show(
					ex + "\n\nPlease reinstall and make sure the specified location and the " +
						"VisualStudioTemplates are available.", "Project could not be created");
			} //ncrunch: no coverage end
		}

		private void TryCreateProjectAndSendToService()
		{
			var projectCreator = new ProjectCreator(project, new VsTemplate(project.StarterKit), service);
			projectCreator.CreateProject();
			projectCreator.CheckIfTemplateFilesHaveBeenCopiedToLocation();
			service.Send(new CreateProject(projectCreator.Project.Name, project.StarterKit));
		}

		private bool CanCreateProject()
		{
			return InputValidator.IsValidProjectName(Name) &&
				InputValidator.IsValidAbsolutePath(Location);
		}
	}
}