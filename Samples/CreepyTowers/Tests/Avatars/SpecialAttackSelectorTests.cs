using CreepyTowers.Avatars;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Avatars
{
	public class SpecialAttackSelectorTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			ContentLoader.Load<GameLevel>("DummyLevelInfo");
		}

		[Test]
		public void DragonBreathOfFire()
		{
			var player = new Player();
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.DragonBreathOfFire;
			player.Avatar.SpecialAttackAIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackAIsActivated);
		}

		[Test]
		public void DragonAuraCannon()
		{
			var player = new Player();
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.DragonAuraCannon;
			player.Avatar.SpecialAttackBIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackBIsActivated);
		}

		[Test]
		public void PenguinBigFirework()
		{
			var player = new Player("", CreepyTowers.Content.Avatars.Penguin);
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.PenguinBigFirework;
			player.Avatar.SpecialAttackAIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackAIsActivated);
		}

		[Test, Timeout(5000)]
		public void PenguinCarpetBombing()
		{
			var player = new Player("", CreepyTowers.Content.Avatars.Penguin);
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.PenguinCarpetBombing;
			player.Avatar.SpecialAttackBIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackBIsActivated);
		}

		[Test]
		public void PiggyBankCoinMinefield()
		{
			var player = new Player("", CreepyTowers.Content.Avatars.PiggyBank);
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.PiggyBankCoinMinefield;
			player.Avatar.SpecialAttackAIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackAIsActivated);
		}

		[Test]
		public void PiggyBankPayDay()
		{
			var player = new Player("", CreepyTowers.Content.Avatars.PiggyBank);
			player.Avatar.ActivatedSpecialAttack = AvatarAttack.PiggyBankPayDay;
			player.Avatar.SpecialAttackBIsActivated = true;
			SpecialAttackSelector.SelectAttack(Vector2D.One);
			Assert.IsFalse(player.Avatar.SpecialAttackBIsActivated);
		}
	}
}