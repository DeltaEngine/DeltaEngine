using System.Collections.Generic;
using CreepyTowers.Content;
using DeltaEngine.Commands;
using DeltaEngine.Input;

namespace $safeprojectname$
{
	public class InGameCommands
	{
		public InGameCommands()
		{
			CommandsList = new List<Command>();
			SetInputCommands();
		}

		public List<Command> CommandsList { get; private set; }

		private static void SetInputCommands()
		{
			Command.Register(GameCommands.MouseLeftButtonClick.ToString(), new Trigger[] { new MouseButtonTrigger() });
			Command.Register(GameCommands.MouseRightButtonClick.ToString(),
				new Trigger[] { new MouseButtonTrigger(MouseButton.Right) });
			Command.Register(GameCommands.ViewPanning.ToString(), new Trigger[] { new MouseDragTrigger(MouseButton.Middle) });
			Command.Register(GameCommands.ViewZooming.ToString(), new Trigger[] { new MouseZoomTrigger() });
			Command.Register(GameCommands.TurnViewRight.ToString(), new Trigger[] { new KeyTrigger(Key.CursorRight, State.Pressed) });
			Command.Register(GameCommands.TurnViewLeft.ToString(), new Trigger[] { new KeyTrigger(Key.CursorLeft, State.Pressed) });
		}
	}
}