using System;

namespace DeltaEngine.Graphics.OpenGL
{
	internal abstract class BaseGraphicsContext : IDisposable
	{
		protected IntPtr GraphicsContext;

		public MethodLoader Loader { get; private set; }

		public static BaseGraphicsContext Current { get; protected set; }

		public abstract bool VSync { set; }

		public abstract bool IsCurrent { get; }

		public IntPtr GLHandle { get; protected set; }

		protected BaseGraphicsContext()
		{
			Loader = new MethodLoader(this);
		}

		public abstract IntPtr GetGLProcAddress(string functionName);

		public abstract IntPtr GetProcAddress(IntPtr library, string functionName);

		public abstract void SwapBuffers();

		public abstract bool MakeCurrent();

		internal abstract void UpdateDeviceParameters(byte colorBits, byte depth, byte stencil);

		public abstract void Dispose();
	}
}