using System;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Entities;

namespace CreepyTowers.Tests
{
	public sealed class MockGame : Entity
	{
		public MockGame()
		{
			Add(new InGameCommands());
		}

		public void MessageInsufficientMoney(int amountRequired)
		{
			if (InsufficientCredits != null)
				InsufficientCredits(amountRequired);
		}

		public event Action<int> InsufficientCredits;

		public void MessageCreditsUpdated(int difference)
		{
			if (CreditsUpdated != null)
				CreditsUpdated(difference);
		}

		public event Action<int> CreditsUpdated;

		public override void Dispose()
		{
			base.Dispose();
			foreach (Creep creep in EntitiesRunner.Current.GetEntitiesOfType<Creep>())
				creep.Dispose();
			foreach (Tower tower in EntitiesRunner.Current.GetEntitiesOfType<Tower>())
				tower.Dispose();
		}

		public static void ExitGame()
		{
			var allEntities = EntitiesRunner.Current.GetAllEntities();
			foreach (Entity entity in allEntities)
				entity.IsActive = false;
		}
	}
}
