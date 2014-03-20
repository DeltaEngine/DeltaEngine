namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Request to create Content Project in Content Service, command to create code project on client
	/// </summary>
	public class CreateProject
	{
		protected CreateProject() {} //ncrunch: no coverage

		public CreateProject(string projectName, string starterKit)
		{
			ProjectName = projectName;
			StarterKit = starterKit;
		}

		public string ProjectName { get; private set; }
		public string StarterKit { get; private set; }
	}
}