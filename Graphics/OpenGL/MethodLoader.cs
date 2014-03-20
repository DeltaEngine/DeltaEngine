using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DeltaEngine.Core;

namespace DeltaEngine.Graphics.OpenGL
{
	internal class MethodLoader
	{
		private readonly BaseGraphicsContext context;
		private readonly Dictionary<string, Delegate> functions;

		public MethodLoader(BaseGraphicsContext context)
		{
			this.context = context;
			functions = new Dictionary<string, Delegate>();
		}

		public T Get<T>()
			where T : class
		{
			string name = typeof(T).Name;
			if (functions.ContainsKey(name))
				return functions[name] != null ? ConvertDelegate<T>(functions[name]) : null;
			return LoadFromNativeLibrary<T>(name);
		}

		private static T ConvertDelegate<T>(Delegate callback)
			where T : class
		{
			return (T)Convert.ChangeType(callback, typeof(T));
		}

		private T LoadFromNativeLibrary<T>(string name)
			where T : class
		{
			functions.Add(name, null);
			IntPtr proc = GetFunctionPointer(name);
			if (proc == IntPtr.Zero)
				return null;
			Delegate deleg = Marshal.GetDelegateForFunctionPointer(proc, typeof(T));
			functions[name] = deleg;
			return ConvertDelegate<T>(deleg);
		}

		private IntPtr GetFunctionPointer(string name)
		{
			IntPtr pointer = context.GetGLProcAddress(name);
			if (pointer == IntPtr.Zero)
			{
				pointer = context.GetProcAddress(context.GLHandle, name);
				if (pointer == IntPtr.Zero)
				{
					Logger.Info("Failed to get func " + name + " library=" + context.GLHandle);
					return IntPtr.Zero;
				}
			}
			return pointer;
		}
	}
}