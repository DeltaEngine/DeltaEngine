using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GhostWars.Tests
{
	public class GameTests : TestWithMocksOrVisually
	{
		[Test]
		public void SendGhostsFromOneTreeToAnother()
		{
			var trees = new TreeManager(Team.HumanYellow);
			trees.AddTree(new Vector2D(0.75f, 0.4f), Team.HumanYellow);
			trees.AddTree(new Vector2D(0.25f, 0.55f), Team.None);
		}
		
		[Test]
		public void ShortTreeDistance()
		{
			var trees = new TreeManager(Team.HumanYellow);
			trees.AddTree(new Vector2D(0.35f, 0.4f), Team.HumanYellow);
			trees.AddTree(new Vector2D(0.65f, 0.5f), Team.None);
		}
	}
}