namespace DeltaEngine.Editor.Messages
{
	public class DeleteProject
	{
		private DeleteProject() {}

		public DeleteProject(string projectName)
		{
			ProjectName = projectName;
		}

		public string ProjectName { get; private set; }
	}
}