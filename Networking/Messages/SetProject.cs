namespace DeltaEngine.Networking.Messages
{
	/// <summary>
	/// Server sends this to the client initially after Login to give the client the project name
	/// </summary>
	public class SetProject
	{
		private SetProject() {}

		public SetProject(string name, ProjectPermissions permissions)
		{
			Name = name;
			Permissions = permissions;
		}

		public string Name { get; private set; }
		public ProjectPermissions Permissions { get; private set; }
	}
}