using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;

namespace $safeprojectname$
{
	/// <summary>
	/// Controls, can be set according to a gameState
	/// </summary>
	public class Controls
	{
		public Controls(Game game)
		{
			this.game = game;
			controlCommands = new Command[6];
			commandsInUse = 0;
		}

		private readonly Game game;
		private readonly Command[] controlCommands;
		private int commandsInUse;

		public void SetControlsToState(GameState state)
		{
			for (int i = 0; i < commandsInUse; i++)
				controlCommands[i].Dispose();
			commandsInUse = 0;
			switch (state)
			{
			case GameState.Playing:
				AddPlayerAccelerationControl();
				AddPlayerSteerLeft();
				AddPlayerSteerRight();
				AddPlayerShootingControls();
				AddPlayerMoveDirectly();
				break;
			case GameState.GameOver:
				AddRestartControl();
				AddMainMenuControl();
				break;
			default:
				return; //ncrunch: no coverage
			}
		}

		private void AddPlayerMoveDirectly()
		{
			controlCommands[5] = new Command (SteerPlayerAnalogue).Add(new GamePadAnalogTrigger(GamePadAnalog.LeftThumbStick));
			commandsInUse++;
		}

		//ncrunch: no coverage start, interaction
		private void SteerPlayerAnalogue(Vector2D direction)
		{
			if(direction.Y > 0)
			game.InteractionLogic.Player.ShipAccelerate(direction.Y * 0.7f);
			if (direction.X > 0)
				game.InteractionLogic.Player.SteerRight(direction.X * 0.7f);
			if(direction.X < 0)
				game.InteractionLogic.Player.SteerLeft(-direction.X * 0.7f);
		} //ncrunch: no coverage end

		private void AddPlayerAccelerationControl()
		{
			controlCommands[0] = new Command(PlayerForward);
			controlCommands[0].Add(new KeyTrigger(Key.W, State.Pressed));
			controlCommands[0].Add(new KeyTrigger(Key.W));
			controlCommands[0].Add(new KeyTrigger(Key.CursorUp, State.Pressed));
			controlCommands[0].Add(new KeyTrigger(Key.CursorUp));
		}

		private void AddPlayerSteerLeft()
		{
			controlCommands[1] = new Command(PlayerSteerLeft);
			controlCommands[1].Add(new KeyTrigger(Key.A, State.Pressed));
			controlCommands[1].Add(new KeyTrigger(Key.A));
			controlCommands[1].Add(new KeyTrigger(Key.CursorLeft, State.Pressed));
			controlCommands[1].Add(new KeyTrigger(Key.CursorLeft));
			commandsInUse++;
		}

		private void AddPlayerSteerRight()
		{
			controlCommands[2] = new Command(PlayerSteerRight);
			controlCommands[2].Add(new KeyTrigger(Key.D, State.Pressed));
			controlCommands[2].Add(new KeyTrigger(Key.D));
			controlCommands[2].Add(new KeyTrigger(Key.CursorRight, State.Pressed));
			controlCommands[2].Add(new KeyTrigger(Key.CursorRight));
			commandsInUse++;
		}

		private void AddPlayerShootingControls()
		{
			controlCommands[3] = new Command(PlayerBeginFireing).Add(new KeyTrigger(Key.Space));
			controlCommands[3].Add(new GamePadButtonTrigger(GamePadButton.A));
			controlCommands[4] =
				new Command(PlayerHoldFire).Add(new KeyTrigger(Key.Space, State.Releasing));
			controlCommands[4].Add(new GamePadButtonTrigger(GamePadButton.A, State.Releasing));
			commandsInUse += 2;
		}

		private void AddRestartControl()
		{
			controlCommands[0] = new Command(() => game.RestartGame());
			controlCommands[0].Add(new KeyTrigger(Key.Space, State.Releasing));
			controlCommands[0].Add(new GamePadButtonTrigger(GamePadButton.A, State.Releasing));
			commandsInUse++;
		}

		private void AddMainMenuControl()
		{
			controlCommands[1] = new Command(() => game.BackToMenu());
			controlCommands[1].Add(new KeyTrigger(Key.Escape, State.Releasing));
			controlCommands[1].Add(new GamePadButtonTrigger(GamePadButton.B, State.Releasing));
			commandsInUse++;
		}

		//ncrunch: no coverage start
		private void PlayerForward()
		{
			game.InteractionLogic.Player.ShipAccelerate();
		}

		private void PlayerSteerLeft()
		{
			game.InteractionLogic.Player.SteerLeft();
		}

		private void PlayerSteerRight()
		{
			game.InteractionLogic.Player.SteerRight();
		}

		private void PlayerBeginFireing()
		{
			game.InteractionLogic.Player.IsFiring = true;
		}

		private void PlayerHoldFire()
		{
			game.InteractionLogic.Player.IsFiring = false;
		}
	}
}