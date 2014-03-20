using System;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace Blocks
{
	internal class MainMenu
	{
		//ncrunch: no coverage start
		public MainMenu()
		{
			BlocksContent = new FruitBlocksContent();
			LoadAndSetup();
		}

		private Scene menuScene;

		private void LoadAndSetup()
		{
			menuScene = BlocksContent.Load<Scene>("MainMenu");
			foreach (var control in menuScene.Controls)
			{
				if (!control.GetType().IsSubclassOf(typeof(Button)))
					continue;
				var button = control as Button;
				if (button.Name=="StartGame")
				{
					button.Clicked += InvokeGameStart;
				}
				if (button.Name=="HowToPlay")
				{
					button.Clicked += ShowHowToPlaySubMenu;
				}
				if (button.Name=="QuitGame")
				{
					button.Clicked += TryInvokeQuit;
				}
				if (button.Name=="ContentSwitcher")
				{
					button.Clicked += SwitchContent;
				}
			}
		}

		public BlocksContent BlocksContent { get; private set; }

		private void InvokeGameStart()
		{
			if (InitGame != null)
				InitGame();
		}

		public event Action InitGame;

		private void ShowHowToPlaySubMenu()
		{
			if (howToPlay == null)
				howToPlay = BlocksContent.Load<Scene>("HowToPlayMenu");
			foreach (var control in howToPlay.Controls)
			{
				if (!control.GetType().IsSubclassOf(typeof(Button)))
					continue;
				var button = control as Button;
				if (button.Name == "Back")
				{
					button.Clicked += () =>
					{
						howToPlay.Hide();
						Show();
					};
					break;
				}
			}
			howToPlay.Show();
			Hide();
		}

		private Scene howToPlay;

		private void TryInvokeQuit()
		{
			if (QuitGame != null)
				QuitGame();
		}

		public event Action QuitGame;

		private void SwitchContent()
		{
			contentSwitched = !contentSwitched;
			BlocksContent = contentSwitched
				? new JewelBlocksContent() : (BlocksContent)new FruitBlocksContent();
			Clear();
			if (howToPlay != null)
				howToPlay.Clear();
			howToPlay = null;
			menuScene = null;
			LoadAndSetup();
		}

		private bool contentSwitched;

		public void Show()
		{
			if (menuScene != null)
				menuScene.Show();
		}

		public void Hide()
		{
			if (menuScene != null)
				menuScene.Hide();
		}

		public void Clear()
		{
			if (menuScene != null)
				menuScene.Clear();
		}

		//ncrunch: no coverage end
	}
}