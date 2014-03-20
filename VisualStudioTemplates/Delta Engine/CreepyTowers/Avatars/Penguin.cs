using System.Collections.Generic;
using CreepyTowers.Towers;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;

namespace $safeprojectname$.Avatars
{
	/// <summary>
	/// Flying Penguin is a master of speed. Special abilities are: 
	/// Rapid Recharge: Increases the attack frequency by a third
	/// Carpet Bombing: Multiple bombs fall across the grid at random
	/// Big Firework: Single massive bomb falls at a player directed spot
	/// </summary>
	public class Penguin : Avatar
	{
		public Penguin()
		{
			IsLocked = false;
			AddTag(Content.Avatars.Penguin.ToString());
			AttackFrequencyMultiplier = 1.3333f; // Therefore reduces cooldown by a quarter
		}

		public override void PerformAttack(AvatarAttack attack, Vector2D position)
		{
			if (attack == AvatarAttack.PenguinBigFirework)
				PerformBigFireworkAttack(position);
			else if (attack == AvatarAttack.PenguinCarpetBombing)
				PerformCarpetBombingAttack();
		}

		private static void PerformBigFireworkAttack(Vector2D position)
		{
			var creepsWithinRange = GetCreepsWithinRange(position, BigFireworkRange);
			foreach (var creep in creepsWithinRange)
				creep.ReceiveAttack(TowerType.Impact, BigFireworkDamage);
		}

		private const float BigFireworkRange = 3.0f;
		private const float BigFireworkDamage = 10.0f;

		private void PerformCarpetBombingAttack()
		{
			var width = (int)Level.Current.Size.Width;
			var height = (int)Level.Current.Size.Height;
			var bombCount = (int)(width * height * BombPercentage);
			bombLocations.Clear();
			do
			{
				DropBomb(new Vector2D(Randomizer.Current.Get(0, width), Randomizer.Current.Get(0, height)));
			} while (bombLocations.Count < bombCount);
		}

		private const float BombPercentage = 0.1f;
		private readonly List<Vector2D> bombLocations = new List<Vector2D>();

		private void DropBomb(Vector2D gridPosition)
		{
			if (bombLocations.Contains(gridPosition))
				return;
			bombLocations.Add(gridPosition);
			var creepsWithinRange = GetCreepsWithinRange(gridPosition + Vector2D.Half, CarpetBombingRange);
			foreach (var creep in creepsWithinRange)
				creep.ReceiveAttack(TowerType.Ice, CarpetBombingDamage);
		}

		private const float CarpetBombingRange = 1.0f;
		private const float CarpetBombingDamage = 10.0f;
	}
}