using System.IO;
using DeltaEngine.Commands;
using DeltaEngine.Core;

namespace DeltaEngine.Content.Xml
{
	/// <summary>
	/// Commands from a content file which trigger input events.
	/// </summary>
	public class InputCommands : XmlContent
	{
		protected InputCommands(string contentName)
			: base(contentName) {}

		protected override bool AllowCreationIfContentNotFound
		{
			get { return Name == "DefaultCommands"; }
		}

		/// <summary>
		/// Creates a simple DefaultCommands.xml file in memory and passes it to Command.Register.
		/// Trigger types cannot be created directly here because we have no dependency to Input here.
		/// For better performance with MockResolver the same code is duplicated there, just faster.
		/// </summary>
		protected override void CreateDefault()
		{
			var exit = new XmlData("Command");
			exit.AddChild("KeyTrigger", "Escape");
			Command.Register(Command.Exit, ParseTriggers(exit));
			var click = new XmlData("Command");
			click.AddChild("KeyTrigger", "Space");
			click.AddChild("MouseButtonTrigger", "Left");
			click.AddChild("TouchPressTrigger", "");
			click.AddChild("GamePadButtonTrigger", "A");
			Command.Register(Command.Click, ParseTriggers(click));
			var middleClick = new XmlData("Command");
			middleClick.AddChild("MouseButtonTrigger", "Middle");
			Command.Register(Command.MiddleClick, ParseTriggers(middleClick));
			var rightClick = new XmlData("Command");
			rightClick.AddChild("MouseButtonTrigger", "Right");
			Command.Register(Command.RightClick, ParseTriggers(rightClick));
			var moveLeft = new XmlData("Command");
			moveLeft.AddChild("KeyTrigger", "CursorLeft Pressed");
			Command.Register(Command.MoveLeft, ParseTriggers(moveLeft));
			var moveRight = new XmlData("Command");
			moveRight.AddChild("KeyTrigger", "CursorRight Pressed");
			Command.Register(Command.MoveRight, ParseTriggers(moveRight));
			var moveUp = new XmlData("Command");
			moveUp.AddChild("KeyTrigger", "CursorUp Pressed");
			Command.Register(Command.MoveUp, ParseTriggers(moveUp));
			var moveDown = new XmlData("Command");
			moveDown.AddChild("KeyTrigger", "CursorDown Pressed");
			Command.Register(Command.MoveDown, ParseTriggers(moveDown));
			var moveDirectly = new XmlData("Command");
			moveDirectly.AddChild("KeyTrigger", "CursorLeft Pressed");
			moveDirectly.AddChild("KeyTrigger", "CursorRight Pressed");
			moveDirectly.AddChild("KeyTrigger", "CursorUp Pressed");
			moveDirectly.AddChild("KeyTrigger", "CursorDown Pressed");
			Command.Register(Command.MoveDirectly, ParseTriggers(moveDirectly));
			var rotateDirectly = new XmlData("Command");
			rotateDirectly.AddChild("MouseMovementTrigger", "");
			Command.Register(Command.RotateDirectly, ParseTriggers(rotateDirectly));
			var back = new XmlData("Command");
			back.AddChild("KeyTrigger", "Backspace Pressed");
			Command.Register(Command.Back, ParseTriggers(back));
			var drag = new XmlData("Command");
			drag.AddChild("MouseDragTrigger", "Left");
			Command.Register(Command.Drag, ParseTriggers(drag));
			var flick = new XmlData("Command");
			flick.AddChild("TouchFlickTrigger", "");
			Command.Register(Command.Flick, ParseTriggers(flick));
			var pinch = new XmlData("Command");
			pinch.AddChild("TouchPinchTrigger", "");
			Command.Register(Command.Pinch, ParseTriggers(pinch));
			var hold = new XmlData("Command");
			hold.AddChild("TouchHoldTrigger", "");
			Command.Register(Command.Hold, ParseTriggers(hold));
			var doubleClick = new XmlData("Command");
			doubleClick.AddChild("MouseDoubleClickTrigger", "Left");
			Command.Register(Command.DoubleClick, ParseTriggers(doubleClick));
			var rotate = new XmlData("Command");
			rotate.AddChild("TouchRotateTrigger", "");
			Command.Register(Command.Rotate, ParseTriggers(rotate));
			var zoom = new XmlData("Command");
			zoom.AddChild("MouseZoomTrigger", "");
			Command.Register(Command.Zoom, ParseTriggers(zoom));
		}

		protected override void LoadData(Stream fileData)
		{
			base.LoadData(fileData);
			foreach (var child in Data.Children)
				Command.Register(child.GetAttributeValue("Name"), ParseTriggers(child));//ncrunch: no coverage
		}

		protected static Trigger[] ParseTriggers(XmlData command)
		{
			var triggers = new Trigger[command.Children.Count];
			for (int index = 0; index < command.Children.Count; index++)
			{
				var trigger = command.Children[index];
				var type = BinaryDataExtensions.GetTypeFromShortNameOrFullNameIfNotFound(trigger.Name);
				if (type == null)
					throw new Trigger.UnableToCreateTriggerTypeIsUnknown(trigger.Name); //ncrunch: no coverage
				triggers[index] =
					Trigger.GenerateTriggerFromType(type, trigger.Name, trigger.Value) as Trigger;
			}
			return triggers;
		}
	}
}