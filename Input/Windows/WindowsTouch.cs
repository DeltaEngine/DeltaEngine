using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input.Windows
{
	/// <summary>
	/// Native Windows implementation of the Touch interface.
	/// </summary>
	public sealed class WindowsTouch : Touch
	{
		public WindowsTouch(Window window)
		{
			touches = new TouchCollection(new CursorPositionTranslater(window));
			if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
				return;
			//ncrunch: no coverage start
			IsAvailable = CheckIfWindows7OrHigher();
			if (IsAvailable)
				hook = new TouchHook(window);
			else
				Logger.Warning("Touch is not supported by the OS. Touch triggers won't work!"); 
		} 

		private readonly TouchHook hook;
		private readonly TouchCollection touches;

		public override bool IsAvailable { get; protected set; }

		public override void Dispose()
		{
			if (hook != null)
				hook.Dispose();
		}

		private static bool CheckIfWindows7OrHigher()
		{
			Version version = Environment.OSVersion.Version;
			return version.Major >= 6 && version.Minor >= 1;
		}

		public override Vector2D GetPosition(int touchIndex)
		{
			return touches.locations[touchIndex];
		}

		public override State GetState(int touchIndex)
		{
			return touches.states[touchIndex];
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			if (!IsAvailable)
				return; //ncrunch: no coverage (can only be reached from Windows Vista or earlier)
			var newTouches = new List<NativeTouchInput>(hook.nativeTouches.ToArray());
			touches.UpdateAllTouches(newTouches);
			hook.nativeTouches.Clear();
			base.Update(entities);
		}
	}
}