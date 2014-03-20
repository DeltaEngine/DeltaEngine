namespace DeltaEngine.GameLogic
{
	public class Achievement
	{
		public Achievement(string achievementName, string description)
		{
			Name = achievementName;
			Description = description;
			IsAchieved = false;
		}

		public string Name { get; protected set; }
		public string Description { get; protected set; }
		public bool IsAchieved { get; set; }
	}
}
