namespace DeltaEngine.Editor.Messages
{
	public class ProjectError
	{
		private ProjectError() {}

		public ProjectError(string errorMessage, string projectName)
		{
			ErrorMessage = errorMessage;
			ProjectName = projectName;
		}

		public string ErrorMessage { get; private set; }

		public string ProjectName { get; private set; }
	}
}