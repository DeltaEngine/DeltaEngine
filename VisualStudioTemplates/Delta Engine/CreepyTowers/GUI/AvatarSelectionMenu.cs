using System;
using System.Collections.Generic;
using CreepyTowers.Avatars;
using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class AvatarSelectionMenu : Menu
	{
		public AvatarSelectionMenu()
		{
			sceneMaterials = new AvatarSelectionMaterials();
			avatarSlots = new List<AvatarSlot>();
			avatarList = (Content.Avatars[])Enum.GetValues(typeof(Content.Avatars));
			count = 0;
			totalAvatarCount = Enum.GetValues(typeof(Content.Avatars)).Length;
			CreateScene();
		}

		private readonly AvatarSelectionMaterials sceneMaterials;
		private readonly List<AvatarSlot> avatarSlots;
		private readonly Content.Avatars[] avatarList;
		private int count;
		private readonly int totalAvatarCount;

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneAvatarSelection.ToString());
			InitializeAvatarImageAndInfo();
			CreateAvatarSlots();
			UpdateAvatarSlots();
			Hide();
			AttachButtonEvents();
		}

		private void InitializeAvatarImageAndInfo()
		{
			selectedAvatarImage =
				(Picture)GetSceneControl(Content.AvatarSelectionMenu.SelectedAvatarImage.ToString());
			selectedAvatarInfo =
				(Picture)GetSceneControl(Content.AvatarSelectionMenu.SelectedAvatarInfo.ToString());
		}

		private Picture selectedAvatarInfo;
		private Picture selectedAvatarImage;

		private void CreateAvatarSlots()
		{
			avatarSlots.Add(new AvatarSlot
			{
				IconImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot1.ToString()),
				LockImage =
					(Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot1Lock.ToString()),
				GemImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot1Gem.ToString())
			});

			avatarSlots.Add(new AvatarSlot
			{
				IconImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot2.ToString()),
				LockImage =
					(Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot2Lock.ToString()),
				GemImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot2Gem.ToString())
			});

			avatarSlots.Add(new AvatarSlot
			{
				IconImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot3.ToString()),
				LockImage =
					(Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot3Lock.ToString()),
				GemImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot3Gem.ToString())
			});

			avatarSlots.Add(new AvatarSlot
			{
				IconImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot4.ToString()),
				LockImage =
					(Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot4Lock.ToString()),
				GemImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot4Gem.ToString())
			});

			avatarSlots.Add(new AvatarSlot
			{
				IconImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot5.ToString()),
				LockImage =
					(Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot5Lock.ToString()),
				GemImage = (Picture)GetSceneControl(Content.AvatarSelectionMenu.AvatarSlot5Gem.ToString())
			});
		}

		private void UpdateAvatarSlots()
		{
			var slotCount = count;
			foreach (AvatarSlot avatarSlot in avatarSlots)
			{
				CheckIfAvatarIsUnlocked(avatarSlot, slotCount);
				slotCount++;
			}
		}

		private void CheckIfAvatarIsUnlocked(AvatarSlot avatarSlot, int index)
		{
			if (index >= totalAvatarCount || index < 0)
			{
				avatarSlot.IsVisible = false;
				return;
			}

			if (!avatarSlot.IsVisible)
				avatarSlot.IsVisible = true;
			avatarSlot.AvatarName = avatarList[index];

			foreach (Avatar avatar in Player.Current.AvailableAvatars)
				if (IsAvatarAvailable(avatarSlot, avatar))
				{
					avatarSlot.LockImage.IsVisible = false;
					avatarSlot.GemImage.IsVisible = false;
					avatarSlot.IconImage.Material = sceneMaterials.AvatarIconsMaterials[index];
					return;
				}
				else
				{
					avatarSlot.LockImage.IsVisible = true;
					avatarSlot.GemImage.IsVisible = true;
					avatarSlot.IconImage.Material = sceneMaterials.AvatarIconsGreyMaterials[index];
				}
		}

		private static bool IsAvatarAvailable(AvatarSlot avatarSlot, Entity avatar)
		{
			return avatar.GetTags().Contains(avatarSlot.AvatarName.ToString());
		}

		private void AttachButtonEvents()
		{
			AddMoveLeftButtonEvent();
			AddMoveRightButtonEvent();
			AddBackButtonEvent();
			AddButtonContinueEvent();
		}

		private void AddMoveLeftButtonEvent()
		{
			var moveLeftButton =
				(InteractiveButton)GetSceneControl(Content.AvatarSelectionMenu.ButtonLeft.ToString());
			moveLeftButton.Clicked += MoveOneItemLeft;
		}

		private void MoveOneItemLeft()
		{
			count--;
			if (count < -avatarSlots.Count / 2)
			{
				count = -avatarSlots.Count / 2;
				return;
			}

			UpdateAvatarSlots();
			UpdateAvatarImageAndInfo();
		}

		private void UpdateAvatarImageAndInfo()
		{
			selectedAvatarImage.Material = sceneMaterials.AvatarImageMaterials[count + 2];
			selectedAvatarInfo.Material = sceneMaterials.AvatarInfoMaterials[count + 2];
		}

		private void AddMoveRightButtonEvent()
		{
			var moveLeftButton =
				(InteractiveButton)GetSceneControl(Content.AvatarSelectionMenu.ButtonRight.ToString());
			moveLeftButton.Clicked += MoveOneItemRight;
		}

		private void MoveOneItemRight()
		{
			count++;
			if (count >= (totalAvatarCount - avatarSlots.Count / 2))
			{
				count = (totalAvatarCount - avatarSlots.Count / 2) - 1;
				return;
			}
			UpdateAvatarSlots();
			UpdateAvatarImageAndInfo();
		}

		private void AddBackButtonEvent()
		{
			var backButton =
				(InteractiveButton)GetSceneControl(Content.AvatarSelectionMenu.ButtonBack.ToString());
			backButton.Clicked +=
				() => ToggleVisibility(GameMenus.SceneAvatarSelection, GameMenus.SceneMainMenu);
		}

		private static void ToggleVisibility(GameMenus menuToHide, GameMenus menuToShow)
		{
			MenuController.Current.HideMenu(menuToHide);
			MenuController.Current.ShowMenu(menuToShow);
		}

		private void AddButtonContinueEvent()
		{
			var continueButton =
				(InteractiveButton)GetSceneControl(Content.AvatarSelectionMenu.ButtonContinue.ToString());
			continueButton.Clicked +=
				() => ToggleVisibility(GameMenus.SceneAvatarSelection, GameMenus.SceneNightmare1);
		}

		public override void Show()
		{
			if (Scene == null || Scene.Controls.Count == 0)
				return;
			Scene.Show();
			UpdateAvatarSlots();
		}

		public override void Reset()
		{
			count = 0;
			UpdateAvatarSlots();
			UpdateAvatarImageAndInfo();
		}
	}
}