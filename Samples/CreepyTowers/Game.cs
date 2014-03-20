using System;
using CreepyTowers.Content;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.GUI;
using CreepyTowers.Towers;
using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Multimedia;

namespace CreepyTowers
{
	public sealed class Game : Entity
	{
		public Game(Window window, SoundDevice soundDevice)
		{
			new Player("Player");
			new TowerTargetFinder();
			window.BackgroundColor = Color.Black;
			InitializeWindowAndSoundDevice(window, soundDevice);
			new Command(Command.Exit, ExitGame);
			Add(new InGameCommands());
			new MenuController();
			ShowMainMenu();
		}

		private static void InitializeWindowAndSoundDevice(Window window, SoundDevice soundDevice)
		{
			AppWindow = window;
			SoundDevice = soundDevice;
		}

		public static Window AppWindow { get; private set; }
		public static SoundDevice SoundDevice { get; set; }

		private static void ShowMainMenu()
		{
			MenuController.Current.ShowMenu(GameMenus.SceneMainMenu);
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
			AppWindow.CloseAfterFrame();
		}
	}
}