using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	/// <summary>
	/// Knits the main control classes together by feeding events raised by one to another
	/// </summary>
	public class Game
	{
		public Game(Window window)
		{
			menu = new MainMenu();
			//ncrunch: no coverage start
			menu.InitGame += () =>
			{
				menu.Hide();
				StartGame();
			}; //ncrunch: no coverage end
			menu.QuitGame += window.CloseAfterFrame;
			window.Title = "Blocks";
		}

		private MainMenu menu;
		public UserInterface UserInterface { get; private set; }
		public Controller Controller { get; private set; }
		public bool IsInGame { get; set; }

		public void StartGame()
		{
			UserInterface = new UserInterface(menu.BlocksContent);
			Controller = new Controller(DisplayMode, menu.BlocksContent);
			IsInGame = true;
			Initialize();
		}

		private void Initialize()
		{
			SetDisplayMode();
			SetControllerEvents();
			SetInputEvents();
		}

		private void SetDisplayMode()
		{
			var aspectRatio = ScreenSpace.Current.Viewport.Aspect;
			DisplayMode = aspectRatio >= 1.0f ? Orientation.Landscape : Orientation.Portrait;
		}

		protected Orientation DisplayMode { get; set; }

		private void SetControllerEvents()
		{
			Controller.AddToScore += UserInterface.AddToScore;
			Controller.Lose += UserInterface.Lose;
		}

		private void SetInputEvents()
		{
			CreateCommands();
			SetKeyboardEvents();
			SetMouseEvents();
			SetTouchEvents();
		}

		private void CreateCommands()
		{
			commands = new Command[9];
			commands[0] = new Command(() => StartMovingBlockLeft());
			commands[1] = new Command(() => { Controller.isBlockMovingLeft = false; });
			commands[2] = new Command(() => StartMovingBlockRight());
			commands[3] = new Command(() => { Controller.isBlockMovingRight = false; });
			commands[4] = new Command(() => Controller.RotateBlockAntiClockwiseIfPossible());
			commands[5] = new Command(() => { Controller.IsFallingFast = true; });
			commands[6] = new Command(() => { Controller.IsFallingFast = false; });
			commands[7] = new Command(position => { Pressing(position); });
			commands[8] = new Command(() => { Controller.IsFallingFast = false; });
		}

		private Command[] commands;

		private void SetKeyboardEvents()
		{
			commands[0].Add(new KeyTrigger(Key.CursorLeft));
			commands[1].Add(new KeyTrigger(Key.CursorLeft, State.Releasing));
			commands[2].Add(new KeyTrigger(Key.CursorRight));
			commands[3].Add(new KeyTrigger(Key.CursorRight, State.Releasing));
			commands[4].Add(new KeyTrigger(Key.CursorUp));
			commands[4].Add(new KeyTrigger(Key.Space));
			commands[5].Add(new KeyTrigger(Key.CursorDown));
			commands[6].Add(new KeyTrigger(Key.CursorDown, State.Releasing));
		}

		private void StartMovingBlockLeft()
		{
			Controller.MoveBlockLeftIfPossible();
			Controller.isBlockMovingLeft = true;
		}

		private void StartMovingBlockRight()
		{
			Controller.MoveBlockRightIfPossible();
			Controller.isBlockMovingRight = true;
		}

		private void SetMouseEvents()
		{
			commands[7].Add(new MouseButtonTrigger());
			commands[8].Add(new MouseButtonTrigger(MouseButton.Left, State.Releasing));
		}

		private void Pressing(Vector2D position)
		{
			if (position.X < 0.4f)
				Controller.MoveBlockLeftIfPossible(); //ncrunch: no coverage
			else if (position.X > 0.6f)
				Controller.MoveBlockRightIfPossible(); //ncrunch: no coverage
			else if (position.Y < 0.5f)
				Controller.RotateBlockAntiClockwiseIfPossible(); //ncrunch: no coverage
			else
				Controller.IsFallingFast = true;
		}

		private void SetTouchEvents()
		{
			commands[7].Add(new TouchPositionTrigger());
			commands[8].Add(new TouchPositionTrigger(State.Releasing));
		}
	}
}