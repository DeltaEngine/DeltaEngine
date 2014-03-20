using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace GhostWars.Tests
{
	class TreeTests : TestWithMocksOrVisually
	{
		[Test]
		public void TreeStartsOffBeingActive()
		{
			var humanTree = GiveTreeOfTeam();
			Assert.IsTrue(humanTree.IsActive);
		}

		private static Tree GiveTreeOfTeam(Team team = Team.None)
		{
			return new Tree(Vector2D.Half, team);
		}

		[Test]
		public void HumanYellowTreeIsNotAiOthersAre()
		{
			var humanTree = GiveTreeOfTeam(Team.HumanYellow);
			Assert.IsFalse(humanTree.IsAi);
			var tealAiTree = GiveTreeOfTeam(Team.ComputerTeal);
			Assert.IsTrue(tealAiTree.IsAi);
			var purpleAiTree = GiveTreeOfTeam(Team.ComputerPurple);
			Assert.IsTrue(purpleAiTree.IsAi);
		}

		[Test]
		public void AdvancingByGrowthIntervalIncreasesGhosts()
		{
			var tree = GiveTreeOfTeam(Team.ComputerTeal);
			var originalGhostCount = tree.NumberOfGhosts;
			MainMenu.State = GameState.Game;
			AdvanceTimeAndUpdateEntities(2);
			Assert.AreEqual(originalGhostCount + 1, tree.NumberOfGhosts);
		}

		[Test]
		public void AttackEmptyTreeCostsUsedGhostsAndTurnsTeam()
		{
			var tree = GiveTreeOfTeam();
			tree.Attack(Team.HumanYellow, 5);
			Assert.AreEqual(Team.HumanYellow, tree.CurrentTeam);
			Assert.AreEqual(0, tree.NumberOfGhosts);
		}

		[Test]
		public void AttackSameTeamTreeAddsGhosts()
		{
			var tree = GiveTreeOfTeam(Team.HumanYellow);
			var originalCount = tree.NumberOfGhosts;
			var waveGhosts = 4;
			tree.Attack(Team.HumanYellow, waveGhosts);
			Assert.AreEqual(originalCount + waveGhosts, tree.NumberOfGhosts);
		}

		[Test]
		public void AttackEnemyCountered()
		{
			var tree = GiveTreeOfTeam(Team.HumanYellow);
			tree.Level = 2;
			const int OriginalCount = 60;
			tree.NumberOfGhosts = OriginalCount;
			const int WaveCount = 5;
			tree.Attack(Team.ComputerTeal, WaveCount);
			Assert.AreEqual(Team.HumanYellow, tree.CurrentTeam);
		}

		[Test]
		public void AttackEnemySuccessfullyConquering()
		{
			var tree = GiveTreeOfTeam(Team.ComputerPurple);
			const int OriginalCount = 2;
			tree.NumberOfGhosts = OriginalCount;
			const int WaveCount = 5;
			tree.Attack(Team.HumanYellow, WaveCount);
			Assert.AreEqual(Team.HumanYellow, tree.CurrentTeam);
			Assert.AreEqual(0, tree.NumberOfGhosts);
		}

		[Test]
		public void AttackPlayerSuccessfullyConquering()
		{
			var tree = GiveTreeOfTeam(Team.HumanYellow);
			const int OriginalCount = 2;
			tree.NumberOfGhosts = OriginalCount;
			const int WaveCount = 5;
			tree.Attack(Team.ComputerTeal, WaveCount);
			Assert.AreEqual(Team.ComputerTeal, tree.CurrentTeam);
			Assert.AreEqual(0, tree.NumberOfGhosts);
		}

		[Test]
		public void UpgradePlayerTree()
		{
			var tree = GiveTreeOfTeam(Team.HumanYellow);
			tree.NumberOfGhosts = GameLogic.GhostsToUpgrade;
			tree.TryToUpgrade();
			Assert.AreEqual(2, tree.Level);
		}

		[Test]
		public void CannotUpgradeWithoutRequiredGhostCount()
		{
			var tree = GiveTreeOfTeam(Team.HumanYellow);
			tree.NumberOfGhosts = GameLogic.GhostsToUpgrade / 2;
			tree.TryToUpgrade();
			Assert.AreEqual(1, tree.Level);
		}

		[Test]
		public void SettingInactiveDeactivatesCounterText()
		{
			var tree = GiveTreeOfTeam();
			tree.IsActive = false;
			Assert.IsFalse(tree.NumberText.IsActive);
		}

		[Test]
		public void CannotSetGhostsGreaterThanLevelMax()
		{
			var tree = GiveTreeOfTeam();
			tree.NumberOfGhosts = 40;
			Assert.AreEqual(GameLogic.GhostsToUpgrade, tree.NumberOfGhosts);
		}
	}
}
