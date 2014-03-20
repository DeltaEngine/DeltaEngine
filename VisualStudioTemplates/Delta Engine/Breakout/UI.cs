using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace $safeprojectname$
{
	/// <summary>
	/// Primes the window to respond to keyboard commands and launches the game
	/// </summary>
	public class UI : Entity, Updateable
	{
		public UI(Window window, Game game)
		{
			this.window = window;
			this.game = game;
			new Command(window.CloseAfterFrame).Add(new KeyTrigger(Key.Escape, State.Pressed));
			new Command(() => window.SetFullscreen(new Size(1920, 1080))).Add(new KeyTrigger(Key.F));
		}

		private readonly Window window;
		private readonly Game game;

		public void Update()
		{
			if (Time.CheckEvery(0.2f))
				window.Title = "Breakout " + game.Score;
		}

		public override bool IsPauseable
		{
			get { return true; }
		}
	}
}