using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using SysPoint = System.Drawing.Point;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native mouse implementation using a windows hook and invokes.
	/// </summary>
	public class WindowsMouse : Mouse
	{
		//ncrunch: no coverage start
		public WindowsMouse(Window window)
		{
			if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				hook = new MouseHook();
			positionTranslater = new CursorPositionTranslater(window);
			positionTranslater.window.ViewportSizeChanged += size => wasViewportResizedThisFrame = true;
			mouseCounter = new MouseDeviceCounter();
		}

		internal readonly MouseHook hook;
		private readonly CursorPositionTranslater positionTranslater;
		private bool wasViewportResizedThisFrame;
		private readonly MouseDeviceCounter mouseCounter;

		public override bool IsAvailable
		{
			get { return mouseCounter.GetNumberOfAvailableMice() > 0; }
			protected set {} //ncrunch: no coverage (senseless regarding the "get" part)
		}

		public override void SetNativePosition(Vector2D position)
		{
			positionTranslater.SetCursorPosition(position);
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			UpdateMousePosition();
			UpdateMouseValues();
			base.Update(entities);
		}

		private void UpdateMousePosition()
		{
			Position = positionTranslater.GetCursorPosition();
		}

		private void UpdateMouseValues()
		{
			if (hook == null)
				return;
			ScrollWheelValue = hook.ScrollWheelValue;
			leftButton = hook.ProcessButtonQueue(leftButton, MouseButton.Left);
			if (AreLeftButtonEventsBeingIgnoredDueToWindowResizing())
				wasViewportResizedThisFrame = false;
			else
				LeftButton = leftButton;
			MiddleButton = hook.ProcessButtonQueue(MiddleButton, MouseButton.Middle);
			RightButton = hook.ProcessButtonQueue(RightButton, MouseButton.Right);
			X1Button = hook.ProcessButtonQueue(X1Button, MouseButton.X1);
			X2Button = hook.ProcessButtonQueue(X2Button, MouseButton.X2);
		}

		private State leftButton;

		private bool AreLeftButtonEventsBeingIgnoredDueToWindowResizing()
		{
			return wasViewportResizedThisFrame ||
						(leftButton == State.Releasing && LeftButton == State.Released);
		}

		public override void Dispose()
		{
			if (hook != null)
				hook.Dispose();
		}
	}
}