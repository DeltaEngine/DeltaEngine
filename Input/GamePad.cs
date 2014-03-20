using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current game pad input values.
	/// </summary>
	public abstract class GamePad : InputDevice
	{
		protected GamePad(GamePadNumber number)
		{
			Number = number;
		}

		protected GamePadNumber Number { get; private set; }

		public abstract Vector2D GetLeftThumbStick();
		public abstract Vector2D GetRightThumbStick();
		public abstract float GetLeftTrigger();
		public abstract float GetRightTrigger();
		public abstract State GetButtonState(GamePadButton button);
		public abstract void Vibrate(float strength);

		public override void Update(IEnumerable<Entity> entities)
		{
			if (!IsAvailable)
				return; //ncrunch: no coverage
			UpdateGamePadStates();
			foreach (var button in entities.OfType<GamePadButtonTrigger>())
				if (GetButtonState(button.Button) == button.State)
					button.Invoke();
			foreach (var stick in entities.OfType<GamePadAnalogTrigger>())
				if (IsGamePadStickTriggered(stick))
					stick.Invoke();
		}

		protected abstract void UpdateGamePadStates();

		private bool IsGamePadStickTriggered(GamePadAnalogTrigger trigger)
		{
			switch (trigger.Analog)
			{
			case GamePadAnalog.LeftThumbStick:
				trigger.Position = GetLeftThumbStick();
				break;
			case GamePadAnalog.RightThumbStick:
				trigger.Position = GetRightThumbStick();
				break;
			case GamePadAnalog.LeftTrigger:
				trigger.Position = new Vector2D(GetLeftTrigger(), GetLeftTrigger());
				break;
			case GamePadAnalog.RightTrigger:
				trigger.Position = new Vector2D(GetRightTrigger(), GetRightTrigger());
				break;
			}
			return trigger.Position != Vector2D.Unused;
		}
	}
}