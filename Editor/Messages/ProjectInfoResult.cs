namespace DeltaEngine.Editor.Messages
{
	public class ProjectInfoResult
	{
		private ProjectInfoResult() {}

		public ProjectInfoResult(string name, string description, bool isFeatured)
		{
			Name = name;
			Description = description;
			IsFeatured = isFeatured;
		}

		public string Name { get; private set; }
		public string Description { get; private set; }
		public bool IsFeatured { get; private set; }
	}
}