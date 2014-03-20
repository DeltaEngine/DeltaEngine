using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Additional methods for assembly related actions.
	/// </summary>
	public static class AssemblyExtensions
	{
		//ncrunch: no coverage start
		public static string GetTestNameOrProjectName()
		{
			return testOrProjectName ?? (testOrProjectName = StackTraceExtensions.GetEntryName());
		}

		private static string testOrProjectName;

		public static string GetEntryAssemblyForProjectName()
		{
			return projectName ?? (projectName = StackTraceExtensions.GetExecutingAssemblyName());
		}

		private static string projectName;

		public static bool IsAllowed(this AssemblyName assemblyName)
		{
			return IsAllowed(assemblyName.Name);
		}

		internal static bool IsAllowed(string name)
		{
			return !(IsMicrosoftAssembly(name) || IsIdeHelperTool(name) || IsThirdPartyLibrary(name));
		}

		public static bool IsMicrosoftAssembly(string name)
		{
			return name == "mscorlib" || name == "System" || name.StartsWith("System.") ||
				name.StartsWith("Microsoft.") || name.StartsWith("WindowsBase") ||
				name.StartsWith("PresentationFramework") || name.StartsWith("PresentationCore") ||
				name.StartsWith("WindowsFormsIntegration");
		}

		private static bool IsIdeHelperTool(string name)
		{
			return name.StartsWith("NUnit.") || name.StartsWith("nunit.") || name.StartsWith("pnunit.") ||
				name.StartsWith("JetBrains.") || name.StartsWith("NCrunch.") ||
				name.StartsWith("nCrunch.") || name.StartsWith("ReSharper.") || name.StartsWith("vshost32");
		}

		private static bool IsThirdPartyLibrary(string name)
		{
			return ThirdPartyLibsFullNames.Contains(name) ||
				ThirdPartyLibsPartialNames.Any(name.StartsWith);
		}

		private static readonly string[] ThirdPartyLibsFullNames = { "OpenAL32", "wrap_oal",
			"libEGL", "libgles", "libGLESv2", "csogg", "csvorbis", "Autofac", "Moq", "OpenTK",
			"Newtonsoft.Json", "NVorbis", "NAudio", "DotNetZip.Reduced", "Ionic.Zip",
			"UIAutomationTypes" };

		private static readonly string[] ThirdPartyLibsPartialNames = { "libvlc",
			"DynamicProxyGen", "Anonymously Hosted", "Pencil.Gaming", "AvalonDock", "Farseer",
			"MvvmLight", "SharpDX", "SlimDX", "ToyMp3", "EntityFramework", "NHibernate", "Approval",
			"System.IO.Abstractions", "System.Windows.Interactivity", "AsfMojo", "Ionic.",	"Xceed",
			"WPFLocalizeExtension", "XAMLMarkupExtensions", "Glfw", "MonoGame", "GalaSoft.MvvmLight",
			"AurelienRibon." };

		public static bool IsAllowed(this Assembly assembly)
		{
			return IsAllowed(assembly.GetName().Name);
		}

		public static bool IsPlatformAssembly(string assemblyName)
		{
			if (assemblyName.EndsWith(".Tests") || assemblyName.EndsWith(".Remote") ||
				assemblyName == "DeltaEngine.Input")
				return false;
			return assemblyName == "DeltaEngine" || assemblyName == "DeltaEngine.Content.Disk" ||
				assemblyName == "DeltaEngine.Content.Online" ||
				assemblyName.StartsWith("DeltaEngine.Graphics.") ||
				assemblyName.StartsWith("DeltaEngine.Multimedia.") ||
				assemblyName.StartsWith("DeltaEngine.Input.") ||
				assemblyName.StartsWith("DeltaEngine.Platforms") ||
				assemblyName.StartsWith("DeltaEngine.Windows") ||
				assemblyName.StartsWith("DeltaEngine.TestWith");
		}

		/// <summary>
		/// See http://geekswithblogs.net/rupreet/archive/2005/11/02/58873.aspx
		/// </summary>
		public static bool IsManagedAssembly(string fileName)
		{
			using (Stream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var reader = new BinaryReader(fs);
				GoToDataDictionaryOfPeOptionalHeaders(fs, reader);
				var dataDictionaryRva = new uint[16];
				var dataDictionarySize = new uint[16];
				for (int i = 0; i < 15; i++)
				{
					dataDictionaryRva[i] = reader.ReadUInt32();
					dataDictionarySize[i] = reader.ReadUInt32();
				}
				return dataDictionaryRva[14] != 0;
			}
		}

		/// <summary>
		/// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms680313(v=vs.85).aspx
		/// </summary>
		private static void GoToDataDictionaryOfPeOptionalHeaders(Stream fs, BinaryReader reader)
		{
			fs.Position = 0x3C;
			fs.Position = reader.ReadUInt32();
			reader.ReadBytes(24);
			fs.Position = Convert.ToUInt16(Convert.ToUInt16(fs.Position) + 0x60);
		}

		public static bool IsEditorAssembly(string assemblyName)
		{
			return assemblyName.StartsWith("DeltaEngine.Editor");
		}
	}
}