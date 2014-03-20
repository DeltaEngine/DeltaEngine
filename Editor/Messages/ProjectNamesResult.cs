namespace DeltaEngine.Editor.Messages
{
	/// <summary>
	/// Names of accessible projects for the currently logged in user
	/// </summary>
	public class ProjectNamesResult
	{
		protected ProjectNamesResult() {} //ncrunch: no coverage

		public ProjectNamesResult(string[] privateProject, string[] publicProjects)
		{
			PrivateProjects = privateProject;
			PublicProjects = publicProjects;
		}

		public string[] PrivateProjects { get; private set; }
		public string[] PublicProjects { get; private set; }
	}
}