using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Input Commands are loaded via the InputCommands.xml file, see InputCommands.cs for details.
	/// You can also create your own commands, which will be executed whenever any trigger is invoked.
	/// </summary>
	public class Command : Entity, Updateable
	{
		public static void Register(string commandName, params Trigger[] commandTriggers)
		{
			if (string.IsNullOrEmpty(commandName))
				throw new ArgumentNullException("commandName");
			if (commandTriggers == null || commandTriggers.Length == 0)
				throw new UnableToRegisterCommandWithoutTriggers(commandName);
			if (RegisteredCommands.ContainsKey(commandName))
				RegisteredCommands[commandName] = commandTriggers;
			else
				RegisteredCommands.Add(commandName, commandTriggers);
		}

		private static readonly Dictionary<string, Trigger[]> RegisteredCommands =
			new Dictionary<string, Trigger[]>();

		public class UnableToRegisterCommandWithoutTriggers : Exception
		{
			public UnableToRegisterCommandWithoutTriggers(string commandName)
				: base(commandName) {}
		}

		public Command(string commandName, Action action)
			: this(action)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
		}

		public Command(Action action)
		{
			this.action = action;
			UpdatePriority = Priority.First;
		}

		private readonly Action action;
		
		private static IEnumerable<Trigger> LoadTriggersForCommand(string commandName)
		{
			Trigger[] loadedTriggers;
			InvokeNonEditorContentLoaderToMakeSureCommandsInitializationHappened();
			if (RegisteredCommands.TryGetValue(commandName, out loadedTriggers))
				return loadedTriggers;
			throw new CommandNameWasNotRegistered();
		}

		private static void InvokeNonEditorContentLoaderToMakeSureCommandsInitializationHappened()
		{
			ContentLoader.Exists("DefaultCommands");
		}

		public class CommandNameWasNotRegistered : Exception {}

		public Command(string commandName, Action<Vector2D> positionAction)
			: this(positionAction)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
		}

		public Command(Action<Vector2D> positionAction)
		{
			this.positionAction = positionAction;
			UpdatePriority = Priority.First;
		}

		private readonly Action<Vector2D> positionAction;

		public Command(string commandName, Action<Vector2D, Vector2D, bool> dragAction)
			: this(dragAction)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
		}

		public Command(Action<Vector2D, Vector2D, bool> dragAction)
		{
			this.dragAction = dragAction;
			UpdatePriority = Priority.First;
		}

		private readonly Action<Vector2D, Vector2D, bool> dragAction;

		public Command(string commandName, Action<float> zoomAction)
			: this(zoomAction)
		{
			triggers.AddRange(LoadTriggersForCommand(commandName));
		}

		public Command(Action<float> zoomAction)
		{
			this.zoomAction = zoomAction;
			UpdatePriority = Priority.First;
		}

		private readonly Action<float> zoomAction;

		public Command Add(Trigger trigger)
		{
			triggers.Add(trigger);
			return this;
		}

		private readonly List<Trigger> triggers = new List<Trigger>();

		public const string Click = "Click";
		public const string DoubleClick = "DoubleClick";
		public const string Hold = "Hold";
		public const string Drag = "Drag";
		public const string Flick = "Flick";
		public const string Pinch = "Pinch";
		public const string Rotate = "Rotate";
		public const string DualDrag = "DualDrag";
		public const string MiddleClick = "MiddleClick";
		public const string RightClick = "RightClick";
		public const string MoveLeft = "MoveLeft";
		public const string MoveRight = "MoveRight";
		public const string MoveUp = "MoveUp";
		public const string MoveDown = "MoveDown";
		public const string Zoom = "Zoom";
		/// <summary>
		/// Allows to move left, right, up or down using ASDW or the cursor keys or an onscreen stick.
		/// </summary>
		public const string MoveDirectly = "MoveDirectly";
		/// <summary>
		/// Rotate in 3D space, normally bound to the mouse, game pad thumb stick or onscreen stick.
		/// </summary>
		public const string RotateDirectly = "RotateDirectly";
		/// <summary>
		/// Go back one screen, normally unbound except if scene allows it. Right click or back button.
		/// </summary>
		public const string Back = "Back";
		/// <summary>
		/// Exits whole application (or scene) if supported. Mostly handled by the platform (Alt+F4).
		/// </summary>
		public const string Exit = "Exit";

		public void Update()
		{
			Trigger invokedTrigger = triggers.FirstOrDefault(t => t.WasInvokedThisTick);
			if (invokedTrigger == null)
				return;
			var positionTrigger = invokedTrigger as PositionTrigger;
			var dragTrigger = invokedTrigger as DragTrigger;
			var zoomTrigger = invokedTrigger as ZoomTrigger;
			if (positionAction != null && positionTrigger != null)
				positionAction(positionTrigger.Position);
			else if (dragAction != null && dragTrigger != null)
				dragAction(dragTrigger.StartPosition, dragTrigger.Position, dragTrigger.DoneDragging);
			else if (zoomAction != null && zoomTrigger != null)
				zoomAction(zoomTrigger.ZoomAmount);
			else if (action != null)
				action();
			else if (positionAction != null)
				positionAction(Vector2D.Half);
			else if (dragAction != null)
				dragAction(Vector2D.Half, Vector2D.Half, false);
			else if (zoomAction != null)
				zoomAction(0);
		}

		public override bool IsPauseable { get { return false; } }

		public List<Trigger> GetTriggers()
		{
			return triggers;
		}
	}
}