namespace DeltaEngine.Editor.Messages
{
	public class ProjectInfoRequest
	{
		private ProjectInfoRequest() {}

		public ProjectInfoRequest(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}