using System;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// All input devices (keyboard, mouse, touch, gamepad) will be updated each frame as Runners.
	/// Only available devices will be included into Commands and event trigger checks.
	/// </summary>
	public abstract class InputDevice : UpdateBehavior, IDisposable
	{
		protected InputDevice()
			: base(Priority.First, false) {}

		public abstract bool IsAvailable { get; protected set; }
		public abstract void Dispose();
	}
}