using CreepyTowers.Avatars;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class PlayerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Initialize()
		{
			player = new Player("mYsT");
		}

		private Player player;

		[Test, CloseAfterFirstFrame]
		public void CheckPlayerName()
		{
			Assert.AreEqual("mYsT", player.Name);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckPlayerDragonAvatar()
		{

			Assert.AreEqual(typeof(Dragon), player.Avatar.GetType());
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Avatar>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckPlayerPenguinAvatar()
		{
			player.UnlockAvatar(CreepyTowers.Content.Avatars.Penguin);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.Penguin);
			Assert.AreEqual(typeof(Penguin), player.Avatar.GetType());
		}

		[Test, CloseAfterFirstFrame]
		public void CheckPlayerPiggyBankAvatar()
		{
			player.UnlockAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			Assert.AreEqual(typeof(PiggyBank), player.Avatar.GetType());
		}

		[Test, CloseAfterFirstFrame]
		public void TotalPlayerXpIsSumOfAvatarXp()
		{
			player.Avatar.Xp = 10;
			Assert.AreEqual(10, player.Avatar.Xp);
			player.UnlockAvatar(CreepyTowers.Content.Avatars.Penguin);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.Penguin);
			player.Avatar.Xp = 25;
			Assert.AreEqual(25, player.Avatar.Xp);
			Assert.AreEqual(35, player.Xp);
		}

		[Test, CloseAfterFirstFrame]
		public void TotalPlayerLevelIsSumOfAvatarLevels()
		{
			player.Avatar.ProgressLevel = 2;
			Assert.AreEqual(2, player.Avatar.ProgressLevel);
			player.UnlockAvatar(CreepyTowers.Content.Avatars.Penguin);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.Penguin);
			player.Avatar.ProgressLevel = 4;
			Assert.AreEqual(4, player.Avatar.ProgressLevel);
			Assert.AreEqual(6, player.ProgressLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void SwitchingBackToOldAvatarRestoresOldAvatarAndData()
		{
			player.Avatar.Xp = 10;
			player.Avatar.ProgressLevel = 2;
			player.UnlockAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			player.ChangeAvatar(CreepyTowers.Content.Avatars.PiggyBank);
			player.Avatar.Xp = 25;
			player.Avatar.ProgressLevel = 4;
			player.ChangeAvatar(CreepyTowers.Content.Avatars.Dragon);
			Assert.AreEqual(10, player.Avatar.Xp);
			Assert.AreEqual(2, player.Avatar.ProgressLevel);
		}

		[Test, CloseAfterFirstFrame]
		public void InitializingMaxLivesAlsoInitializedLivesLeftForPlayer()
		{
			player.MaxLives = 10;
			Assert.AreEqual(10, player.MaxLives);
			Assert.AreEqual(10, player.LivesLeft);
		}
	}
}