using System;
using System.IO;
using System.Linq;
using DeltaEngine.Content.Json;
using DeltaEngine.Graphics.OpenGL;
using DeltaEngine.Input.Windows;
using DeltaEngine.Multimedia.OpenAL;
using DeltaEngine.Physics2D;
using DeltaEngine.Physics2D.Farseer;
using DeltaEngine.Physics3D;
using DeltaEngine.Physics3D.Jitter;
using DeltaEngine.Platforms.Windows;
using DeltaEngine.Content.Xml;
using DeltaEngine.Graphics;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D;

namespace DeltaEngine.Platforms
{
	public class OpenGLResolver : AppRunner
	{
		private readonly string[] nativeDllsNeeded = { "openal32.dll", "wrap_oal.dll" };

		public OpenGLResolver()
		{
			TryInitializeOpenGL();
		}

		private void TryInitializeOpenGL()
		{
			MakeSureOpenALDllsAreAvailable();
			RegisterCommonEngineSingletons();
			RegisterSingleton<FormsWindow>();
			RegisterSingleton<WindowsSystemInformation>();
			RegisterSingleton<OpenGLDevice>();
			RegisterSingleton<Drawing>();
			RegisterSingleton<BatchRenderer2D>();
			RegisterSingleton<BatchRenderer3D>();
			RegisterSingleton<OpenGL20ScreenshotCapturer>();
			RegisterSingleton<ALSoundDevice>();
			RegisterSingleton<WindowsMouse>();
			RegisterSingleton<WindowsKeyboard>();
			RegisterSingleton<WindowsTouch>();
			RegisterSingleton<WindowsGamePad>();
			Register<InputCommands>();
			if (IsAlreadyInitialized)
				throw new UnableToRegisterMoreTypesAppAlreadyStarted();
		}

		protected override void RegisterMediaTypes()
		{
			base.RegisterMediaTypes();
			Register<OpenGL20Image>();
			Register<OpenGLShader>();
			Register<OpenGL20Geometry>();
			Register<OpenALSound>();
			Register<ALMusic>();
			Register<XmlContent>();
			Register<JsonContent>();
		}

		protected override void RegisterPhysics()
		{
			RegisterSingleton<FarseerPhysics>();
			RegisterSingleton<JitterPhysics>();
			Register<AffixToPhysics2D>();
			Register<AffixToPhysics3D>();
		}

		private void MakeSureOpenALDllsAreAvailable()
		{
			if (AreNativeDllsMissing())
				TryCopyNativeDlls();
		}

		private bool AreNativeDllsMissing()
		{
			return nativeDllsNeeded.Any((string nativeDll) => !File.Exists(nativeDll));
		}

		private void TryCopyNativeDlls()
		{
			if (TryCopyNativeDllsFromNuGetPackage())
				return;
			if (TryCopyNativeDllsFromDeltaEnginePath())
				return;
			throw new FailedToCopyNativeOpenALDllFiles("OpenAL dlls not found inside the application " + "output directory nor inside the %DeltaEnginePath% environment variable. Make sure it's " + "set and containing the required files: " + string.Join(",", nativeDllsNeeded));
		}

		private bool TryCopyNativeDllsFromNuGetPackage()
		{
			string nuGetPackagesPath = FindNuGetPackagesPath();
			string nativeBinariesPath = Path.Combine(nuGetPackagesPath, "packages", "OpenTKWithOpenAL.1.1.1508.5724", "NativeBinaries", "x86");
			if (!Directory.Exists(nativeBinariesPath))
				return false;
			CopyNativeDllsFromPath(nativeBinariesPath);
			return true;
		}

		private static string FindNuGetPackagesPath()
		{
			const int MaxPathLength = 18;
			string path = Path.Combine("..", "..");
			while (!IsPackagesDirectory(path))
			{
				path = Path.Combine(path, "..");
				if (path.Length > MaxPathLength)
					break;
			}
			return path;
		}

		private void CopyNativeDllsFromPath(string nativeBinariesPath)
		{
			foreach (string nativeDll in nativeDllsNeeded)
				File.Copy(Path.Combine(nativeBinariesPath, nativeDll), nativeDll, true);
		}

		private static bool IsPackagesDirectory(string path)
		{
			return Directory.Exists(Path.Combine(path, "packages"));
		}

		private bool TryCopyNativeDllsFromDeltaEnginePath()
		{
			string enginePath = Environment.GetEnvironmentVariable("DeltaEnginePath");
			if (enginePath == null || !Directory.Exists(Path.Combine(enginePath, "OpenGL")))
				return false;
			CopyNativeDllsFromPath(Path.Combine(enginePath, "OpenGL"));
			return true;
		}

		private class FailedToCopyNativeOpenALDllFiles : Exception
		{
			public FailedToCopyNativeOpenALDllFiles(string message)
				: base(message) {}
		}
	}
}