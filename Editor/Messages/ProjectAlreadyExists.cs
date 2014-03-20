namespace DeltaEngine.Editor.Messages
{
	public class ProjectAlreadyExists
	{
		private ProjectAlreadyExists() {}

		public ProjectAlreadyExists(string projectName)
		{
			ProjectName = projectName;
		}

		public string ProjectName { get; private set; }
	}
}