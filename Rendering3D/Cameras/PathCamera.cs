using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// Follows a path as if on rails. Can be started and stopped. Updates at 60fps
	/// </summary>
	public sealed class PathCamera : Camera, RapidUpdateable
	{
		public PathCamera(Device device, Window window, CameraPath path)
			: base(device, window)
		{
			if (path.ViewMatrices == null || path.ViewMatrices.Length < 2)
				throw new NoTrackSpecified();
			viewMatrices = path.ViewMatrices;
		}

		public class NoTrackSpecified : Exception { }

		public PathCamera(Device device, Window window)
			: base(device, window)
		{
			viewMatrices = new Matrix[]{Matrix.Identity, Matrix.Identity};
		}

		private readonly Matrix[] viewMatrices;

		public void RapidUpdate()
		{
			if (IsMoving && currentFrame < viewMatrices.Length - 1)
				currentFrame += 1;
		}

		public bool IsMoving { get; set; }

		private int currentFrame;

		public int CurrentFrame
		{
			get { return currentFrame; }
			set
			{
				currentFrame = value.Clamp(0, viewMatrices.Length - 1);
			}
		}

		public override void ResetDefaults()
		{
			currentFrame = 0;
		}

		protected internal override Matrix GetCurrentViewMatrix()
		{
			return viewMatrices[currentFrame];
		}
	}
}