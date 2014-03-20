using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace DeltaEngine.Graphics.OpenGL.Wgl
{
	internal class WglGraphicsContext : BaseGraphicsContext
	{
		private readonly IntPtr windowHandle;
		private const string GLLib = "opengl32.dll";
		private IntPtr windowContext;
		private const PixelFormatDescriptorFlags PixelFormatFlags = PixelFormatDescriptorFlags.DrawToWindow | PixelFormatDescriptorFlags.SupportOpenGL | PixelFormatDescriptorFlags.DoubleBuffer;
		private static readonly Dictionary<int, int> PixelFormatCache = new Dictionary<int, int>();
		private bool isDisposed;
		private const string KernelLib = "kernel32.dll";
		private const string GdiLib = "gdi32.dll";
		private const string UserLib = "user32.dll";
		private delegate bool wglSwapIntervalEXT(int interval);
		private static wglSwapIntervalEXT WglSwapIntervalEXT;
		private delegate bool wglChoosePixelFormatARB(IntPtr hdc, int[] piAttribIList, float[] pfAttribFList, uint nMaxFormats, [In] [Out] int[] piFormats, out uint nNumFormats);
		private wglChoosePixelFormatARB WglChoosePixelFormatARB;
		private delegate bool wglGetPixelFormatAttribivARB(IntPtr hdc, int iPixelFormat, int iLayerPlane, uint nAttributes, int[] piAttributes, int[] piValues);
		private wglGetPixelFormatAttribivARB WglGetPixelFormatAttribivARB;
		private delegate IntPtr wglCreateContextAttribsARB(IntPtr hdc, IntPtr hShareContext, int[] attribList);
		private wglCreateContextAttribsARB WglCreateContextAttribsARB;

		public override bool VSync
		{
			set
			{
				if (WglSwapIntervalEXT != null)
					WglSwapIntervalEXT(value ? 1 : 0);
			}
		}

		public override bool IsCurrent
		{
			get
			{
				return GraphicsContext == GetCurrentContext();
			}
		}

		public WglGraphicsContext(IntPtr windowHandle)
		{
			this.windowHandle = windowHandle;
			GLHandle = LoadLibrary(GLLib);
			if (GLHandle == IntPtr.Zero)
				throw new Exception("Failed to load the native OpenGL library '" + GLLib + "'. Make sure you have the latest graphics card driver installed!");
		}

		public override IntPtr GetGLProcAddress(string functionName)
		{
			return ExternGetGLProcAddress(functionName);
		}

		public override void SwapBuffers()
		{
			ExternSwapBuffers(windowContext);
		}

		public override bool MakeCurrent()
		{
			Current = this;
			return ExternMakeCurrent(windowContext, GraphicsContext);
		}

		internal override void UpdateDeviceParameters(byte colorBits, byte depth, byte stencil)
		{
			if (GraphicsContext != IntPtr.Zero)
				FreeResources();
			windowContext = GetWindowDC(windowHandle);
			PixelFormatDescriptor descriptor = new PixelFormatDescriptor { Size = 40, Version = 1, PixelType = ColorFormatPixelType.Rgba, ColorBits = colorBits, DepthBits = depth, StencilBits = stencil, LayerType = WglLayerType.PfdMainPlane, Flags = PixelFormatFlags, RedBits = 8, GreenBits = 8, BlueBits = 8 };
			if (colorBits == 32)
				descriptor.AlphaBits = 8;
			int cacheKey = colorBits + (depth << 8) + (stencil << 16);
			int flag;
			if (PixelFormatCache.ContainsKey(cacheKey))
				flag = PixelFormatCache[cacheKey];
			else
			{
				flag = ChoosePixelFormat(windowContext, ref descriptor);
				PixelFormatCache.Add(cacheKey, flag);
			}
			SetPixelFormat(windowContext, flag, ref descriptor);
			GraphicsContext = CreateContext(windowContext);
			MakeCurrent();
			LoadExtensions();
		}

		private void LoadExtensions()
		{
			WglChoosePixelFormatARB = Loader.Get<wglChoosePixelFormatARB>();
			WglGetPixelFormatAttribivARB = Loader.Get<wglGetPixelFormatAttribivARB>();
			WglSwapIntervalEXT = Loader.Get<wglSwapIntervalEXT>();
			WglCreateContextAttribsARB = Loader.Get<wglCreateContextAttribsARB>();
		}

		public override void Dispose()
		{
			if (isDisposed)
				return;
			isDisposed = true;
			FreeResources();
			FreeLibrary(GLHandle);
		}

		private void FreeResources()
		{
			ExternMakeCurrent(IntPtr.Zero, IntPtr.Zero);
			DeleteContext(GraphicsContext);
			ReleaseDC(windowHandle, windowContext);
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport(KernelLib, EntryPoint = "LoadLibrary")]
		internal static extern IntPtr LoadLibrary(string lpFileName);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(KernelLib, EntryPoint = "GetProcAddress")]
		private static extern IntPtr ExternGetSystemProcAddress(IntPtr library, string lpProcName);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(KernelLib, EntryPoint = "FreeLibrary")]
		internal static extern bool FreeLibrary(IntPtr library);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GLLib, EntryPoint = "wglGetProcAddress")]
		public static extern IntPtr ExternGetGLProcAddress(string functionName);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GLLib, EntryPoint = "wglCreateContext")]
		public static extern IntPtr CreateContext(IntPtr deviceContext);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GLLib, EntryPoint = "wglDeleteContext")]
		public static extern bool DeleteContext(IntPtr oldContext);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GLLib, EntryPoint = "wglGetCurrentContext", ExactSpelling = true)]
		public static extern IntPtr GetCurrentContext();

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GLLib, EntryPoint = "wglMakeCurrent")]
		public static extern bool ExternMakeCurrent(IntPtr deviceContext, IntPtr newContext);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GdiLib, EntryPoint = "ChoosePixelFormat")]
		public static extern int ChoosePixelFormat(IntPtr deviceContext, ref PixelFormatDescriptor pPfd);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GdiLib, EntryPoint = "SwapBuffers")]
		public static extern bool ExternSwapBuffers(IntPtr deviceContext);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(GdiLib, EntryPoint = "SetPixelFormat")]
		private static extern bool SetPixelFormat(IntPtr dc, int format, ref PixelFormatDescriptor pfd);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(UserLib, EntryPoint = "GetWindowDC")]
		private static extern IntPtr GetWindowDC(IntPtr hwnd);

		[SuppressUnmanagedCodeSecurity]
		[DllImport(UserLib, EntryPoint = "ReleaseDC")]
		private static extern bool ReleaseDC(IntPtr hwnd, IntPtr dc);

		public override IntPtr GetProcAddress(IntPtr library, string functionName)
		{
			return ExternGetSystemProcAddress(library, functionName);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int ChangeDisplaySettingsEx([MarshalAs(UnmanagedType.LPTStr)] string lpszDeviceName, DeviceMode lpDevMode, IntPtr hwnd, ChangeDisplaySettingsEnum dwflags, IntPtr lParam);

		[Flags]
		internal enum ChangeDisplaySettingsEnum
		{
			UpdateRegistry = 1,
			Test = 2,
			Fullscreen = 4,
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal class DeviceMode
		{
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			internal string DeviceName;
			internal short SpecVersion;
			internal short DriverVersion;
			private short Size;
			internal short DriverExtra;
			internal int Fields;
			internal POINT Position;
			internal int DisplayOrientation;
			internal int DisplayFixedOutput;
			internal short Color;
			internal short Duplex;
			internal short YResolution;
			internal short TTOption;
			internal short Collate;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			internal string FormName;
			internal short LogPixels;
			internal int BitsPerPel;
			internal int PelsWidth;
			internal int PelsHeight;
			internal int DisplayFlags;
			internal int DisplayFrequency;
			internal int ICMMethod;
			internal int ICMIntent;
			internal int MediaType;
			internal int DitherType;
			internal int Reserved1;
			internal int Reserved2;
			internal int PanningWidth;
			internal int PanningHeight;

			internal DeviceMode()
			{
				this.Size = (short)Marshal.SizeOf((object)this);
			}
		}

		internal struct POINT
		{
			internal int X;
			internal int Y;

			public POINT(int x, int y)
			{
				X = x;
				Y = y;
			}
		}
	}
}