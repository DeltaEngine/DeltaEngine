using System;
using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Avatars;
using DeltaEngine.Entities;

namespace CreepyTowers
{
	public sealed class Player : Entity
	{
		public Player(string name = "", Content.Avatars avatar = Content.Avatars.Dragon)
		{
			Name = name;
			AvailableAvatars = new List<Avatar>();
			UnlockAvatar(avatar);
			//UnlockAvatar(Content.Avatars.PiggyBank);
			//UnlockAvatar(Content.Avatars.Penguin);
		}

		public string Name { get; private set; }
		public List<Avatar> AvailableAvatars { get; private set; }

		public void UnlockAvatar(Content.Avatars avatar)
		{
			switch (avatar)
			{
			case Content.Avatars.Dragon:
				unlockedAvatar = new Dragon();
				if (!AvailableAvatars.Contains(unlockedAvatar))
					AvailableAvatars.Add(unlockedAvatar);
				break;

			case Content.Avatars.Penguin:
				unlockedAvatar = new Penguin();
				if (!AvailableAvatars.Contains(unlockedAvatar))
					AvailableAvatars.Add(unlockedAvatar);
				break;

			case Content.Avatars.PiggyBank:
				unlockedAvatar = new PiggyBank();
				if (!AvailableAvatars.Contains(unlockedAvatar))
					AvailableAvatars.Add(unlockedAvatar);
				break;
			}

			if (AvailableAvatars.Count == 1)
				ChangeAvatar(avatar);
		}

		private Avatar unlockedAvatar;

		public void ChangeAvatar(Content.Avatars avatarType)
		{
			foreach (Avatar avatar in AvailableAvatars)
				if (!avatar.IsLocked && avatar.GetType().ToString().Contains(avatarType.ToString()))
				{
					Avatar = avatar;
					return;
				}
			throw new AvatarTypeNotInitialized();
		}

		public Avatar Avatar { get; private set; }

		public class AvatarTypeNotInitialized : Exception {}

		public int Gold { get; set; }
		public int Gems { get; set; }
		public int Time { get; set; }

		public int MaxLives
		{
			get { return maxLives; }
			set
			{
				maxLives = value;
				LivesLeft = maxLives;
			}
		}
		private int maxLives;
		public int LivesLeft { get; set; }

		public int Xp
		{
			get
			{
				var allAvatars = EntitiesRunner.Current.GetEntitiesOfType<Avatar>();
				return allAvatars.Sum(avatar => avatar.Xp);
			}
		}

		public int ProgressLevel
		{
			get
			{
				var allAvatars = EntitiesRunner.Current.GetEntitiesOfType<Avatar>();
				return allAvatars.Sum(avatar => avatar.ProgressLevel);
			}
		}

		public static Player Current
		{
			get
			{
				var players = EntitiesRunner.Current.GetEntitiesOfType<Player>();
				return players.Count > 0 ? players[0] : null;
			}
		}
	}
}