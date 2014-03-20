using DeltaEngine.Datatypes;

namespace $safeprojectname$
{
	public enum Team
	{
		None,
		HumanYellow,
		ComputerPurple,
		ComputerTeal,
	}

	public static class TeamExtensions
	{
		public static Color ToColor(this Team team)
		{
			return team == Team.None
				? Color.Gray
				: team == Team.HumanYellow
					? GameLogic.TeamHumanYellowColor
					: team == Team.ComputerPurple
						? GameLogic.TeamComputerPurpleColor : GameLogic.TeamComputerTealColor;
		}
	}
}